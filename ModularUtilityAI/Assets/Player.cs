using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    private Rigidbody rb;
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private float mouseSensitivity = 5f;
    [SerializeField]
    private float speed = 20f;
    private void Awake()
    {
        instance = this;

        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float xr = mouseSensitivity * Input.GetAxis("Mouse X");
        float yr = mouseSensitivity * Input.GetAxis("Mouse Y");

        transform.Rotate(0f, xr, 0f);
        cam.transform.Rotate(-yr, 0f, 0f);

        Vector3 movement = transform.right*Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");

        rb.AddForce(movement * speed);



    }


}
