using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DelegateContainer<T> {

    public delegate T customDelegate(UAIBehaviour behaviour);

    public customDelegate delegateCall;

    [SerializeField]
    private GameObject delegateObject;
    [SerializeField]
    private MonoBehaviour delegateScript;

    [SerializeField]
    private string delegateMethodName;

    public void Init()
    {
        if(delegateObject != null)
        {
            delegateCall = (customDelegate)Delegate.CreateDelegate(typeof(customDelegate), delegateObject, delegateMethodName);
        }
    }

}


[Serializable]
public class UAIBehaviour
{
    private bool isActive = false;
    public bool IsActive() { return isActive; }
    private float value = 0f;
    private float rawValue = 0f;

    public float GetCurrentValue() { return value; }
    public float GetCurrentRawValue() { return rawValue; }

    public float evaluationCooldown = 0.5f;
    private float evaluationCooldownCount = 0f;

    public float valueRangeMin = 0f;
    public float valueRangeMax = 1f;



    public delegate bool ConditionAction(UAIBehaviour behaviour);
    public List<ConditionAction> InterruptConditions;

    public delegate float EvaluationAction(UAIBehaviour behaviour);
    // Conditions that when any are met, will stop this behaviour
    [SerializeField]
    private DelegateContainer<float> evaluator;
    public EvaluationAction Evaluater;

    public UnityEvent WhenActive;
    public UnityEvent OnStart;
    public UnityEvent OnEnd;


    public UAIBehaviour(float _valueRangeMin = 0f, float _valueRangeMax = 1f, float _evaluationCooldown = 0.5f)
    {
        valueRangeMin = _valueRangeMin;
        valueRangeMax = _valueRangeMax;
        evaluationCooldown = _evaluationCooldown;
    }

    public void Init()
    {
        if(evaluator != null)
        {
            evaluator.Init();
            Evaluater = (EvaluationAction)evaluator.delegateCall.GetInvocationList()[0];
        }

    }


    public void End()
    {
        isActive = false;
        OnEnd.Invoke();
    }

    public void Start()
    {
        isActive = true;
        OnStart.Invoke();
    }

    public float Evaluate()
    {


        evaluationCooldownCount += Time.deltaTime;
        if(evaluationCooldownCount >= evaluationCooldown)
        {
            evaluationCooldownCount = 0;
            rawValue = Evaluater.Invoke(this);

            // Remap range between 0 - 1
            value = valueRangeMin != 0f || valueRangeMax != 1f ?
                0f + (rawValue - valueRangeMin) * (1 - 0f) / (valueRangeMax - valueRangeMin) :
                rawValue;

        }

        return value;
    }
}
