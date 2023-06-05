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


    public string displayName = "Behaviour";

    private bool displayInInspector = false;

    // The evaluator is a methods that calculates the wanted behaviour value.
    // The user can assign custom methods
    [Tooltip("Method that calculates the wanted behaviour value. Must take UAIBEhaviour as parameter and return float.")]
    public DelegateContainer<float, UAIBehaviour> Evaluator = new DelegateContainer<float, UAIBehaviour>();


    // Conditions that when any are met, will stop this behaviour
    [Tooltip("Conditions that when any are met, this behaviour will end. Must return boolean.")]
    public List<DelegateContainer<bool>> InterruptConditions = new List<DelegateContainer<bool>>();

    public UnityEvent WhenActive;
    public UnityEvent OnStart;
    public UnityEvent OnEnd;


    public UAIBehaviour(string _name = "None", float _valueRangeMin = 0f, float _valueRangeMax = 1f, float _evaluationCooldown = 0.5f)
    {
        valueRangeMin = _valueRangeMin;
        valueRangeMax = _valueRangeMax;
        evaluationCooldown = _evaluationCooldown;
        evaluationCooldownCount = _evaluationCooldown;
        displayName = _name;
    }


    public void Init()
    {
        // Override code only evaluator with inspector evaluator
        Evaluator?.Init();

        // Add all inspector condition delegate calls to code only list
        foreach (DelegateContainer<bool> condition in InterruptConditions)
        {
            condition?.Init(this);
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
        if (!Evaluator.IsFresh())
        {
            Evaluator?.Init();
        }

        // Add all inspector condition delegate calls to code only list
        foreach (DelegateContainer<bool> condition in InterruptConditions)
        {
            if (!condition.IsFresh())
            {
                condition?.Init(this);
            }

        }
    }

    public float Evaluate()
    {
        evaluationCooldownCount += Time.deltaTime;
        if(evaluationCooldownCount >= evaluationCooldown)
        {
            evaluationCooldownCount = 0;
            rawValue = valueRangeMin;
            value = 0f;
            if (Evaluator.IsValid())
            {
                ValidateEvaluator();
                rawValue = Evaluator.Invoke(this);
                // Remap range between 0 - 1
                value = valueRangeMin != 0f || valueRangeMax != 1f ?
                    0f + (rawValue - valueRangeMin) * (1 - 0f) / (valueRangeMax - valueRangeMin) :
                    rawValue;
            }
        }

        return value;
    }

    // Default condition methods
    // ----------------------------------
    // ----------------------------------
    // ----------------------------------
    private float timerDurationCount = 0;
    public bool TimerCondition(float duration, bool unscaledTime)
    {
        timerDurationCount += unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        Debug.Log(displayName + " " + timerDurationCount);
        if (timerDurationCount >= duration)
        {

            return true;
        }
        return false;
    }

    // ----------------------------------
    // ----------------------------------

}
