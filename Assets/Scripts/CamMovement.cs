using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame

    private float moveSpeed = 0.05f;
    private float rotation = 90.0f;

    void Update()
    {

        // Checks for inputs,

        // If "W" Is pressed,
        if (Input.GetKey("w"))
        {

            // Applys the translation based on the objects rotation.
            transform.Translate(transform.forward * moveSpeed, Space.World);
        }

        if (Input.GetKey("a"))
        {
            transform.Translate(-transform.right * moveSpeed, Space.World);
        }

        if (Input.GetKey("s"))
        {
            transform.Translate(-transform.forward * moveSpeed, Space.World);
        }

        if (Input.GetKey("d"))
        {
            transform.Translate(transform.right * moveSpeed, Space.World);
        }

        if (Input.GetKey("r"))
        {
            transform.Translate(transform.up * moveSpeed, Space.World);
        }

        if (Input.GetKey("f"))
        {
            transform.Translate(-transform.up * moveSpeed, Space.World);
        }

        if (Input.GetKeyDown("q"))
        {
            // Applys a rotation of 90 degrees to the object
            transform.Rotate(-Vector3.up * rotation, Space.World);
        }

        if (Input.GetKeyDown("e"))
        {
            transform.Rotate(Vector3.up * rotation, Space.World);
        }
    }
}
