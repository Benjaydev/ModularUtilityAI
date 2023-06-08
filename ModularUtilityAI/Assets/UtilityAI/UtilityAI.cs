using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
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
        public float expectedValueRangeMin;
        public float expectedValueRangeMax;
        public float evaluationCooldown;

    }

    [Header("Instance Generator")]
    [Tooltip("The custom behaviours to generate in the custom UtilityAI")]
    // Custom behaviour fields
    [SerializeField]
    private Creationfield[] behavioursToGenerate = new Creationfield[0];

    protected List<UAIBehaviour> behaviours = new List<UAIBehaviour>();
    [System.NonSerialized]
    public UAIBehaviour currentBehaviour;

    // The method used to choose new behaviours.
    // Delegate that returns an index of a supplied weight. Must take float array as parameter and return int.
    [Header("Behaviour Systems")]
    [Tooltip("The method used to choose new behaviours. Returns an index of a supplied weight. Must take float array as parameter and return int.\r\n E.g. Takes in an array of 5 floats, returns an index from 0-4.")]
    public DelegateContainer<int, float[]> BehaviourSelector = new DelegateContainer<int, float[]>(GetIndexOfRandomisedWeights);
    public float behaviourSelectorCooldown = 1f;
    private float behaviourSelectorCooldownCount = 0f;

    public bool showDebug = false;
    public Vector3 valueDisplayAreaOffset = new Vector3(0,0,0);


    protected virtual void Awake()
    {
        BehaviourSelector?.Init();
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
                currentBehaviour?.End();
                currentBehaviour = behaviours[i];
                currentBehaviour.Start();
            }

            weights[i] = val;
        }

        behaviourSelectorCooldownCount += Time.deltaTime;

        // If there is no current behaviour or behaviour has no conditions
        if (currentBehaviour != null && currentBehaviour.IsActive())
        {
            if (currentBehaviour.WhenActive.GetPersistentEventCount() > 0)
            {
                ExecutionTester.SetStartEvent();
            }
            // Call the events that should happen when this behaviour is active
            currentBehaviour.WhenActive.Invoke();

            if(behaviourSelectorCooldownCount >= behaviourSelectorCooldown)
            {
                // Check all interrupt conditions
                foreach (DelegateContainer<bool> condition in currentBehaviour.InterruptConditions)
                {
                    if (condition.Invoke())
                    {
                        ChooseNewBehaviour(weights);
                        break;
                    }
                    // if condition failed, and all conditions have to be met for a change, then break;
                    if(currentBehaviour.allConditionsMustBeMet)
                    {
                        break;
                    }
                }
                behaviourSelectorCooldownCount = 0;
            }

        }

        // If there is no active behaviour, find a new one
        if(currentBehaviour == null || !currentBehaviour.IsActive())
        {
            ChooseNewBehaviour(weights);
        }

        // Call child AI update functionality
        SendMessage("AIUpdate", SendMessageOptions.DontRequireReceiver);
    }

    private void ChooseNewBehaviour(float[] weights)
    {
        // Select new behaviour
        int chosenBehaviourIndex = BehaviourSelector.Invoke(weights);
        if (chosenBehaviourIndex >= 0 && chosenBehaviourIndex < behaviours.Count)
        {
            currentBehaviour?.End();
            currentBehaviour = behaviours[chosenBehaviourIndex];
            currentBehaviour.Start();
        }
    }


    // Default weight calculation methods
    // ----------------------------------
    // ----------------------------------
    // ----------------------------------
    public static int GetIndexOfRandomisedWeights(float[] weights)
    {
        ExecutionTester.SetEndDelegate();
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

        return Random.Range(0, weights.Length+1);
    }
    public static int GetIndexOfHighestWeight(float[] weights)
    {
        ExecutionTester.SetEndDelegate();
        int highestIndex = 0;
        float highestWeight = weights[0];
        // Find the value that pushes sum above random value
        for (int i = 1; i < weights.Length; i++)
        {
            if(weights[i] > highestWeight)
            {
                highestIndex = i;
                highestWeight = weights[i];
            }
        }

        return highestIndex;
    }
    public static int GetIndexOfLowestWeight(float[] weights)
    {
        ExecutionTester.SetEndDelegate();
        int lowestIndex = 0;
        float lowestWeight = weights[0];
        // Find the value that pushes sum above random value
        for (int i = 1; i < weights.Length; i++)
        {
            if (weights[i] < lowestWeight)
            {
                lowestIndex = i;
                lowestWeight = weights[i];
            }
        }

        return lowestIndex;
    }
    // ----------------------------------
    // ----------------------------------
    // ----------------------------------


    // Default condition methods
    // ----------------------------------
    // ----------------------------------
    // ----------------------------------
    public bool TimerCondition(float duration, bool unscaledTime, UAIBehaviour owner)
    {
        ExecutionTester.SetEndMethodInfo();
        return owner.TimerCondition(duration, unscaledTime, behaviourSelectorCooldown);
    }

    // Score comparisons
    public bool ScoreLessThan(float value, bool rawScore, UAIBehaviour owner)
    {
        ExecutionTester.SetEndMethodInfo();
        float score = rawScore ? owner.GetCurrentRawValue() : owner.GetCurrentValue();
        return score < value;
    }
    public bool ScoreGreaterThan(float value, bool rawScore, UAIBehaviour owner)
    {
        ExecutionTester.SetEndMethodInfo();
        float score = rawScore ? owner.GetCurrentRawValue() : owner.GetCurrentValue();
        return score > value;
    }
    public bool ScoreLessThanOrEqualTo(float value, bool rawScore, UAIBehaviour owner)
    {
        ExecutionTester.SetEndMethodInfo();
        float score = rawScore ? owner.GetCurrentRawValue() : owner.GetCurrentValue();
        return score <= value;
    }
    public bool ScoreGreaterThanOrEqualTo(float value, bool rawScore, UAIBehaviour owner)
    {
        ExecutionTester.SetEndMethodInfo();
        float score = rawScore ? owner.GetCurrentRawValue() : owner.GetCurrentValue();
        return score <= value;
    }
    public bool ScoreEqualTo(float value, bool rawScore, UAIBehaviour owner)
    {
        ExecutionTester.SetEndMethodInfo();
        float score = rawScore ? owner.GetCurrentRawValue() : owner.GetCurrentValue();
        return score == value;
    }
    public bool ScoreNotEqualTo(float value, bool rawScore, UAIBehaviour owner)
    {
        ExecutionTester.SetEndMethodInfo();
        float score = rawScore ? owner.GetCurrentRawValue() : owner.GetCurrentValue();
        return score != value;
    }

    public bool ScoreWithinRange(float min, float max, bool rawScore, UAIBehaviour owner)
    {
        ExecutionTester.SetEndMethodInfo();
        float score = rawScore ? owner.GetCurrentRawValue() : owner.GetCurrentValue();
        return score >= min && score <= max;
    }

    // ----------------------------------
    // ----------------------------------
    // ----------------------------------

    // Default evaluation methods
    // ----------------------------------
    // ----------------------------------
    // ----------------------------------
    public static float EvaluateToOne(UAIBehaviour behaviour)
    {
        ExecutionTester.SetEndDelegate();
        return 1f;
    }
    public static float EvaluateToMin(UAIBehaviour behaviour)
    {
        ExecutionTester.SetEndDelegate();
        return behaviour.expectedValueRangeMin;
    }
    public static float EvaluateToMax(UAIBehaviour behaviour)
    {
        ExecutionTester.SetEndDelegate();
        return behaviour.expectedValueRangeMax;
    }
    // ----------------------------------
    // ----------------------------------
    // ----------------------------------

