using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


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

    // The evaluator is a methods that calculates the wanted behaviour value.
    // The user can assign custom methods
        // Code only evaluator
    public delegate float EvaluationAction(UAIBehaviour behaviour);
    public EvaluationAction Evaluator = EmptyEvaluator;
        // Inspector exposed evaluator
    [SerializeField]
    [Tooltip("Method that calculates the wanted behaviour value. Must take UAIBEhaviour as parameter and return float.")]
    private DelegateContainer<float, UAIBehaviour> evaluator;


    // Conditions that when any are met, will stop this behaviour
        // Code only conditions
    public delegate bool ConditionAction(UAIBehaviour behaviour);
    public List<ConditionAction> InterruptConditions = new List<ConditionAction>();
        // Inspector exposed conditions
    [SerializeField]
    [Tooltip("Conditions that when any are met, this behaviour will end. Must take UAIBEhaviour as parameter and return boolean.")]
    private DelegateContainer<bool, UAIBehaviour>[] interruptConditions = new DelegateContainer<bool, UAIBehaviour>[1];


    public UnityEvent WhenActive;
    public UnityEvent OnStart;
    public UnityEvent OnEnd;


    public UAIBehaviour(float _valueRangeMin = 0f, float _valueRangeMax = 1f, float _evaluationCooldown = 0.5f)
    {
        valueRangeMin = _valueRangeMin;
        valueRangeMax = _valueRangeMax;
        evaluationCooldown = _evaluationCooldown;
        evaluationCooldownCount = _evaluationCooldown;
    }

    private static float EmptyEvaluator(UAIBehaviour behaviour)
    {
        return behaviour.value;
    }


    public void Init()
    {
    
        // Override code only evaluator with inspector evaluator
        evaluator?.Init();
        Evaluator = evaluator.delegateCall.Invoke;

        Debug.Log(evaluator.delegateCall.GetType().FullName);

        // Add all inspector condition delegate calls to code only list
        foreach (DelegateContainer<bool, UAIBehaviour> condition in interruptConditions)
        {
            condition?.Init();
            InterruptConditions.Add(condition.delegateCall.Invoke);
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
    private void ValidateEvaluator()
    {
        if (!evaluator.IsFresh())
        {
            evaluator.Init();
            Evaluator = evaluator.delegateCall.Invoke;
        }

        // Add all inspector condition delegate calls to code only list
        foreach (DelegateContainer<bool, UAIBehaviour> condition in interruptConditions)
        {
            if (!condition.IsFresh())
            {
                condition?.Init();
                InterruptConditions.Add(condition.delegateCall.Invoke);
            }

        }
    }

    public float Evaluate()
    {

        evaluationCooldownCount += Time.deltaTime;
        if(evaluationCooldownCount >= evaluationCooldown)
        {
            evaluationCooldownCount = 0;
            ValidateEvaluator();
            rawValue = Evaluator.Invoke(this);

            // Remap range between 0 - 1
            value = valueRangeMin != 0f || valueRangeMax != 1f ?
                0f + (rawValue - valueRangeMin) * (1 - 0f) / (valueRangeMax - valueRangeMin) :
                rawValue;

        }

        return value;
    }
}
