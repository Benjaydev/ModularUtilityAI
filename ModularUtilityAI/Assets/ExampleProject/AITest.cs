using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class AITest : UtilityAI_AITest
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

    [SerializeField]
    private Animator animator;
    [SerializeField]
    private GameObject model;


    public static List<GameObject> instances = new List<GameObject>();
    private List<GameObject> otherAI = new List<GameObject>();

    private Transform closestTalker;



    private float thirst = 0;
    private float hunger = 0;
    private float tiredness = 0;


    public void AIAwake()
    {
        instances.Add(gameObject);
    }


    public void AIStart()
    {
    }

    public void AIUpdate()
    {

        // Keep track of all other ai
        if(instances.Count > otherAI.Count+1)
        {
            otherAI = new List<GameObject>(instances);
            otherAI.Remove(gameObject);
        }

        // Increase thirst
        if (!B_Drink.IsActive())
        {
            thirst += Time.deltaTime / (TimeScript.instance.cycleDurationSeconds / 5);
        }

        if (!B_Sleep.IsActive())
        {
            tiredness += Time.deltaTime / (TimeScript.instance.cycleDurationSeconds / 2)*(TimeScript.instance.timeOfDay >= 0.75f || TimeScript.instance.timeOfDay <= 0.25f ? 2 : 1);
        }

        if (!B_Eat.IsActive())
        {
            hunger += Time.deltaTime / (TimeScript.instance.cycleDurationSeconds / 5);
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
        return 1 - ((B_Eat.GetCurrentValue()+ B_Drink.GetCurrentValue() + B_Sleep.GetCurrentValue() + B_Talk.GetCurrentValue()) / 4);
    }
    public float SleepEvaluator(UAIBehaviour behaviour)
    {
        return tiredness;
    }

    public float TalkEvaluator(UAIBehaviour behaviour)
    {
        Transform closest = FindClosest(otherAI);

        if (Physics.Raycast(transform.position, closest.position - transform.position, out RaycastHit hit, 100))
        {
            if (hit.collider.gameObject == closest.gameObject)
            {
                float dist = (closest.position - transform.position).sqrMagnitude;
                return Mathf.Max(0f, 1f - (dist / 100f) - (behaviour.activeDurationSeconds / 20));
            }

        }
        return 0;

    }

    public void DrinkStart()
    {
        agent.SetDestination(FindClosest(waterSources).position);
        ResetAnimations();
        animator.SetBool("Wander", true);

    }
    public void DrinkActive()
    {
        if (IsAtDestination(3))
        {
            thirst = Mathf.Max(0, thirst - (Time.deltaTime/2));
            if (animator.GetBool("Drink") == false)
            {
                ResetAnimations();
            }
            animator.SetBool("Drink", true);
        }

    }


    public void WanderStart()
    {
        agent.SetDestination(RandomPointInBounds(walkArea.bounds));
        ResetAnimations();
        animator.SetBool("Wander", true);
    }
    public void WanderActive()
    {
        if (agent.isStopped || IsAtDestination(3))
        {
            agent.SetDestination(RandomPointInBounds(walkArea.bounds));
        }
    }


    public void EatStart()
    {
        agent.SetDestination(FindClosest(foodSources).position);
        ResetAnimations();
        animator.SetBool("Wander", true);
    }
    public void EatActive()
    {
        if (IsAtDestination(3))
        {
            hunger = Mathf.Max(0, hunger - (Time.deltaTime/10));
            if (animator.GetBool("Drink") == false)
            {
                ResetAnimations();
            }
            animator.SetBool("Drink", true);
        }
    }

    public void SleepStart()
    {
        agent.SetDestination(FindRandom(beds).position);
        ResetAnimations();
        animator.SetBool("Wander", true);

    }
    public void SleepActive()
    {
        if (IsAtDestination(3))
        {
            tiredness = Mathf.Max(0f, tiredness - Time.deltaTime / (TimeScript.instance.cycleDurationSeconds / 2) * (TimeScript.instance.timeOfDay >= 0.75f || TimeScript.instance.timeOfDay <= 0.25f ? 2 : 1));

            if (animator.GetBool("Sleep") == false)
            {
                ResetAnimations();
            }
            animator.SetBool("Sleep", true);
            model.transform.localPosition = new Vector3(0, 0.6f, 0);
        }
    }
    public void SleepEnd()
    {
        ResetAnimations();
        model.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void TalkStart()
    {
        closestTalker = FindClosest(otherAI).transform;
        agent.SetDestination(transform.position);
        ResetAnimations();
        animator.SetBool("Talk", true);
    }
    public void TalkActive()
    {
        transform.rotation = Quaternion.LookRotation(closestTalker.position - transform.position);
        transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z));
    }

    public Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    public Transform FindClosest(GameObject[] gos)
    {
        Transform closest = null;
        float closestDist = float.MaxValue;
        for (int i = 0; i < gos.Length; i++)
        {
            // Only search distance on xy plane
            Vector2 xz = new Vector2(gos[i].transform.position.x, gos[i].transform.position.z);
            Vector2 xz2 = new Vector2(transform.position.x, transform.position.z);
            float dist = (xz - xz2).sqrMagnitude;
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = gos[i].transform;
            }
        }
        return closest;
    }
    public Transform FindClosest(List<GameObject> gos)
    {
        Transform closest = null;
        float closestDist = float.MaxValue;
        for (int i = 0; i < gos.Count; i++)
        {
            // Only search distance on xy plane
            Vector2 xz = new Vector2(gos[i].transform.position.x, gos[i].transform.position.z);
            Vector2 xz2 = new Vector2(transform.position.x, transform.position.z);
            float dist = (xz - xz2).sqrMagnitude;
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
        if(gos.Length == 1)
        {
            return gos[0].transform;
        }
        return gos[Random.Range(0, gos.Length-1)].transform;
    }

    public bool IsAtDestination(float dist)
    {
        return (agent.destination - transform.position).sqrMagnitude < dist * dist;
    }

    public void ResetAnimations()
    {
        animator.SetBool("Wander", false);
        animator.SetBool("Drink", false); 
        animator.SetBool("Sleep", false);
        animator.SetBool("Talk", false);
    }

}
