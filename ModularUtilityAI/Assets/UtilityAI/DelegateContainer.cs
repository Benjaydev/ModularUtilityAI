using System;
using System.Reflection;
using UnityEngine;

[Serializable]
public class DelegateContainerBase
{
    [SerializeField]
    protected GameObject delegateObject;
    [SerializeField]
    protected MonoBehaviour delegateScript;
    [SerializeField]
    protected string delegateMethodName;
    [SerializeField]
    protected int delegateIndex;

}

[Serializable]
public class DelegateContainer<R, P1> : DelegateContainerBase, ISerializationCallbackReceiver
{
    public delegate R customDelegate(P1 param);

    public customDelegate delegateCall;

    [SerializeField]
    private string typeName = typeof(R).FullName;

    [SerializeField]
    private string paramName1 = typeof(P1).FullName;

    public DelegateContainer(customDelegate del)
    {
        Set(del);
    }

    public void Set(customDelegate d)
    {
        delegateCall = d;
        delegateMethodName = d.Method.Name;
    }

    public R Invoke(P1 param)
    {
        return delegateCall != null ? delegateCall.Invoke(param) : default(R);
    }

    public void Init()
    {
        if (delegateObject != null)
        {
            try { 
                delegateCall = (customDelegate)Delegate.CreateDelegate(typeof(customDelegate), delegateScript, delegateMethodName); 
            }
            catch(Exception e) { 
                //delegateCall = null; 
            }
        }
    }

    public bool IsFresh() { return delegateCall == null || delegateCall.GetInvocationList()[0].GetMethodInfo().Name == delegateMethodName; /* Return whether the current delegate method is the same as the selected method */ }

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
    public delegate R customDelegate(P1 param1, P2 param2);

    public customDelegate delegateCall;

    [SerializeField]
    private string typeName = typeof(R).FullName;

    [SerializeField]
    private string paramName1 = typeof(P1).FullName;

    [SerializeField]
    private string paramName2 = typeof(P2).FullName;

    public DelegateContainer(customDelegate del)
    {
        Set(del);
    }

    public void Set(customDelegate d)
    {
        delegateCall = d;
    }
    public R Invoke(P1 param1, P2 param2)
    {
        return delegateCall != null ? delegateCall.Invoke(param1, param2) : default(R);
    }

    public void Init()
    {
        if (delegateObject != null)
        {
            try { delegateCall = (customDelegate)Delegate.CreateDelegate(typeof(customDelegate), delegateScript, delegateMethodName); }
            catch { delegateCall = null; }
        }
    }

    public bool IsFresh() { return delegateCall == null || delegateCall.GetInvocationList()[0].GetMethodInfo().Name == delegateMethodName; /* Return whether the current delegate method is the same as the selected method */ }

    public void OnBeforeSerialize() { }

    public void OnAfterDeserialize()
    {
        typeName = typeof(R).FullName;
        paramName1 = typeof(P1).FullName;
        paramName2 = typeof(P2).FullName;
    }
}