using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEngine.Windows;

public class UtilityAI_AITest : UtilityAI
{
    [Header("Custom Behaviours")]
    public UAIBehaviour B_Run = new UAIBehaviour("Run", 0f, 0f, 0f);
    public UAIBehaviour B_Punch = new UAIBehaviour("Punch", 0f, 0f, 0f);
    public UAIBehaviour B_Walk = new UAIBehaviour("Walk", 0f, 0f, 0f);
    public UAIBehaviour B_Swim = new UAIBehaviour("Swim", 0f, 0f, 0f);

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        behaviours.Add(B_Run);
        behaviours.Add(B_Punch);
        behaviours.Add(B_Walk);
        behaviours.Add(B_Swim);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

#if UNITY_EDITOR
    // Update is called once per frame
    private void OnDrawGizmos()
    {
       var pos = transform.position;
       float dist = (Camera.current.transform.position - pos).sqrMagnitude;
       if(dist < 40000)
       {
           dist = Mathf.Sqrt(dist) / 10;
           GUIStyle guiStyle = new GUIStyle();
           guiStyle.alignment = TextAnchor.MiddleCenter;
           guiStyle.normal.textColor = Color.red;
           guiStyle.fontSize = (int)(20f / dist);
           float textSeperation = 0.4f;
           Vector2 dimensions = new Vector2(1, 0.4f*4);

           Handles.Label(pos + new Vector3(0, dimensions.y * 2, 0), "Current: " + (currentBehaviour != null ? currentBehaviour.name : "None"), guiStyle);
           Handles.Label(pos + new Vector3(0, dimensions.y * 2-(textSeperation*1), 0), "Run: " + (Mathf.Round(B_Run.GetCurrentValue()*100) / 100).ToString(), guiStyle);
           Handles.Label(pos + new Vector3(0, dimensions.y * 2-(textSeperation*2), 0), "Punch: " + (Mathf.Round(B_Punch.GetCurrentValue()*100) / 100).ToString(), guiStyle);
           Handles.Label(pos + new Vector3(0, dimensions.y * 2-(textSeperation*3), 0), "Walk: " + (Mathf.Round(B_Walk.GetCurrentValue()*100) / 100).ToString(), guiStyle);
           Handles.Label(pos + new Vector3(0, dimensions.y * 2-(textSeperation*4), 0), "Swim: " + (Mathf.Round(B_Swim.GetCurrentValue()*100) / 100).ToString(), guiStyle);
        }
    }
#endif
}