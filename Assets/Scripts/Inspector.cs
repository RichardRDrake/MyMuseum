﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Inspector : MonoBehaviour
{

    public GameObject[] objects;
    public List<float> distances;
    Transform parent;
    public GameObject camera;
    public GameObject UI;

    GameObject inspected;
    GameObject assetPlacer;

    bool toggle = true;
    Vector3 pos, pos2;
    Vector3 apos, apos2;
    CamController controller;
    Asset assetBundle;
    Vector3 oldRot;

    bool UIToggle = false;

    float xRotation, yRotation;

    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent;
        controller = camera.GetComponent<CamController>();
        UI.SetActive(UIToggle);
        assetPlacer = GameObject.Find("AssetPlacer");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp("f")) // On key press,
        {
            // Toggle variable is checked to determine whether we are currently inspecting an object,
            if (toggle == true) // If an object isn't currently being inspected,
            {
                // Call CheckSurround 
                CheckSurround();

            }
            else // If an object is already being inspected,
            {
                // Toggle the display text
                DisplayTextToggle();

                // Set the toggle variable to true again
                toggle = true;

                // Set the camera viewing rotation to be the original value
                camera.transform.eulerAngles = new Vector3(camera.transform.eulerAngles.x + 20, camera.transform.eulerAngles.y, camera.transform.eulerAngles.z);

                // Hides the cursor again
                Cursor.visible = false;

                // Lets the camera controller know we are no longer inspecting, re-enables hotkeys (allowing the user to move again)
                controller.inspection = false;
                controller.canHotkey = true;


                // Reset the objects rotation to its original value
                inspected.transform.eulerAngles = oldRot;

                // Resets the inspected object and camera's position back to their original values, prior to inspection.
                inspected.transform.position = apos;
                parent.transform.position = apos2;

          
            }
        }

        // While toggle is false,
        if (toggle == false)
        {
            // Call MouseControl to allow for mouse rotation/scrolling
            MouseControl();
        }
    }


    void MouseControl()
    {
        // If left click is held,
        if (Input.GetMouseButton(0))
        {
            // Rotate the object based on the mouse movement
            inspected.transform.Rotate((-Input.GetAxis("Mouse Y") * 600 * Time.deltaTime), -(Input.GetAxis("Mouse X") * 600 * Time.deltaTime), 0, Space.World);
        }


        // Scroll wheel zooming controls

        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            // Declares new variable for zomming in
            float position = parent.localPosition.z;

            // Increases zoom position while scrolling forward
            position += 0.1f;

            // Clamps the value so the user can't zoom in forever
            position = Mathf.Clamp(position, 495, 505);

            // Changes the position of the object, based on the zoom value
            parent.localPosition = new Vector3(500, 500, position);
        }

        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
        {
         
            float position = parent.localPosition.z;

            // Decreases zoom position while scrolling backwards
            position -= 0.1f;
            position = Mathf.Clamp(position, 495, 505);
            
            parent.localPosition = new Vector3(500, 500, position);
        }
    }


    void CheckSurround()
    {
        // Populate objects array with all gameObjects with the "object tag"
        objects = GameObject.FindGameObjectsWithTag("object");
       
        // If therea are objects within the array,
        if (objects.Length > 0)
        {
            // Reset distances list
            distances = new List<float>();
        
            // For every object in the array,
            for (int i = 0; i < objects.Length; i++)
            {
                // Get the object/camera positions
              pos = objects[i].transform.position;
              pos2 = parent.transform.position;

              // Determine the total distance between the camera and object, add it to the distances list
              distances.Add(Mathf.Sqrt(Mathf.Pow(pos2.x - pos.x, 2) + Mathf.Pow(pos2.z - pos.z, 2)));
            }

            // Get the minimum distance from the list
            float min = distances.Min();

            // Check if the distance is below 1.5. If it is,
            if (min < 1.5 && min > 0) 
            {
                // Entering inspection mode

                // Sets toggle to false so the next time the player presses the "f" key, they will instead exist inspection mode
                toggle = false;

                // Tells the camea controller that we are currently inspecting
                controller.inspection = true;
                // Disables hotkeys for the camera controller.
                controller.canHotkey = false;

                // Get the index from the list of distances, for the smallest distance value
                int index = distances.IndexOf(min);

                // Save old object and camera positions prior to moving them to a new space.
                apos = objects[index].transform.position;
                apos2 = parent.transform.position;
               

                // Shows the cursor
                Cursor.visible = true;

                // Sets the inspected variable to the object that is closest to the player (the min distance object)
                inspected = objects[index];

                // Call the text toggle function
                DisplayTextToggle();

                // Get the child text components from the UI, for editing the text based on the currently inspected object.
                Text title = UI.GetComponent<Transform>().Find("objectName").GetComponent<Text>();
                Text desc = UI.GetComponent<Transform>().Find("objectDesc").GetComponent<Text>();


                // Gets the specific asset for the currently inspected object, from the grid manager. This is stored within the assetBundle variable
                assetBundle = assetPlacer.GetComponent<GridManager>().GetPlacedObject(inspected);

                // Null check for assetBundle
                if (assetBundle != null)
                {
                    // If assetBundle isn't null,
             
                    // Populate the UI text with the corresponding asset text and content.
                    title.text = assetBundle.Name;
                    desc.text = assetBundle.Content;

                }
                else
                {
                    Debug.Log("Asset not found");
                }

                // Save old rotation of the inspected object
                oldRot = inspected.transform.eulerAngles;

                // Reposition/rotate the camera and inspected object.
                objects[index].transform.position = new Vector3(500, 500, 503);
                camera.transform.eulerAngles = new Vector3(camera.transform.eulerAngles.x - 20, camera.transform.eulerAngles.y, camera.transform.eulerAngles.z);
                parent.transform.rotation = Quaternion.LookRotation(new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0, 1, 0));
                parent.localPosition = new Vector3(500, 500, 500);
            }

        }

    }


    void DisplayTextToggle()
    {
        // UI active toggle, will toggle between true/false each time it's called
        UIToggle = !UIToggle;
        UI.SetActive(UIToggle);
     
    }

}
