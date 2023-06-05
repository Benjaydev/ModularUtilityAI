using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MyCustomEditor : EditorWindow
{
    public static void ShowMyEditor()
    {
        // This method is called when the user selects the menu item in the Editor
        EditorWindow wnd = GetWindow<MyCustomEditor>();
        wnd.titleContent = new GUIContent("UtilityAI Generator");
    }

    public void CreateGUI()
    {
        // Get a list of all sprites in the project
        string[] allObjectGuids = AssetDatabase.FindAssets("t:" + typeof(UtilityAI).FullName);

        string[] files = Directory.GetFiles(Application.dataPath + "/UtilityAI/Instances", "*.cs", SearchOption.TopDirectoryOnly);
        Debug.Log(files.Length);
        var allObjects = new List<MonoScript>();
        foreach (var file in files)
        {
            MonoScript val = (MonoScript)AssetDatabase.LoadAssetAtPath(file, typeof(MonoScript));
            if (val != null)
            {
                allObjects.Add(val);
            }
        }
        Debug.Log(allObjects.Count);
    }
}