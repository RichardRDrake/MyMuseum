using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent;
    }

    // Update is called once per frame

    private float moveSpeed = 5.0f;
    private float rotation = 90.0f;

    /* Hi Shaun, Joe here.
     * I've made a couple of changes to your code here, main change being that we move the parent instead of the camera itself
     * this is so the camera can be angled downwards
     * I've also multiplied your translations by delta.Time so that the amount they move doesnt vary with frame rates
    */

    private Transform parent;

    void Update()
    {

        // Checks for inputs,

        // If "W" Is pressed,
        if (Input.GetKey("w"))
        {
            // Applys the translation based on the objects rotation.
            parent.Translate(parent.forward * moveSpeed * Time.deltaTime, Space.World);
        }

        if (Input.GetKey("a"))
        {
            parent.Translate(-parent.right * moveSpeed * Time.deltaTime, Space.World);
        }

        if (Input.GetKey("s"))
        {
            parent.Translate(-parent.forward * moveSpeed * Time.deltaTime, Space.World);
        }

        if (Input.GetKey("d"))
        {
            parent.Translate(parent.right * moveSpeed * Time.deltaTime, Space.World);
        }

        if (Input.GetKey("r"))
        {
            parent.Translate(parent.up * moveSpeed * Time.deltaTime, Space.World);
        }

        if (Input.GetKey("f"))
        {
            parent.Translate(-parent.up * moveSpeed * Time.deltaTime, Space.World);
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
}
