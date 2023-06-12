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
        Rect objRect = new Rect(position.x, position.y + 20f, position.width / 2, position.height / 2);
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
                    if (parameters.Length == delegateParams.Count)
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


    // Custom drawer for MethodInfo DelegateContainer 

    [CustomPropertyDrawer(typeof(DelegateContainer<>))]
    public class DelegateContainerDrawerSingle : PropertyDrawer
    {
        float propertySizeDefault = 42f;
        float propertySize = 42f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return propertySize;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty delegateObject = property.FindPropertyRelative("delegateObject");
            SerializedProperty delegateIndex = property.FindPropertyRelative("delegateIndex");
            SerializedProperty delegateMethodName = property.FindPropertyRelative("delegateMethodName");
            SerializedProperty delegateScript = property.FindPropertyRelative("delegateScript");

            EditorGUI.BeginProperty(position, label, property);


            // Create title label
            Rect titleRect = new Rect(position.x, position.y, position.width, 21f);
            EditorGUI.LabelField(titleRect, new GUIContent(label));

            // Game Object field
            Rect objRect = new Rect(position.x, position.y + 20f, position.width / 2, 21f);
            EditorGUI.PropertyField(objRect, delegateObject, new GUIContent(""));

            GameObject selectedObject = (GameObject)delegateObject.objectReferenceValue;

            // Get return type property
            SerializedProperty delegateTypeName = property.FindPropertyRelative("typeName");

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
                        // Check whether method's return type matches container's return type, there is only 1 parameters, and this parameter is of type UAIBehaviour
                        if (info.ReturnType.FullName == delegateTypeName.stringValue)
                        {
                            // This is a valid method
                            infosScripts.Add(script);
                            infos.Add(info);
                        }
                    }
                }

                // Get names of methods
                string[] names = new string[infos.Count];
                for (int i = 0; i < infos.Count; i++)
                {
                    string paramTypes = "";
                    ParameterInfo[] paramInfos = infos[i].GetParameters();
                    for (int p = 0; p < paramInfos.Length; p++)
                    {
                        paramTypes += paramInfos[p].ParameterType.HumanName() + (p != paramInfos.Length-1 ? "," : "");
                    }

                    names[i] = (infos[i].DeclaringType.Name + "/" + infos[i].Name + "(" + paramTypes + ")");
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
                    Rect methodRect = new Rect(position.x + (position.width / 2) + 4, position.y + 22f, (position.width / 2) - 4, 21f);
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
                    Rect methodRect = new Rect(position.x + (position.width / 2) + 4, position.y + 20f, (position.width / 2) - 4, 21f);
                    GUI.enabled = false;
                    EditorGUI.Popup(methodRect, 0, new string[1] { "None" });

                    GUI.enabled = true;

                }

                // If method is chosen
                if (delegateIndex.intValue >= 0 && delegateIndex.intValue < names.Length)
                {
                    SerializedProperty paramContainers = property.FindPropertyRelative("frontEndParameters");
                    ParameterInfo[] parameters = infos[delegateIndex.intValue].GetParameters();

                    propertySize = propertySizeDefault + 22f * parameters.Length+1;

                    // If array is too big
                    while (paramContainers.arraySize > parameters.Length)
                    {
                        paramContainers.DeleteArrayElementAtIndex(paramContainers.arraySize-1);
                    }

                    // Loop through all parameters of method and create input fields for them
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        // If array is too small
                        if(paramContainers.arraySize < parameters.Length)
                        {
                            paramContainers.InsertArrayElementAtIndex(i);
                        }

                        // Positioning and label
                        Rect newRect = new Rect(position.x + 5f + position.width / 2, position.y + 20f + (22f * (i + 1)), (position.width / 2) - 5f, 21f);
                        GUIStyle style = new GUIStyle();
                        style.alignment = TextAnchor.MiddleRight;
                        style.normal.textColor = Color.white;
                        EditorGUI.LabelField(new Rect(newRect.x - (position.width / 2) + 5f, newRect.y, newRect.width, newRect.height), new GUIContent(parameters[i].HumanName() + " : "), style);

                        // Parameter
                        SerializedProperty param = paramContainers.GetArrayElementAtIndex(i);
                        ParameterInfo p = parameters[i];

                        SerializedProperty usingType = param.FindPropertyRelative("usingType");

                        // The owner UAIBehaviour parameter is a default parameter that can be added to a function. It should not show an input field,
                        // but instead will automatically use the owning UAIBehaviour.
                        if(p.ParameterType == typeof(UAIBehaviour) && p.Name.ToLower() == "owner")
                        {
                            EditorGUI.LabelField(newRect, new GUIContent("This"));
                            usingType.stringValue = "OWNER";
                            continue;
                        }

                        // Int Field
                        if (p.ParameterType == typeof(int))
                        {
                            usingType.stringValue = "INT";
                            SerializedProperty fP = param.FindPropertyRelative("INT");
                            fP.intValue = EditorGUI.IntField(newRect, fP.intValue);
                        }
                        // Float Field
                        else if (p.ParameterType == typeof(float))
                        {
                            usingType.stringValue = "FLOAT";
                            SerializedProperty fP = param.FindPropertyRelative("FLOAT");
                            fP.floatValue = EditorGUI.FloatField(newRect, fP.floatValue);
                        }
                        // String Field
                        else if (p.ParameterType == typeof(string))
                        {
                            usingType.stringValue = "STRING";
                            SerializedProperty fP = param.FindPropertyRelative("STRING");
                            fP.stringValue = EditorGUI.TextField(newRect, fP.stringValue);
                        }
                        // Bool Field
                        else if (p.ParameterType == typeof(bool))
                        {
                            usingType.stringValue = "BOOL";
                            SerializedProperty fP = param.FindPropertyRelative("BOOL");
                            fP.boolValue = EditorGUI.Toggle(newRect, fP.boolValue);
                        }
                        // Enum Field
                        else if (p.ParameterType.IsEnum)
                        {
                            // Can't use EnumPopup as it doesn't allow the use of integers to represent enums, and integers are necessary for the back end (The container doesn't know what enum type is being used, so it must use int)
                            usingType.stringValue = "ENUM";
                            SerializedProperty fP = param.FindPropertyRelative("INT");
                            fP.intValue = EditorGUI.Popup(newRect, fP.intValue, p.ParameterType.GetEnumNames());
                        }
                        // GameObject Field
                        else if (p.ParameterType == typeof(GameObject))
                        {
                            usingType.stringValue = "GAMEOBJECT";
                            SerializedProperty fP = param.FindPropertyRelative("GAMEOBJECT");
                            EditorGUI.ObjectField(newRect, fP, new GUIContent());
                        }
                        // Script Field
                        else 
                        {
                            usingType.stringValue = "SCRIPT";
                            SerializedProperty fP = param.FindPropertyRelative("SCRIPT");
                            EditorGUI.ObjectField(newRect, fP, parameters[i].ParameterType, new GUIContent());
                        }

                    }
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
}