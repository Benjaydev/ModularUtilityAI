using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;

[CustomEditor(typeof(UtilityAI), true)]
public class UtilityAIInspectorOrganiser : Editor
{
    private SerializedObject utilityAI;
    private List<string> names = new List<string>();
    // Store behaviour property names so they can later be ignored (Also has some other properties that should be ignored)
    private List<string> Bnames = new List<string>() { "BehaviourSelector", "behavioursToGenerate", "behaviourNames", "m_Script" };
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
        if (!names.Contains("Generate"))
        {
            names.Add("Generate");
        }

    }
    public override void OnInspectorGUI()
    {
        utilityAI.Update();
        EditorGUI.BeginChangeCheck();

        // Get all the behaviour names
        //PropertyInfo pro = target.GetType().GetProperty("BehaviourNames", BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
        //string[] tabNames = (string[])pro.GetValue(target);



        // Draw default properties excluding the ones which will be customised/removed
        DrawPropertiesExcluding(utilityAI, Bnames.ToArray());

        // Draw the behaviour selector
        EditorGUILayout.PropertyField(utilityAI.FindProperty(Bnames[0]));

        // Tabs
        if(currentTab >= names.Count)
            currentTab = names.Count-1;
        currentTab = GUILayout.SelectionGrid(currentTab, names.ToArray(), (int)(Screen.width/100f));

        // The final tab is the generation tab
        if(currentTab == names.Count-1)
        {
            SerializedProperty generate = utilityAI.FindProperty(Bnames[1]);
            generate.isExpanded = true;
            EditorGUILayout.PropertyField(generate, true);
            // Generate button
            if (GUILayout.Button("Generate"))
            {
                ((UtilityAI)target).GenerateAIInstance();

                EditorUtility.SetDirty(((UtilityAI)target));
            }
        }
        // Show the behaviour tab
        else
        {
            // Current open tab
            properties[currentTab].isExpanded = true;
            EditorGUILayout.PropertyField(properties[currentTab]);
        }


        EditorGUI.EndChangeCheck();
        utilityAI.ApplyModifiedProperties();


    }
}


