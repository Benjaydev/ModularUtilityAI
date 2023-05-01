using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class CustomFunction {

    public delegate void CustomDelegate();
    public CustomDelegate customDelegate;

    public string name = "Ben";
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

    // Conditions that when any are met, will stop this behaviour
    [SerializeField]
    private CustomFunction customFunction;

    public delegate bool ConditionAction(float currentValue, float rawCurrentValue);
    public List<ConditionAction> InterruptConditions;

    [SerializeField]
    public delegate float EvaluationAction();
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
            rawValue = Evaluater.Invoke();

            // Remap range between 0 - 1
            value = valueRangeMin != 0f || valueRangeMax != 1f ?
                0f + (rawValue - valueRangeMin) * (1 - 0f) / (valueRangeMax - valueRangeMin) :
                rawValue;

        }

        return value;
    }
}
