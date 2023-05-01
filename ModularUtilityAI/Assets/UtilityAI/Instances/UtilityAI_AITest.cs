using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Windows;

public class UtilityAI_AITest : UtilityAI
{
    public UAIBehaviour BWalk = new UAIBehaviour(0f, 1f, 1.23333f);
    public UAIBehaviour BRun = new UAIBehaviour(2f, 9f, 0f);
    public UAIBehaviour BPunch = new UAIBehaviour(324.322f, 99.02f, 0.54f);
    public UAIBehaviour BSwim = new UAIBehaviour(0f, 0f, 0f);
    public UAIBehaviour BJump = new UAIBehaviour(22f, 21f, 0.4f);

// Start is called before the first frame update
    private void Awake()
    {
        BaseAwake();
        behaviours.Add(BWalk);
        behaviours.Add(BRun);
        behaviours.Add(BPunch);
        behaviours.Add(BSwim);
        behaviours.Add(BJump);
    }

    private void Start()
    {
        BaseStart();
    }

    // Update is called once per frame
    private void Update()
    {
        //BaseUpdate();
        Debug.Log("Walk");
        Debug.Log("Run");
        Debug.Log("Punch");
        Debug.Log("Swim");
        Debug.Log("Jump");
    }
}