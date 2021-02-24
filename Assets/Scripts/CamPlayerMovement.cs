using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamPlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent;
    }

    private float moveSpeed = 5.0f;
    private float rotation = 90.0f;

    public float horSensitivity = 1.0f;
    public float verSensitivity = 1.0f;

    private float xRotation = 0.0f;
    private float yRotation = 0.0f;

    // Update is called once per frame

    private Transform parent;

    void Update()
    {
        KeyboardMovement();
        MouseControl();
    }


    void KeyboardMovement()
    {
        // Checks for inputs,

        // If "W" Is pressed,
        if (Input.GetKey("w"))
        {
            // Sets y value to 0 so the player won't move based on tilt.
            var forward = parent.forward;
            forward.y = 0;
           
            // Applys the translation based on the objects rotation.
            parent.Translate(forward * moveSpeed * Time.deltaTime, Space.World);
        }

        if (Input.GetKey("a"))
        {
            parent.Translate(-transform.right * moveSpeed * Time.deltaTime, Space.World);
        }

        if (Input.GetKey("s"))
        {
            var forward = parent.forward;
            forward.y = 0;
            parent.Translate(-forward * moveSpeed * Time.deltaTime, Space.World);
        }

        if (Input.GetKey("d"))
        {
            parent.Translate(transform.right * moveSpeed * Time.deltaTime, Space.World);
        }

        if (Input.GetKeyDown("q"))
        {
            // Applys a rotation of 90 degrees to the object
            parent.Rotate(-Vector3.up * rotation, Space.World);
        }

        if (Input.GetKeyDown("e"))
        {
            parent.Rotate(Vector3.up * rotation, Space.World);
        }
    }

    void MouseControl()
    {
        float mouseX = Input.GetAxis("Mouse X") * horSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * verSensitivity;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -60, 60);

        parent.eulerAngles = new Vector3(xRotation, yRotation, 0.0f);

    }



}
