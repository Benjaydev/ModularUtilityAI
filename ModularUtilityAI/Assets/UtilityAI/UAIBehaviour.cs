using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


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

    public float expectedValueRangeMin = 0f;
    public float expectedValueRangeMax = 1f;
    public bool clampScoreWithinRange = true;


    public string displayName = "Behaviour";

    [HideInInspector]
    public float activeDurationSeconds = 0f;


    // The evaluator is a methods that calculates the wanted behaviour value.
    // The user can assign custom methods
    [Tooltip("Method that calculates the wanted behaviour value. Must take UAIBEhaviour as parameter and return float.")]
    public DelegateContainer<float, UAIBehaviour> Evaluator = new DelegateContainer<float, UAIBehaviour>();

    [Header("Conditions")]
    [Tooltip("Whether the behaviour can only be interrupted if all conditions are met. By default only one condition needs to be met.")]
    public bool allConditionsMustBeMet = false;
    // Conditions that when any are met, will stop this behaviour
    [Tooltip("Conditions that when any are met, this behaviour will end. Must return boolean.")]
    public List<DelegateContainer<bool>> InterruptConditions = new List<DelegateContainer<bool>>();

    [Header("Events")]
    public UnityEvent OnStart;
    public UnityEvent WhenActive;
    public UnityEvent OnEnd;


    public UAIBehaviour(string _name = "None", float _valueRangeMin = 0f, float _valueRangeMax = 1f, float _evaluationCooldown = 0.5f)
    {
        expectedValueRangeMin = _valueRangeMin;
        expectedValueRangeMax = _valueRangeMax;
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
        timerDurationCount = 0f;

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
        activeDurationSeconds = isActive ? activeDurationSeconds + Time.deltaTime : 0f;

        evaluationCooldownCount += Time.deltaTime;
        if(evaluationCooldownCount >= evaluationCooldown)
        {
            evaluationCooldownCount = 0;
            rawValue = expectedValueRangeMin;
            value = 0f;
            if (Evaluator.IsValid())
            {
                ValidateEvaluator();
                rawValue = Evaluator.Invoke(this);
                if (clampScoreWithinRange) { rawValue = Mathf.Clamp(rawValue, expectedValueRangeMin, expectedValueRangeMax); }
                // Remap range between 0 - 1
                value = expectedValueRangeMin != 0f || expectedValueRangeMax != 1f ?
                    0f + (rawValue - expectedValueRangeMin) * (1 - 0f) / (expectedValueRangeMax - expectedValueRangeMin) :
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
    public bool TimerCondition(float duration, bool unscaledTime, float deltaTime)
    {
        timerDurationCount += unscaledTime ? deltaTime/Time.timeScale : deltaTime;
        if (timerDurationCount >= duration)
        {

            return true;
        }
        return false;
    }

    // ----------------------------------
    // ----------------------------------

}
