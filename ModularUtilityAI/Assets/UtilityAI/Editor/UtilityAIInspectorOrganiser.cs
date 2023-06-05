using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UtilityAI), true)]
public class UtilityAIInspectorOrganiser : Editor
{
    private SerializedObject utilityAI;

    private int currentTab = 0;

    private void OnEnable()
    {
        utilityAI = new SerializedObject(target);
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        SerializedProperty tabNames = utilityAI.FindProperty("behaviourNames");

        string[] names = new string[tabNames.arraySize];
        SerializedProperty[] properties = new SerializedProperty[tabNames.arraySize];
        for (int i = 0; i < tabNames.arraySize; i++)
        {
            names[i] = tabNames.GetArrayElementAtIndex(i).stringValue;
            properties[i] = utilityAI.FindProperty("B_" + names[i]);
        }

        currentTab = GUILayout.Toolbar(currentTab, names);

        Object behaviour = properties[currentTab].objectReferenceValue;

    }
}
