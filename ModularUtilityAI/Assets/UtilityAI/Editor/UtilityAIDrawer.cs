using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

[CustomPropertyDrawer(typeof(UtilityAIButton))]
public class UtilityAIDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 42f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        UtilityAI attachedTo = (UtilityAI)property.serializedObject.targetObject;

        Rect rect = new Rect(position.x, position.y, position.width, position.height / 2);
        if(GUI.Button(rect, new GUIContent("Generate AI Instance")))
        {
            attachedTo.SendMessage("Build", SendMessageOptions.DontRequireReceiver);
        };

        EditorGUI.EndProperty();
    }

}