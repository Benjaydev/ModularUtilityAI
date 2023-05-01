using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UIElements;
using System.Reflection;
using UnityEditor.UIElements;

//[CustomEditor(typeof(UAIBehaviour))]
[CustomPropertyDrawer(typeof(DelegateContainer<float>))]
public class DelegateEditorMenu : PropertyDrawer
{
    private SerializedProperty delegateObject;
    private SerializedProperty delegateScript;
    private SerializedProperty delegateMethodName;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if(delegateObject == null)
        {
            delegateObject = property.FindPropertyRelative("delegateObject");
        }
        if(delegateMethodName == null)
        {
            delegateMethodName = property.FindPropertyRelative("delegateMethodName");
        }
        if(delegateScript == null)
        {
            delegateScript = property.FindPropertyRelative("delegateScript");
        }


        return 42f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Create title label
        Rect rect = new Rect(position.x, position.y, position.width, position.height/2);
        EditorGUI.LabelField(rect, new GUIContent(property.displayName + " Function"));

        // Game Object field
        Rect objRect = new Rect(position.x, position.y+20f, position.width/2, position.height/2);
        EditorGUI.PropertyField(objRect, delegateObject, new GUIContent(""));

        GameObject selectedObject = (GameObject)delegateObject.objectReferenceValue;

        if(selectedObject != null)
        {
            MonoBehaviour[] scripts = selectedObject.GetComponents<MonoBehaviour>();
            List<MethodInfo> infos = new List<MethodInfo>();
            // Get all methods from all scripts on selected object
            foreach(MonoBehaviour script in scripts)
            {
                foreach (MethodInfo info in script.GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public))
                {
                    infos.Add(info);
                }
            }

            // Get names of methods
            string[] names = new string[infos.Count];
            for (int i = 0; i < infos.Count; i++)
            {
                //infos[i].ReturnType
                ParameterInfo[] parameters = infos[i].GetParameters();
                if(infos[i].ReturnType == typeof(float) && parameters.Length == 1 && parameters[0].ParameterType == typeof(UAIBehaviour))
                {
                    names[i] = infos[i].DeclaringType.Name + "/" + infos[i].Name;
                }

            }
            if(names.Length > 0)
            {
                // Create dropdown menu with all methodds
                Rect methodRect = new Rect(position.x + position.width / 2, position.y + 22f, position.width / 2, position.height / 2);
                int selectedIndex = 0;
                selectedIndex = EditorGUI.Popup(methodRect, 0, names);

                //sdelegateScript.objectReferenceValue = selectedObject.GetComponent <infos[selectedIndex].DeclaringType> ();
                delegateMethodName.stringValue = infos[selectedIndex].Name;
            }

        }


        EditorGUI.EndProperty();


    }

}