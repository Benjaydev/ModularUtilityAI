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


    public void Init()
    {
        if (delegateObject != null)
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

[Serializable]
public class DelegateContainer<R, P1> : DelegateContainerBase, ISerializationCallbackReceiver
{
    public delegate R customDelegate(P1 param);

    [SerializeField]
    private string typeName = typeof(R).FullName;

    [SerializeField]
    private string paramName1 = typeof(P1).FullName;

    public DelegateContainer(){}
    public DelegateContainer(customDelegate m)
    {
        method = m.Method;
        delegateMethodName = method.Name;
    }

    public R Invoke(P1 param)
    {
        return method == null ? default(R) : (R)method.Invoke(delegateScript, new object[] { param });
    }

    public void Set(customDelegate m, object script)
    {
        method = m.Method;
        delegateMethodName = method.Name;
        delegateScript = (MonoBehaviour)script;
        delegateObject = delegateScript.gameObject;
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
    public delegate R customDelegate(P1 param);

    [SerializeField]
    private string typeName = typeof(R).FullName;

    [SerializeField]
    private string paramName1 = typeof(P1).FullName;

    [SerializeField]
    private string paramName2 = typeof(P2).FullName;

    public DelegateContainer()
    {
    }
    public DelegateContainer(customDelegate m)
    {
        method = m.Method;
        delegateMethodName = method.Name;
    }

    public R Invoke(P1 param1, P2 param2)
    {
        return method == null ? default(R) : (R)method.Invoke(delegateScript, new object[] { param1, param2 });
    }
    public void Set(customDelegate m, object script)
    {
        method = m.Method;
        delegateMethodName = method.Name;
        delegateScript = (MonoBehaviour)script;
        delegateObject = delegateScript.gameObject;
    }

    public void OnBeforeSerialize() { }

    public void OnAfterDeserialize()
    {
        typeName = typeof(R).FullName;
        paramName1 = typeof(P1).FullName;
        paramName2 = typeof(P2).FullName;
    }
}