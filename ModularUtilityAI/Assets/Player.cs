using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    public GameObject marker;

    [SerializeField]
    private LayerMask rayMask;


    private void Awake()
    {
        instance = this;

    }

    private void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100000, rayMask))
        {
            marker.transform.position = hit.point;
        }
    }


}
