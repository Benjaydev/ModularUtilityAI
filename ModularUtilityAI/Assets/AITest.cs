using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class AITest : UtilityAI_AITest, IUtilityAIMethods
{
    public void AIAwake()
    {   
        Debug.Log("AIAwake");
    }

    public void AIStart()
    {
        Debug.Log("AIStart");
    }

    public void AIUpdate()
    {
        Debug.Log("AIUpdate");
    }

    public float Test(UAIBehaviour behaviour)
    {
        Debug.Log("Test");
        return 0f;
    } 
    public float FunTest(UAIBehaviour behaviour)
    {
        Debug.Log("TestFun");
        return 1f;
    } 
    public float CoolTest(UAIBehaviour behaviour)
    {
        Debug.Log("TestCool");
        return 5f;
    } 
    public float AwesomeTest(int behaviour)
    {
        Debug.Log("TestAwesome");
        return 9f;
    }

    public bool BadTest(UAIBehaviour behaviour)
    {
        Debug.Log("BAD");
        return true;
    }

    public int WeightTest(float[] weights)
    {
        Debug.Log("Weight");
        return 0;
    }
}
