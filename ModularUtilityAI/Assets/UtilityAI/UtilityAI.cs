using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class UtilityAI : MonoBehaviour
{
    [System.Serializable]
    public struct Creationfield
    {
        public string name;
        public float valueRangeMin;
        public float valueRangeMax;
        public float evaluationCooldown;
    }

    [SerializeField]
    private Creationfield[] fields = new Creationfield[0];

    protected List<UAIBehaviour> behaviours = new List<UAIBehaviour>();
    [System.NonSerialized]
    public UAIBehaviour currentBehaviour;

    public delegate int CustomBehaviourSelector(float[] weights);
    public CustomBehaviourSelector behaviourSelector;


    protected void BaseAwake()
    {
        behaviourSelector = GetIndexOfRandomisedWeights;
    }
    protected void BaseStart()
    {
        foreach(UAIBehaviour behaviour in behaviours)
        {
            behaviour.Init();
        }
    }

    protected void BaseUpdate()
    {
        foreach(UAIBehaviour.ConditionAction condition in currentBehaviour.InterruptConditions){
            if (condition.Invoke(currentBehaviour))
            {
                currentBehaviour.End();
                currentBehaviour = null;
                break;
            }
        }

        if(currentBehaviour == null)
        {
            float[] weights = new float[behaviours.Count];
            for(int i = 0; i < behaviours.Count; i++)
            {
                float val = behaviours[i].Evaluate();
                weights[i] = val;
            }

            // Select new behaviour
            int chosenBehaviourIndex = behaviourSelector.Invoke(weights);
            if(chosenBehaviourIndex >= 0 && chosenBehaviourIndex < behaviours.Count)
            {
                currentBehaviour = behaviours[chosenBehaviourIndex];
            }
        }


        // Call current behaviour events
        if(currentBehaviour.IsActive())
        {
            // Call the events that should happen when this behaviour is active
            currentBehaviour.WhenActive?.Invoke();
        }
    }

    int GetIndexOfRandomisedWeights(float[] weights)
    {
        // Get total sum
        float sum = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            sum += weights[i];
        }

        if (sum == 0)
        {
            return weights.Length - 1;
        }

        float randVal = Random.Range(0, sum);
        sum = 0;

        // Find the value that pushes sum above random value
        for (int i = 0; i < weights.Length; i++)
        {
            sum += weights[i];
            // Return current index
            if (sum >= randVal)
            {
                return i;
            }
        }

        return weights.Length - 1;
    }




    [ContextMenu("Generate AI Instance")]
    public void Build()
    {
        if(GetType().ToString() == "UtilityAI")
        {
            throw new System.Exception("An instance can not be generated using the UtilityAI script. Must create a seperate script which inherits from UtilityAI, then generate.");
        }

        StringBuilder sb = new StringBuilder();

        // Create new cs script which inherits from the scriptbuilder class with the new custom parameters
        string path = Application.dataPath + "/UtilityAI/Instances/";
        string inheritedName = GetType().ToString();
        // Create Folder
        if (!UnityEngine.Windows.Directory.Exists(path)) { UnityEngine.Windows.Directory.CreateDirectory(path); }

        string csName = "UtilityAI_" + inheritedName;

        string template = BuildTemplate(csName);

        bool alreadyExists = UnityEngine.Windows.Directory.Exists(path + csName + ".cs");
        System.IO.File.WriteAllText(path + csName + ".cs", template);


        if (!alreadyExists)
        {
            // Find the path of the script that is using the ScriptBuilder class
            string[] res = System.IO.Directory.GetFiles(Application.dataPath, inheritedName + ".cs", SearchOption.AllDirectories);
            if (res.Length == 0)
            {
                Debug.LogError("Inherited cs file '" + inheritedName + ".cs' could not be found.");
                return;
            }
            string inheritedPath = res[0];


            // Replace ScriptBuilder class inheritence with newly created class (csName)
            string inheritedFileText = System.IO.File.ReadAllText(inheritedPath);

            Regex regex = new Regex("UtilityAI");
            inheritedFileText = regex.Replace(inheritedFileText, csName, 1);

            System.IO.File.WriteAllText(inheritedPath, inheritedFileText);


        }

        // Recompile scripts
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private string BuildTemplate(string csName)
    {
        // Build the template
        string templateBeginning =
            "using System.Collections;\r\n" +
            "using System.Collections.Generic;\r\n" +
            "using System.Text;\r\n" +
            "using UnityEngine;\r\n" +
            "using UnityEngine.Windows;\r\n\r\n" +
            "public class " + csName + " : UtilityAI\r\n" +
            "{\r\n";



        string templateParameters = "";

        string templateAwake =
            "\r\n// Start is called before the first frame update\r\n    " +
            "protected void Awake()\r\n" +
            "    {\r\n" +
            "        BaseAwake();\r\n";


        string templateUpdate =
            "    // Update is called once per frame\r\n" +
            "    protected void Update()\r\n" +
            "    {\r\n" +
            "        BaseUpdate();\r\n";

        for (int i = 0; i < fields.Length; i++)
        {
            // Add the custom parameters for each field
            templateParameters += "    public UAIBehaviour B" + fields[i].name + " = new UAIBehaviour(" + fields[i].valueRangeMin.ToString() + "f, " + fields[i].valueRangeMax.ToString() + "f, " + fields[i].evaluationCooldown.ToString() + "f" + ");\r\n";

            // Add custom parameters to list for iteration use in AI
            templateAwake += "        behaviours.Add(" + "B" + fields[i].name + ");\r\n";

            templateUpdate += "        Debug.Log(\"" + fields[i].name + "\");\r\n";
        }

        templateAwake += "    }\r\n\r\n";

        templateUpdate += "    }";


        return templateBeginning + templateParameters + templateAwake + templateUpdate + "\r\n}";
    }
}
