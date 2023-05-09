using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEngine.Windows;

public class UtilityAI_AITest : UtilityAI
{
    [Header("Custom Behaviours")]
    public UAIBehaviour B_Run = new UAIBehaviour("Run", 0f, 1f, 0.3f);
    public UAIBehaviour B_Drink = new UAIBehaviour("Drink", 0f, 1f, 0.3f);
    public UAIBehaviour B_Fight = new UAIBehaviour("Fight", 0f, 1f, 0.3f);
    public UAIBehaviour B_Swim = new UAIBehaviour("Swim", 0f, 1f, 0.3f);

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        behaviours.Add(B_Run);
        behaviours.Add(B_Drink);
        behaviours.Add(B_Fight);
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
           Handles.Label(pos + new Vector3(0, dimensions.y * 2-(textSeperation*2), 0), "Drink: " + (Mathf.Round(B_Drink.GetCurrentValue()*100) / 100).ToString(), guiStyle);
           Handles.Label(pos + new Vector3(0, dimensions.y * 2-(textSeperation*3), 0), "Fight: " + (Mathf.Round(B_Fight.GetCurrentValue()*100) / 100).ToString(), guiStyle);
           Handles.Label(pos + new Vector3(0, dimensions.y * 2-(textSeperation*4), 0), "Swim: " + (Mathf.Round(B_Swim.GetCurrentValue()*100) / 100).ToString(), guiStyle);
        }
    }
#endif
}