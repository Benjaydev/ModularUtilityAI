using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UIElements;
using System.Reflection;
using UnityEditor.UIElements;

//[CustomEditor(typeof(UAIBehaviour))]
[CustomPropertyDrawer(typeof(CustomFunction))]
public class DelegateEditorMenu : PropertyDrawer
{
    private SerializedProperty functionDelegate;

    private GameObject selectedObject;

    //public override void OnInspectorGUI()
    //{
    //    GUILayout.Label("TEST");
    //    //EditorGUILayout.LabelField("Cool field");

    //    //GameObject mySO = null;
    //    //mySO = (GameObject)EditorGUILayout.ObjectField("GameObject", mySO, typeof(GameObject), true);

    //    //List<System.Reflection.MethodInfo> infos = new List<MethodInfo>();

    //    //foreach (System.Reflection.Assembly ass in System.AppDomain.CurrentDomain.GetAssemblies())
    //    //{
    //    //    foreach (System.Type type in ass.GetTypes())
    //    //    {
    //    //        if (mySO.GetType().IsSubclassOf(type))
    //    //        {
    //    //            foreach (System.Reflection.MethodInfo info in type.GetMethods(BindingFlags.Instance))
    //    //            {
    //    //                infos.Add(info);
    //    //            }
    //    //        }
    //    //    }
    //    //}
    //}


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if(functionDelegate == null)
        {
            functionDelegate = property.FindPropertyRelative("customDelegate");
        }

        return 32f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect rect = new Rect(position.x, position.y, position.width, position.height/2);
        EditorGUI.LabelField(rect, new GUIContent("Delegate"));

        Rect objRect = new Rect(position.x, position.y+16f, position.width/2, position.height/2);
        selectedObject = (GameObject)EditorGUI.ObjectField(objRect, "", selectedObject, typeof(GameObject), true);

        if(selectedObject != null)
        {
            List<System.Reflection.MethodInfo> infos = new List<MethodInfo>();

            foreach (System.Reflection.Assembly ass in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (System.Type type in ass.GetTypes())
                {
                    if (selectedObject.GetType().IsSubclassOf(type))
                    {
                        foreach (System.Reflection.MethodInfo info in type.GetMethods(BindingFlags.Instance))
                        {
                            infos.Add(info);
                        }
                    }
                }
            }
            string[] names = new string[infos.Count];
            for (int i = 0; i < infos.Count; i++)
            {
                names[i] = infos[i].Name;
            }

            Rect methodRect = new Rect(position.x + position.width / 2, position.y + 16f, position.width / 2, position.height / 2);
            EditorGUI.Popup(methodRect, 0, names);
        }


        EditorGUI.EndProperty();


    }


    //public override VisualElement CreatePropertyGUI(SerializedProperty property)
    //{
    //    // Create property container element.
    //    var container = new VisualElement();

    //    // Create property fields.
    //    var del = new PropertyField(property.FindPropertyRelative("name"));

    //    // Add fields to the container.
    //    container.Add(del);

    //    return container;
    //}

}
