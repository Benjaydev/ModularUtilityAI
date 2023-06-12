using System;
using System.Reflection;
using UnityEngine;



[Serializable]
public abstract class DelegateContainerBase
{
    public MethodInfo method;

    [SerializeField]
    protected GameObject delegateObject;
    [SerializeField]
    protected MonoBehaviour delegateScript;
    [SerializeField]
    protected string delegateMethodName = "None";
    [SerializeField]
    protected int delegateIndex;

    public void Construct(MonoBehaviour script)
    {
        delegateScript = script;
        delegateObject = delegateScript.gameObject;
    }
    public bool IsConstructed()
    {
        return delegateScript != null && delegateObject != null;
    }

    public virtual void Init()
    {
        if (delegateScript != null)
        {
            try
            {
                method = delegateScript.GetType().GetMethod(delegateMethodName, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            }
            catch (Exception e) { Debug.LogWarning(e); }
        }
    }

    public bool IsFresh(){ return method == null || method.Name == delegateMethodName; /* Return whether the current method is the same as the selected method */ }
    public bool IsValid() { return method != null; }

}

[System.Serializable]
public class TypeContainer
{
    [SerializeField]
    private int INT;
    [SerializeField]
    private float FLOAT;
    [SerializeField]
    private string STRING;
    [SerializeField]
    private bool BOOL;
    [SerializeField]
    private GameObject GAMEOBJECT;
    [SerializeField]
    private MonoBehaviour SCRIPT;

    [SerializeField]
    private string usingType = "INT";

    public string ContainerType { get { return usingType; } }

    // Get the object type that is used by the inspector
    public object GetValid()
    {
        switch (usingType)
        {
            case "INT":
            // Enums have to be represented by integers as the container doesn't know the type of enum being used.
            case "ENUM":
                return INT;
            case "FLOAT":
                return FLOAT;
            case "STRING":
                return STRING;
            case "BOOL":
                return BOOL;
            case "GAMEOBJECT":
                return GAMEOBJECT;
            case "SCRIPT":
                return SCRIPT;
            default:
                return null;
        }
    }
}



[Serializable]
public class DelegateContainer<R> : DelegateContainerBase, ISerializationCallbackReceiver
{

    [SerializeField]
    private string typeName = typeof(R).FullName;

    [SerializeField]
    private TypeContainer[] frontEndParameters;

    [SerializeField]
    private object[] backEndParameters;

    public DelegateContainer() { }
    public DelegateContainer(string methodName, MonoBehaviour script, object[] parameters = null)
    {
        Set(methodName, script, parameters);
    }

    public void Set(string methodName, MonoBehaviour script, object[] parameters = null)
    {
        method = script.GetType().GetMethod(methodName);
        delegateMethodName = method.Name;
        delegateScript = script;
        if(delegateScript != null)
        {
            delegateObject = delegateScript.gameObject;
        }

        backEndParameters = parameters;

    }

    public void Init(UAIBehaviour owner)
    {
        base.Init();
        // Get all inspector-set parameters and convert them to object parameters for later invocation
        // Also check for an UAIBehaviour owner parameter
        backEndParameters = new object[frontEndParameters.Length];
        bool ownerFound = false;
        for (int i = 0; i < frontEndParameters.Length; i++)
        {
            // Set first instance of owner to behaviour
            if (!ownerFound && frontEndParameters[i].ContainerType == "OWNER")
            {
                ownerFound = true;
                backEndParameters[i] = owner;
            }
            // Else get regular type
            else { backEndParameters[i] = frontEndParameters[i].GetValid(); }


        }
    }
    public override void Init()
    {
        base.Init();
        // Get all inspector-set parameters and convert them to object parameters for later invocation
        backEndParameters = new object[frontEndParameters.Length];
        for (int i = 0; i < frontEndParameters.Length; i++)
        {
            backEndParameters[i] = frontEndParameters[i].GetValid();
        }
    }   


    public R Invoke(object[] p = null)
    {
        return method == null ? default(R) : (R)method.Invoke(delegateScript, p == null ? backEndParameters : p);
    }



    public void OnBeforeSerialize() { }

    public void OnAfterDeserialize()
    {
        typeName = typeof(R).FullName;
    }

}


[Serializable]
public class DelegateContainer<R, P1> : DelegateContainerBase, ISerializationCallbackReceiver
{
    public delegate R customDelegate(P1 param);
    public customDelegate del;

    [SerializeField]
    private string typeName = typeof(R).FullName;

    [SerializeField]
    private string paramName1 = typeof(P1).FullName;

    public DelegateContainer(){}
    public DelegateContainer(customDelegate m)
    {
        Set(m);
    }
    public void Set(customDelegate m)
    {  
        del = m;
        method = del.Method;
        delegateMethodName = method.Name;
        delegateScript = (MonoBehaviour)m.Target;
        if(delegateScript != null)
        {
            delegateObject = delegateScript.gameObject;
        }

    }


    public override void Init()
    {
        base.Init();
        if(delegateScript != null)
        {
            try
            {
                del = (customDelegate)Delegate.CreateDelegate(typeof(customDelegate), method.IsStatic ? null : delegateScript, method);
            } 
            catch(Exception e) { Debug.LogWarning(e); }
        }
    }

    public R Invoke(P1 param)
    {
        return del == null ? default(R) : del.Invoke(param);
    }


    public void OnBeforeSerialize(){}

    public void OnAfterDeserialize()
    {
        typeName = typeof(R).FullName;
        paramName1 = typeof(P1).FullName;
    }

}


[Serializable]
public class DelegateContainer<R, P1, P2> : DelegateContainerBase, ISerializationCallbackReceiver
{
    public delegate R customDelegate(P1 param, P2 param2);
    public customDelegate del;

    [SerializeField]
    private string typeName = typeof(R).FullName;

    [SerializeField]
    private string paramName1 = typeof(P1).FullName;

    [SerializeField]
    private string paramName2 = typeof(P2).FullName;

    public DelegateContainer(customDelegate m)
    {
        Set(m);
    }
    public void Set(customDelegate m)
    {
        del = m;
        method = del.Method;
        delegateMethodName = method.Name;
        delegateScript = (MonoBehaviour)m.Target;
        if (delegateScript != null)
        {
            delegateObject = delegateScript.gameObject;
        }
    }


    public override void Init()
    {
        base.Init();
        if (delegateScript != null)
        {
            try
            {
                del = (customDelegate)Delegate.CreateDelegate(typeof(customDelegate), method.IsStatic ? null : delegateScript, method);
            }
            catch (Exception e) { Debug.LogWarning(e); }
        }
    }

    public R Invoke(P1 param, P2 param2)
    {
        return del == null ? default(R) : del.Invoke(param, param2);
    }

    public void OnBeforeSerialize() { }

    public void OnAfterDeserialize()
    {
        typeName = typeof(R).FullName;
        paramName1 = typeof(P1).FullName;
        paramName2 = typeof(P2).FullName;
    }
}