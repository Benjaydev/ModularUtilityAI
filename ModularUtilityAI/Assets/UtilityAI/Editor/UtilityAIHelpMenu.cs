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

    public void SetTitle(GUIStyle textStyle)
    {
        textStyle.fontSize = 18;
        textStyle.fontStyle = FontStyle.Bold;
    }
    public void SetNormal(GUIStyle textStyle)
    {
        textStyle.fontSize = 12;
        textStyle.fontStyle = FontStyle.Normal;
    }
    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal("box");

        // Left side vertical
        EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(position.width / 2));

        GUIStyle textStyle = EditorStyles.label;
        textStyle.wordWrap = true;
        textStyle.alignment = TextAnchor.UpperLeft;
        textStyle.normal.textColor = Color.white;

        SetTitle(textStyle);
        EditorGUILayout.LabelField("Using the Utility AI System in the Inspector", textStyle, GUILayout.Height(25));
        SetNormal(textStyle);



        EditorGUILayout.LabelField("To begin using the Utility AI system, follow these steps:\r\n" +
            "1. Begin by creating and opening a new C# script for your AI (or open a previously made script).\r\n\r\n" +
            "2. Inherit your script from the 'UtilityAI' class.\r\n\r\n" +
            "3. Return to the Unity Editor and attach the script to an object in the scene.\r\n\r\n" +
            "4. View your script inside the inspector window, you will be presented with some Utility AI settings.\r\n\r\n" +
            "5. Press the '[Generate]' button to open the generation menu.\r\n\r\n" +
            "6. Add the behaviours you'd like for your AI to the 'Behaviours To Generate' list (Behaviours can be added, removed, or edited at a later stage).\r\n\r\n" +
            "7. Press the generate button below the list. This will create your new Utility AI.", 
            textStyle);



        //Texture2D LogoTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/UtilityAI/Editor/Images/Picture1.png");
        //GUILayout.Label(LogoTex, GUILayout.MaxWidth(position.width/2), GUILayout.MaxHeight(LogoTex.height * (LogoTex.width/position.width)));
        EditorGUILayout.EndVertical();

        // Right side veritical
        EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(position.width / 2));

        SetTitle(textStyle);
        EditorGUILayout.LabelField("Awake, Start, and Update Methods", textStyle, GUILayout.Height(25));
        SetNormal(textStyle);

        EditorGUILayout.LabelField("It is recommended not to use the default Unity Awake, Start, and Update methods inside your AI script. This is because they are used by the back-end AI systems. If you'd like the functionality of these methods, you can implement the AIAwake(), AIStart(), and AIUpdate() methods, which are called automatically.", textStyle);


        textStyle.normal.textColor = Color.yellow;
        EditorGUILayout.LabelField("The default Unity Awake, Start, and Update Methods can be used if necessary by using protected override - You must make sure the base method is called (base.Awake(), base.Start(), or base.Update() recpectively), otherwise the AI systems will not work.", textStyle);
        textStyle.normal.textColor = Color.white;


        EditorGUILayout.EndVertical();



        EditorGUILayout.EndHorizontal();
    }
}