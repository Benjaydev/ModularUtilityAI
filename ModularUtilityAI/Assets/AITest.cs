using UnityEngine;
using UnityEngine.AI;

public class AITest : UtilityAI_AITest, IUtilityAIMethods
{
    [SerializeField]
    private NavMeshAgent agent;

    public void AIAwake()
    {
    }

    public void AIStart()
    {
    }

    public void AIUpdate()
    {
    }


    public float RunEvaluator(UAIBehaviour behaviour)
    {
        return Mathf.Max(10 - (transform.position - Player.instance.transform.position).magnitude, 0); 
    }

    public void RunActive()
    {
        agent.SetDestination(transform.position + (transform.position - Player.instance.transform.position).normalized * 5);
    }


    public float TalkEvaluator(UAIBehaviour behaviour)
    {
        return 1f;
    }

    public bool ShouldRun(UAIBehaviour behaviour)
    {
        return B_Run.GetCurrentValue() >= 0.6f;
    }
}