#if UNITY_EDITOR

    [ContextMenu("Generate AI Instance")]
    [ExecuteInEditMode]
    public void GenerateAIInstance()
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
        System.IO.File.WriteAllText(path + csName + ".cs", template);

        // Find the path of the script that is inheriting the ScriptBuilder class
        string[] res = System.IO.Directory.GetFiles(Application.dataPath, inheritedName + ".cs", SearchOption.AllDirectories);
        if (res.Length == 0)
        {
            Debug.LogError("Inherited cs file '" + inheritedName + ".cs' could not be found.");
            return;
        }
        string inheritedPath = res[0];

        // Replace UtilityAI class inheritence with newly created class (csName)
        string inheritedFileText = System.IO.File.ReadAllText(inheritedPath);
        // If the inheritence change hasn't already been completed
        if(inheritedFileText.IndexOf(inheritedName + " : " + csName) == -1)
        {
            // Find where the class definition begins
            int ind = inheritedFileText.IndexOf(inheritedName);
            int endInd = ind;
            // Find where the class definition ends
            while (inheritedFileText[endInd] != '\\' && inheritedFileText[endInd] != '{')
            {
                endInd++;
            }
            endInd--;

            // Replace the class definition line with one that inherits from newly created UtilityAI class
            inheritedFileText = inheritedFileText.Remove(ind, endInd - ind -1).Insert(ind, inheritedName + " : " + csName);
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
            "using UnityEditor;\r\n" +
            "using UnityEngine.Windows;\r\n\r\n" +
            "public class " + csName + " : UtilityAI\r\n" +
            "{\r\n";



        string templateParameters = "    \r\n";
        string templateBehaviourNamesField = "    [SerializeField] \r\n    private string[] behaviourNames = new string[" + behavioursToGenerate.Length + "] {";

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


        string templateGizmos =
            "#if UNITY_EDITOR\r\n" +
            "    // Update is called once per frame\r\n" +
            "    private void OnDrawGizmos()\r\n" +
            "    {\r\n" +
            "       if(showDebug) {\r\n" +
            "           var pos = transform.position + valueDisplayAreaOffset;\r\n" +
            "           float dist = (Camera.current.transform.position - pos).sqrMagnitude;\r\n" +
            "           if(dist < 40000)\r\n" +
            "           {\r\n" +
            "               dist = Mathf.Sqrt(dist) / 10;\r\n" +
            "               GUIStyle guiStyle = new GUIStyle();\r\n" +
            "               guiStyle.alignment = TextAnchor.MiddleCenter;\r\n" +
            "               guiStyle.normal.textColor = Color.red;\r\n" +
            "               guiStyle.fontSize = (int)(20f / dist);\r\n" +
            "               float textSeperation = 0.4f;\r\n" +
            "               Vector2 dimensions = new Vector2(1, 0.4f*" + behavioursToGenerate.Length + ");\r\n\r\n" +
            "               Handles.Label(pos + new Vector3(0, dimensions.y * 2, 0), \"Current: \" + (currentBehaviour != null ? currentBehaviour.displayName : \"None\"), guiStyle);\r\n";


        for (int i = 0; i < behavioursToGenerate.Length; i++)
        {
            // Add the custom parameters for each field
            templateParameters += "    public UAIBehaviour B_" + behavioursToGenerate[i].name + " = new UAIBehaviour(\"" + behavioursToGenerate[i].name + "\", " + behavioursToGenerate[i].expectedValueRangeMin.ToString() + "f, " + behavioursToGenerate[i].expectedValueRangeMax.ToString() + "f, " + behavioursToGenerate[i].evaluationCooldown.ToString() + "f" + ");\r\n";
            
            if(i < behavioursToGenerate.Length - 1){ templateBehaviourNamesField += "\"" + behavioursToGenerate[i].name + "\", "; }
            else{ templateBehaviourNamesField += "\"" + behavioursToGenerate[i].name + "\" };\r\n"; }

            // Add custom parameters to list for iteration use in AI
            templateAwake += "        behaviours.Add(B_" + behavioursToGenerate[i].name + ");\r\n";


            templateGizmos += "               Handles.Label(pos + new Vector3(0, dimensions.y * 2-(textSeperation*" + (i + 1) + "), 0), \"" + behavioursToGenerate[i].name + ": \" + (Mathf.Round(B_" + behavioursToGenerate[i].name + ".GetCurrentValue()*100) / 100).ToString(), guiStyle);\r\n";

            //templateUpdate += "        Debug.Log(\"" + fields[i].name + "\");\r\n";
        }

        templateAwake += "    }\r\n\r\n";

        templateUpdate += "    }\r\n\r\n";
        templateGizmos += "        }    }\r\n    }\r\n#endif";

        return templateBeginning + templateParameters + templateBehaviourNamesField + templateAwake + templateUpdate + templateGizmos + "\r\n}";
    }

#endif
}
