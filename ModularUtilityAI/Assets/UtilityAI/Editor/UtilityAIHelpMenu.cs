using UnityEngine;
using UnityEditor;
using System.Collections;

class UtilityAIHelpMenu : EditorWindow
{
    [MenuItem("UtilityAI/Help")]

    public static void ShowWindow()
    {
        GetWindow(typeof(UtilityAIHelpMenu));
    }

    void OnGUI()
    {
        GUILayout.Label("Using the Utility AI System in the Inspector", EditorStyles.boldLabel);

        GUIStyle textStyle = EditorStyles.label;
        textStyle.wordWrap = true;
        textStyle.alignment = TextAnchor.UpperLeft;


        EditorGUI.LabelField(new Rect(3, 20, position.width/2, position.height), "To use the Utility AI system inside the Unity Inspector, you must first follow a few steps:", textStyle);

        //EditorGUILayout.BeginVertical("box");
        //Texture2D LogoTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/UtilityAI/Editor/Images/Picture1.png");
        //GUILayout.Label(LogoTex, GUILayout.MaxWidth(LogoTex.width), GUILayout.MaxHeight(LogoTex.height));
        //EditorGUILayout.EndVertical();
    }
}