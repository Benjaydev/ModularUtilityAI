using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UtilityAI), true)]
public class UtilityAIInspectorOrganiser : Editor
{
    private SerializedObject utilityAI;
    private List<string> names = new List<string>();
    // Store behaviour property names so they can later be ignored (Also has some other properties that should be ignored)
    private List<string> Bnames = new List<string>() { "BehaviourSelector", "behaviourSelectorCooldown", "behavioursToGenerate", "showDebug", "valueDisplayAreaOffset", "behaviourNames", "m_Script" };
    List<SerializedProperty> properties = new List<SerializedProperty>();
    private int currentTab;


    private void OnEnable()
    {
        utilityAI = new SerializedObject(target);


        names = new List<string>();
    

        // Get all behaviours from the behaviours to generate list (Some of these may not yet be generated, so it must be sorted through)
        SerializedProperty tabNames = utilityAI.FindProperty("behavioursToGenerate");
 
        // Get all the behaviour properties
        for (int i = 0; i < tabNames.arraySize; i++)
        {
            // Get behaviour name
            SerializedProperty b = tabNames.GetArrayElementAtIndex(i);
            string n = b.FindPropertyRelative("name").stringValue;
            string bname = "B_" + n;
            // Try get behaviour property
            SerializedProperty behaviourProp = utilityAI.FindProperty(bname);
            bool alreadyContains = names.Contains(n);

            // Behaviour has been generated, and is not already registered
            if (behaviourProp != null && !alreadyContains)
            {
                names.Add(n);
                properties.Add(behaviourProp);
                Bnames.Add(bname);
            }
            // Behaviour is not genertated but is registered, remove it
            else if(behaviourProp == null && alreadyContains)
            {
                names.Remove(n);
                properties.Remove(behaviourProp);
                Bnames.Remove(bname);

            }
        }

    }
    public override void OnInspectorGUI()
    {
        utilityAI.Update();
        EditorGUI.BeginChangeCheck();

        // Draw default properties excluding the ones which will be customised/removed
        DrawPropertiesExcluding(utilityAI, Bnames.ToArray());

        // Draw the behaviour selector
        EditorGUILayout.PropertyField(utilityAI.FindProperty(Bnames[0]));
        EditorGUILayout.PropertyField(utilityAI.FindProperty(Bnames[1]));

        string[] namesArray = new string[names.Count+3];
        for(int i = 0; i < names.Count; i++)
        {
            namesArray[i] = names[i];
        }
        namesArray[namesArray.Length - 1] = "[Help]";
        namesArray[namesArray.Length - 2] = "[Generate]";
        namesArray[namesArray.Length - 3] = "[Debug]";


        // Tabs
        if (currentTab >= namesArray.Length)
            currentTab = namesArray.Length - 1;
        int newTab = GUILayout.SelectionGrid(currentTab, namesArray, (int)(Screen.width/100f));

        // When switching to new tab, lose focus 
        if(newTab != currentTab)
        {
            GUI.FocusControl(null);
        }

        if(newTab == namesArray.Length - 2)
        {
            SerializedProperty generate = utilityAI.FindProperty(Bnames[2]);
            generate.isExpanded = true;
            EditorGUILayout.PropertyField(generate, true);
            // Generate button
            if (GUILayout.Button("Generate"))
            {
                ((UtilityAI)target).GenerateAIInstance();
            }
        }
        else if(newTab == namesArray.Length - 3)
        {
            EditorGUILayout.PropertyField(utilityAI.FindProperty(Bnames[3]));
            EditorGUILayout.PropertyField(utilityAI.FindProperty(Bnames[4]));
        }
        // Show the behaviour tab
        else if(newTab != namesArray.Length - 1)
        {
            // Current open tab
            properties[newTab].isExpanded = true;
            EditorGUILayout.PropertyField(properties[newTab]);
        }


        EditorGUI.EndChangeCheck();
        utilityAI.ApplyModifiedProperties();

        // The final tab is the generation tab
        if (newTab == namesArray.Length - 1)
        {
            UtilityAIHelpMenu window = (UtilityAIHelpMenu)EditorWindow.GetWindow(typeof(UtilityAIHelpMenu), false, "UtilityAIHelpMenu");
            window.Show();
            newTab = currentTab;
        }
        currentTab = newTab;
    }
}


