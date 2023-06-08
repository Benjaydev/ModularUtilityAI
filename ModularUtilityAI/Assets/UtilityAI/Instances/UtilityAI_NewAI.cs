using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEngine.Windows;

public class UtilityAI_NewAI : UtilityAI
{
    
    public UAIBehaviour B_Test1 = new UAIBehaviour("Test1", 0f, 0f, 0f);
    public UAIBehaviour B_Test2 = new UAIBehaviour("Test2", 0f, 0f, 0f);
    public UAIBehaviour B_Test3 = new UAIBehaviour("Test3", 0f, 0f, 0f);
    public UAIBehaviour B_Test4 = new UAIBehaviour("Test4", 0f, 0f, 0f);
    public UAIBehaviour B_Test5 = new UAIBehaviour("Test5", 0f, 0f, 0f);
    [SerializeField] 
    private string[] behaviourNames = new string[5] {"Test1", "Test2", "Test3", "Test4", "Test5" };

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        behaviours.Add(B_Test1);
        behaviours.Add(B_Test2);
        behaviours.Add(B_Test3);
        behaviours.Add(B_Test4);
        behaviours.Add(B_Test5);
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
       if(showDebug) {
           var pos = transform.position + valueDisplayAreaOffset;
           float dist = (Camera.current.transform.position - pos).sqrMagnitude;
           if(dist < 40000)
           {
               dist = Mathf.Sqrt(dist) / 10;
               GUIStyle guiStyle = new GUIStyle();
               guiStyle.alignment = TextAnchor.MiddleCenter;
               guiStyle.normal.textColor = Color.red;
               guiStyle.fontSize = (int)(20f / dist);
               float textSeperation = 0.4f;
               Vector2 dimensions = new Vector2(1, 0.4f*5);

               Handles.Label(pos + new Vector3(0, dimensions.y * 2, 0), "Current: " + (currentBehaviour != null ? currentBehaviour.displayName : "None"), guiStyle);
               Handles.Label(pos + new Vector3(0, dimensions.y * 2-(textSeperation*1), 0), "Test1: " + (Mathf.Round(B_Test1.GetCurrentValue()*100) / 100).ToString(), guiStyle);
               Handles.Label(pos + new Vector3(0, dimensions.y * 2-(textSeperation*2), 0), "Test2: " + (Mathf.Round(B_Test2.GetCurrentValue()*100) / 100).ToString(), guiStyle);
               Handles.Label(pos + new Vector3(0, dimensions.y * 2-(textSeperation*3), 0), "Test3: " + (Mathf.Round(B_Test3.GetCurrentValue()*100) / 100).ToString(), guiStyle);
               Handles.Label(pos + new Vector3(0, dimensions.y * 2-(textSeperation*4), 0), "Test4: " + (Mathf.Round(B_Test4.GetCurrentValue()*100) / 100).ToString(), guiStyle);
               Handles.Label(pos + new Vector3(0, dimensions.y * 2-(textSeperation*5), 0), "Test5: " + (Mathf.Round(B_Test5.GetCurrentValue()*100) / 100).ToString(), guiStyle);
        }    }
    }
#endif
}