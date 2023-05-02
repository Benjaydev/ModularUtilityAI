using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class UtilityAIButton
{ 

}

public interface IUtilityAIMethods
{
    void AIAwake();
    void AIStart();
    void AIUpdate();
}

[System.Serializable]
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

    [Header("Instance Generator")]
    [Tooltip("The custom behaviours to generate in the custom UtilityAI")]
    // Custom behaviour fields
    [SerializeField]
    private Creationfield[] behavioursToGenerate = new Creationfield[0];
    // Button used to show a button in inspector
    [SerializeField]
    [Tooltip("Generate a custom UtilityAI class with the custom behaviour fields.")]
    public UtilityAIButton generateButton = new UtilityAIButton();


    protected List<UAIBehaviour> behaviours = new List<UAIBehaviour>();
    [System.NonSerialized]
    public UAIBehaviour currentBehaviour;

    // The method used to choose new behaviours.
    // Delegate that returns an index of a supplied weight. Must take float array as parameter and return int.
    // Code only selector
    [Tooltip("The method used to choose new behaviours. Returns an index of a supplied weight. Must take float array as parameter and return int.\r\n E.g. Takes in an array of 5 floats, returns an index from 0-4.")]
    public delegate int CustomBehaviourSelector(float[] weights);
    public CustomBehaviourSelector BehaviourSelector;
        // Inspector exposed selector
    [Header("Behaviour Systems")]
    [Tooltip("The method used to choose new behaviours. Returns an index of a supplied weight. Must take float array as parameter and return int.\r\n E.g. Takes in an array of 5 floats, returns an index from 0-4.")]
    [SerializeField]
    private DelegateContainer<int, float[]> behaviourSelector;

    protected virtual void Awake()
    {
        BehaviourSelector = GetIndexOfRandomisedWeights;
        // If inspector exposed selector is valid, replace base code only version
        if(behaviourSelector != null)
        {
            BehaviourSelector = behaviourSelector.delegateCall.Invoke;
        }

        // Call child AI awake functionality
        SendMessage("AIAwake", SendMessageOptions.DontRequireReceiver);
    }

     protected virtual void Start()
     {
        foreach (UAIBehaviour behaviour in behaviours)
        {
            behaviour.Init();
        }

        // Call child AI start functionality
        SendMessage("AIStart", SendMessageOptions.DontRequireReceiver);
    }

    protected virtual void Update()
    {
        // Get all evaluated values of behaviours
        float[] weights = new float[behaviours.Count];
        for (int i = 0; i < behaviours.Count; i++)
        {
            float val = behaviours[i].Evaluate();

            // If another behaviour has been manually activated
            if (behaviours[i].IsActive() && behaviours[i] != currentBehaviour)
            {
                currentBehaviour.End();
                currentBehaviour = behaviours[i];
            }

            weights[i] = val;
        }

        // If there is no current behaviour
        if (currentBehaviour == null || !currentBehaviour.IsActive())
        {
            // Select new behaviour
            int chosenBehaviourIndex = BehaviourSelector.Invoke(weights);
            if (chosenBehaviourIndex >= 0 && chosenBehaviourIndex < behaviours.Count)
            {
                currentBehaviour = behaviours[chosenBehaviourIndex];
            }
        }


        // Call current behaviour events
        if (currentBehaviour.IsActive())
        {
            // Call the events that should happen when this behaviour is active
            currentBehaviour.WhenActive?.Invoke();

            foreach (UAIBehaviour.ConditionAction condition in currentBehaviour.InterruptConditions)
            {
                if (condition.Invoke(currentBehaviour))
                {
                    currentBehaviour.End();
                    currentBehaviour = null;
                    break;
                }
            }
        }

        // Call child AI updat functionality
        SendMessage("AIUpdate", SendMessageOptions.DontRequireReceiver);
    }


    public int GetIndexOfRandomisedWeights(float[] weights)
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
    private void Build()
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
        // Check whether this AI instance already exists. If it does, it only needs to be edited and no further steps are needed.
        bool alreadyExists = UnityEngine.Windows.File.Exists(path + csName + ".cs");
        System.IO.File.WriteAllText(path + csName + ".cs", template);


        if (!alreadyExists)
        {
            // Find the path of the script that is inheriting the ScriptBuilder class
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
            inheritedFileText = regex.Replace(inheritedFileText, csName + ", IUtilityAIMethods", 1);

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



        string templateParameters = "    [Header(\"Custom Behaviours\")]\r\n";

        string templateAwake =
            "\r\n    // Start is called before the first frame update\r\n    " +
            "protected override void Awake()\r\n" +
            "    {\r\n" +
            "        base.Awake();\r\n";


        string templateUpdate =
            "    // Update is called once per frame\r\n" +
            "    protected override void Update()\r\n" +
            "    {\r\n" +
            "        base.Update();\r\n";

        for (int i = 0; i < behavioursToGenerate.Length; i++)
        {
            // Add the custom parameters for each field
            templateParameters += "    public UAIBehaviour B" + behavioursToGenerate[i].name + " = new UAIBehaviour(" + behavioursToGenerate[i].valueRangeMin.ToString() + "f, " + behavioursToGenerate[i].valueRangeMax.ToString() + "f, " + behavioursToGenerate[i].evaluationCooldown.ToString() + "f" + ");\r\n";

            // Add custom parameters to list for iteration use in AI
            templateAwake += "        behaviours.Add(" + "B" + behavioursToGenerate[i].name + ");\r\n";

            //templateUpdate += "        Debug.Log(\"" + fields[i].name + "\");\r\n";
        }

        templateAwake += "    }\r\n\r\n";

        templateUpdate += "    }";


        return templateBeginning + templateParameters + templateAwake + templateUpdate + "\r\n}";
    }
}
