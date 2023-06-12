using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEngine.Windows;

public class UtilityAI_AITest : UtilityAI
{
    
    public UAIBehaviour B_Wander = new UAIBehaviour("Wander", 0f, 0f, 0f);
    public UAIBehaviour B_Drink = new UAIBehaviour("Drink", 0f, 0f, 0f);
    public UAIBehaviour B_Eat = new UAIBehaviour("Eat", 0f, 0f, 0f);
    public UAIBehaviour B_Talk = new UAIBehaviour("Talk", 0f, 0f, 0f);
    public UAIBehaviour B_Sleep = new UAIBehaviour("Sleep", 0f, 0f, 10f);
    [SerializeField] 
    private string[] behaviourNames = new string[5] {"Wander", "Drink", "Eat", "Talk", "Sleep" };

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        behaviours.Add(B_Wander);
        behaviours.Add(B_Drink);
        behaviours.Add(B_Eat);
        behaviours.Add(B_Talk);
        behaviours.Add(B_Sleep);
    }

    // Update is called once per frame
    private void OnGUI()
    {
       if(showDebug && Camera.main != null) {
           Vector3 pos = transform.position + valueDisplayAreaOffset;
           float dist = (Camera.main.transform.position - pos).sqrMagnitude;
           if(dist < 40000)
           {
               GUIStyle guiStyle = new GUIStyle();
               guiStyle.normal.textColor = Color.red;
               guiStyle.alignment = TextAnchor.MiddleCenter;
               dist = Mathf.Sqrt(dist) / 10;
               guiStyle.fontSize = (int)(20f / dist) * debugFontSize;
               float textSeperation = 13f * (2f / dist);
               Vector2 screenPos = Camera.main.WorldToScreenPoint(pos);
               GUI.Label(new Rect(screenPos.x-45, Screen.height - screenPos.y - 10, 45, 10), "Current: " + (currentBehaviour != null ? currentBehaviour.displayName : "None"), guiStyle);
               GUI.Label(new Rect(screenPos.x - 45, Screen.height - screenPos.y - 10 + (textSeperation * 1), 45, 10), "Wander: " + (Mathf.Round(B_Wander.GetCurrentValue()*100) / 100).ToString(), guiStyle);
               GUI.Label(new Rect(screenPos.x - 45, Screen.height - screenPos.y - 10 + (textSeperation * 2), 45, 10), "Drink: " + (Mathf.Round(B_Drink.GetCurrentValue()*100) / 100).ToString(), guiStyle);
               GUI.Label(new Rect(screenPos.x - 45, Screen.height - screenPos.y - 10 + (textSeperation * 3), 45, 10), "Eat: " + (Mathf.Round(B_Eat.GetCurrentValue()*100) / 100).ToString(), guiStyle);
               GUI.Label(new Rect(screenPos.x - 45, Screen.height - screenPos.y - 10 + (textSeperation * 4), 45, 10), "Talk: " + (Mathf.Round(B_Talk.GetCurrentValue()*100) / 100).ToString(), guiStyle);
               GUI.Label(new Rect(screenPos.x - 45, Screen.height - screenPos.y - 10 + (textSeperation * 5), 45, 10), "Sleep: " + (Mathf.Round(B_Sleep.GetCurrentValue()*100) / 100).ToString(), guiStyle);
        }
    }
    }

}