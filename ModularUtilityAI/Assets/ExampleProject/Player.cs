using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    public GameObject marker;

    [SerializeField]
    private LayerMask rayMask;

    [SerializeField]
    private GameObject parentRotator;


    private void Awake()
    {
        instance = this;

    }

    private bool startClick = true;
    private Vector2 startMousePos;
    private Vector2 startRotation;

    private void Update()
    {
        if(Input.GetMouseButton(1))
        {
            if (startClick)
            {
                startMousePos = Input.mousePosition;
                startRotation = parentRotator.transform.rotation.eulerAngles;
                startClick = false;
            }

            Vector2 mouseDiff = (Vector2)Input.mousePosition - startMousePos;
            parentRotator.transform.rotation = Quaternion.Euler(startRotation.x - mouseDiff.y, startRotation.y + mouseDiff.x, 0);

        }
        if (Input.GetMouseButtonUp(1))
        {
            startClick = true;
        }
        transform.Translate(Vector3.forward * Input.mouseScrollDelta.y);






        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100000, rayMask))
        {
            marker.transform.position = hit.point;
        }
    }


    public void SetTimeScale(float scale)
    {
        Time.timeScale = scale;
    }
}
