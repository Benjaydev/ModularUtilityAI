using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScript : MonoBehaviour
{
    public static TimeScript instance;

    public float cycleDurationSeconds = 300;


    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(Time.deltaTime* (360 / cycleDurationSeconds), 0));
    }
}
