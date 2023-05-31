using UnityEngine;
using UnityEngine.AI;

public class AITest : UtilityAI_AITest, IUtilityAIMethods
{
    [SerializeField]
    private NavMeshAgent agent;

    [SerializeField]
    private BoxCollider walkArea;

    [SerializeField]
    private GameObject[] waterSources;



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
        thirst = Mathf.Min(10, thirst + 0.5f * Time.deltaTime);



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
        if ((agent.destination - transform.position).sqrMagnitude < 3 * 3)
        {
            thirst = Mathf.Max(0, thirst - Time.deltaTime * 3);
        }

    }
    public void DrinkStart()
    {
        Transform closest = null;
        float closestDist = float.MaxValue;
        for(int i = 0; i < waterSources.Length; i++)
        {
            float dist = (waterSources[i].transform.position - transform.position).sqrMagnitude;
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = waterSources[i].transform;
            }
        }
        agent.SetDestination(closest.position);
    }

    public void WanderActive()
    {
        if(agent.isStopped || (agent.destination-transform.position).sqrMagnitude < 3*3)
        {
            agent.SetDestination(RandomPointInBounds(walkArea.bounds));
        }
    }
    public void WanderStart()
    {
        agent.SetDestination(RandomPointInBounds(walkArea.bounds));
    }

    public Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
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
