using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;


[System.Serializable]
public enum ThirstEnum
{
    EqualTo,
    LessThan,
    GreaterThan,
}

public class AITest : UtilityAI_AITest, IUtilityAIMethods
{
    [SerializeField]
    private NavMeshAgent agent;

    [SerializeField]
    private BoxCollider walkArea;

    [SerializeField]
    private GameObject[] waterSources;
    [SerializeField]
    private GameObject[] beds;
    [SerializeField]
    private GameObject[] foodSources;

    private float thirst = 0;
    private float hunger = 0;
    private float tiredness = 0;



    public void AIAwake()
    {

    }


    public void AIStart()
    {
    }

    public void AIUpdate()
    {
        // Increase thirst
        if (!B_Drink.IsActive())
        {
            thirst = Mathf.Min(10, thirst + 0.05f * Time.deltaTime);
        }

        if(!B_Sleep.IsActive())
        {
            tiredness = Mathf.Min(1, tiredness + Time.deltaTime / (TimeScript.instance.cycleDurationSeconds/2));
        }
    }


    public float HungerEvaluator(UAIBehaviour behaviour)
    {
        return hunger;
    }
    public float ThirstEvaluator(UAIBehaviour behaviour)
    {
        return thirst;
    }

    public float WanderEvaluator(UAIBehaviour behaviour)
    {
        return 0.5f;
    }
    public float SleepEvaluator(UAIBehaviour behaviour)
    {
        return tiredness;
    }

    public void DrinkActive()
    {
        if (IsAtDestination(3))
        {
            thirst = Mathf.Max(0, thirst - (Time.deltaTime/5));
        }

    }
    public void DrinkStart()
    { 
        agent.SetDestination(FindClosest(waterSources).position);
    }

    public void WanderStart()
    {
        agent.SetDestination(RandomPointInBounds(walkArea.bounds));
    }
    public void WanderActive()
    {
        if(agent.isStopped || IsAtDestination(3))
        {
            agent.SetDestination(RandomPointInBounds(walkArea.bounds));
        }
    }


    public void EatStart()
    {
        agent.SetDestination(FindClosest(foodSources).position);
    }
    public void EatActive()
    {
        if (IsAtDestination(3))
        {
            hunger = Mathf.Max(0, hunger - (Time.deltaTime/10));
        }
    }

    public void SleepStart()
    {
        agent.SetDestination(FindRandom(beds).position);
    }
    public void SleepActive()
    {
        if (IsAtDestination(3))
        {
            tiredness = Mathf.Max(0, tiredness - Time.deltaTime / (TimeScript.instance.cycleDurationSeconds / 3));
        }
    }

    public Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    public float TalkEvaluator()
    {
        return 0f;
    }

    public bool TooHungry()
    {
        return hunger >= 10;
    }


    public bool ThirstCheck(float value, ThirstEnum calculationType)
    {
        switch (calculationType)
        {
            case ThirstEnum.EqualTo:
            default:
                return thirst == value;
            case ThirstEnum.LessThan:
                return thirst < value;
            case ThirstEnum.GreaterThan:
                return thirst > value;


        }
    }


    public int CustomBehaviourSelector(float[] weights)
    {
        return 0;
    }


    public Transform FindClosest(GameObject[] gos)
    {
        Transform closest = null;
        float closestDist = float.MaxValue;
        for (int i = 0; i < gos.Length; i++)
        {
            float dist = (gos[i].transform.position - transform.position).sqrMagnitude;
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = gos[i].transform;
            }
        }
        return closest;
    }

    public Transform FindRandom(GameObject[] gos)
    {
        return gos[Random.Range(0, gos.Length+1)].transform;
    }

    public bool IsAtDestination(float dist)
    {
        return (agent.destination - transform.position).sqrMagnitude < dist * dist;
    }



}
