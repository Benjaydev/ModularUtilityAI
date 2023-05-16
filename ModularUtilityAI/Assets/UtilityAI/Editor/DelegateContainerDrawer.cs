using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using Unity.VisualScripting;

[CustomPropertyDrawer(typeof(DelegateContainer<,>))]
public class DelegateContainerDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 42f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty delegateObject = property.FindPropertyRelative("delegateObject");
        SerializedProperty delegateIndex = property.FindPropertyRelative("delegateIndex");
        SerializedProperty delegateMethodName = property.FindPropertyRelative("delegateMethodName");
        SerializedProperty delegateScript = property.FindPropertyRelative("delegateScript");

        EditorGUI.BeginProperty(position, label, property);


        // Create title label
        Rect titleRect = new Rect(position.x, position.y, position.width, position.height / 2);
        EditorGUI.LabelField(titleRect, new GUIContent(label));

        // Game Object field
        Rect objRect = new Rect(position.x, position.y+20f, position.width/2, position.height/2);
        EditorGUI.PropertyField(objRect, delegateObject, new GUIContent(""));

        GameObject selectedObject = (GameObject)delegateObject.objectReferenceValue;

        // Get return type property
        SerializedProperty delegateTypeName = property.FindPropertyRelative("typeName");

        // Find all possible delegate parameters
        List<SerializedProperty> delegateParams = new List<SerializedProperty>();
        int paramCount = 1;
        string paramDisp = "";
        while (true)
        {
            SerializedProperty param = property.FindPropertyRelative("paramName" + paramCount);
            if (param == null)
            {
                break;
            }
            delegateParams.Add(param);
            if (paramCount > 1)
            {
                paramDisp += ", ";
            }
            paramDisp += param.stringValue;
            paramCount++;
        }

        // Create title label
        Rect methodLabel = new Rect(position.x + 4 + position.width / 2, position.y, position.width, position.height / 2);
        EditorGUI.LabelField(methodLabel, new GUIContent(delegateTypeName.stringValue + " (" + paramDisp + ")"));

        if (selectedObject != null)
        {

            List<MethodInfo> infos = new List<MethodInfo>();
            List<MonoBehaviour> infosScripts = new List<MonoBehaviour>();


            // Search all scripts on selected object
            foreach (MonoBehaviour script in selectedObject.GetComponents<MonoBehaviour>())
            {
                // Get all methods from script
                foreach (MethodInfo info in script.GetType().GetMethods(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static))
                {
                    ParameterInfo[] parameters = info.GetParameters();
                    // Parameter count match
                    if(parameters.Length == delegateParams.Count)
                    {
                        // Check to see if all parameter of method match container
                        bool failed = false;
                        for (int i = 0; i < delegateParams.Count; i++)
                        {
                            if (delegateParams[i].stringValue != parameters[i].ParameterType.FullName)
                            {
                                failed = true;
                                break;
                            }
                        }

                        // Check whether method's return type matches container's return type, there is only 1 parameters, and this parameter is of type UAIBehaviour
                        if (!failed && info.ReturnType.FullName == delegateTypeName.stringValue)
                        {
                            // This is a valid method
                            infosScripts.Add(script);
                            infos.Add(info);
                        }
                    }


                }
            }



            // Get names of methods
            string[] names = new string[infos.Count];
            for (int i = 0; i < infos.Count; i++)
            {
                names[i] = (infos[i].DeclaringType.Name + "/" + infos[i].Name);
                if (infos[i].Name == delegateMethodName.stringValue && delegateScript.objectReferenceValue == infosScripts[i])
                {
                    delegateIndex.intValue = i;
                    delegateObject.objectReferenceValue = ((MonoBehaviour)delegateScript.objectReferenceValue).gameObject;
                }
            }

            // If there are any valid methods avaliable
            if (names.Length > 0)
            {
                // Create dropdown menu with all methods
                Rect methodRect = new Rect(position.x + (position.width / 2) + 4, position.y + 22f, (position.width / 2) - 4, position.height / 2);
                delegateIndex.intValue = EditorGUI.Popup(methodRect, delegateIndex.intValue, names);

                // Save reference to script

                delegateScript.objectReferenceValue = infosScripts[delegateIndex.intValue];

                // Save method name

                delegateMethodName.stringValue = infos[delegateIndex.intValue].Name;
            }
            // No methods avaliable
            else
            {
                // Create empty dropdown menu
                Rect methodRect = new Rect(position.x + (position.width / 2) + 4, position.y + 20f, (position.width / 2) - 4, position.height / 2);
                GUI.enabled = false;
                EditorGUI.Popup(methodRect, 0, new string[1] { "None" });

                GUI.enabled = true;

            }
            

        }
        // Object is null
        else
        {
            // Create empty dropdown Menu
            Rect methodRect = new Rect(position.x + (position.width / 2) + 4, position.y + 20f, (position.width / 2) - 4, position.height / 2);
            GUI.enabled = false;
            EditorGUI.Popup(methodRect, 0, new string[1] { delegateMethodName.stringValue == "None" ? "None" : delegateMethodName.stringValue + " (Default)" });
            GUI.enabled = true;
        }

        EditorGUI.EndProperty();


    }

}