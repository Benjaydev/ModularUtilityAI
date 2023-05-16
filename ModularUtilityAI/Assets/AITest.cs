using UnityEngine;
using UnityEngine.AI;

public class AITest : UtilityAI_AITest, IUtilityAIMethods
{
    [SerializeField]
    private NavMeshAgent agent;


    private float thirst = 0;
    private float hunger = 0;

    public void AIAwake()
    {
    }


    public void AIStart()
    {
    }

    public void AIUpdate()
    {
        thirst = Mathf.Min(10, thirst + Random.Range(0f, 2f) * Time.deltaTime);
    }


    public float RunEvaluator(UAIBehaviour behaviour)
    {

        return Mathf.Max(10 - (transform.position - Player.instance.marker.transform.position).magnitude, 0); 
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


    public void RunActive()
    {
        agent.SetDestination(transform.position + (transform.position - Player.instance.marker.transform.position).normalized * 5);
        hunger = Mathf.Min(10, hunger + Random.Range(0f, 2f) * Time.deltaTime);
    }


    public void EatActive()
    {
        hunger = Mathf.Max(0, hunger-Time.deltaTime);
    }
    public void DrinkActive()
    {
        thirst = Mathf.Max(0, thirst - Time.deltaTime*2);
    }

    public float TalkEvaluator(UAIBehaviour behaviour)
    {
        return 0f;
    }

    public bool TooHungry(UAIBehaviour behaviour)
    {
        return hunger >= 10;
    }

    public bool NotThirsty(UAIBehaviour behaviour)
    {
        return thirst <= 0;
    }


    public int CustomBehaviourSelector(float[] weights)
    {
        return 0;
    }

}
