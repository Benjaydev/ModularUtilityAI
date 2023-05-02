using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using static UnityEngine.GraphicsBuffer;

public class UtilityAI_AITest : UtilityAI
{
    [Header("Custom Behaviours")]
    public UAIBehaviour B_Run = new UAIBehaviour(3f, 2f, 4f);
    public UAIBehaviour B_Fart = new UAIBehaviour(0.33f, 1.23f, 0f);
    public UAIBehaviour B_Punch = new UAIBehaviour(0f, 3f, 3f);

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        behaviours.Add(B_Run);
        behaviours.Add(B_Fart);
        behaviours.Add(B_Punch);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    private void OnDrawGizmos()
    {
        var pos = transform.position;


        GUIStyle guiStyle = new GUIStyle();
        guiStyle.fontSize = 20; //change the font size
        guiStyle.normal.textColor = new Color(1, 0, 0);
        Handles.Label(pos + new Vector3(0, 1, 0), B_Run.GetCurrentValue().ToString(), guiStyle);
        Handles.DrawSolidRectangleWithOutline(new Rect(pos.x - 10, pos.y - 20, 20, 40), new Color(0,0,0,0.5f), Color.white);
    }
}