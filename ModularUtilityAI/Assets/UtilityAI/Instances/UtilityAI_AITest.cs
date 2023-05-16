using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEngine.Windows;

public class UtilityAI_AITest : UtilityAI
{
    [Header("Custom Behaviours")]
    public UAIBehaviour B_Wander = new UAIBehaviour("Wander", 0f, 0f, 0f);
    public UAIBehaviour B_Drink = new UAIBehaviour("Drink", 0f, 0f, 0f);
    public UAIBehaviour B_Eat = new UAIBehaviour("Eat", 0f, 0f, 0f);
    public UAIBehaviour B_Talk = new UAIBehaviour("Talk", 0f, 0f, 0f);

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        behaviours.Add(B_Wander);
        behaviours.Add(B_Drink);
        behaviours.Add(B_Eat);
        behaviours.Add(B_Talk);
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

           Handles.Label(pos + new Vector3(0, dimensions.y * 2, 0), "Current: " + (currentBehaviour != null ? currentBehaviour.displayName : "None"), guiStyle);
           Handles.Label(pos + new Vector3(0, dimensions.y * 2-(textSeperation*1), 0), "Wander: " + (Mathf.Round(B_Wander.GetCurrentValue()*100) / 100).ToString(), guiStyle);
           Handles.Label(pos + new Vector3(0, dimensions.y * 2-(textSeperation*2), 0), "Drink: " + (Mathf.Round(B_Drink.GetCurrentValue()*100) / 100).ToString(), guiStyle);
           Handles.Label(pos + new Vector3(0, dimensions.y * 2-(textSeperation*3), 0), "Eat: " + (Mathf.Round(B_Eat.GetCurrentValue()*100) / 100).ToString(), guiStyle);
           Handles.Label(pos + new Vector3(0, dimensions.y * 2-(textSeperation*4), 0), "Talk: " + (Mathf.Round(B_Talk.GetCurrentValue()*100) / 100).ToString(), guiStyle);
        }
    }
#endif
}