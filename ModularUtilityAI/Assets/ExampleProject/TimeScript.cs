using UnityEngine;

public class TimeScript : MonoBehaviour
{
    public static TimeScript instance;

    public float cycleDurationSeconds = 300;

    public float timeOfDay = 0.5f;

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {

        transform.Rotate(new Vector3(Time.deltaTime* (360 / cycleDurationSeconds), 0));
        timeOfDay += Time.deltaTime/cycleDurationSeconds;
        if(timeOfDay > 1)
        {
            timeOfDay -= 1;
        }

    }

    public bool TimeOfDayWithinRange(float min, float max)
    {
        return timeOfDay >= min && timeOfDay <= max;
    }
}
