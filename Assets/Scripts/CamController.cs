using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    
    //Top object in each UI hierarchy
    [SerializeField] private GameObject UiBuild;
    [SerializeField] private GameObject UiView;
    private UI_Controller UI_Controller;
    private UI_ViewController UI_ViewController;

    //Main menu object and script
    [SerializeField] private GameObject UiMain;
    private UI_MenuController UI_MenuController;

    bool UiToggle = false;
    bool cameraToggle = false;
   
    //camera movement variables

    private Transform parent;
    private float moveSpeed = 5.0f;
    private float rotation = 90.0f;

    public float horSensitivity = 1.0f;
    public float verSensitivity = 1.0f;

    private float xRotation = 0.0f;
    private float yRotation = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        // By default, the game begins using the third person cam
       
        UI_Controller = UiBuild.GetComponent<UI_Controller>();
        UI_MenuController = UiMain.GetComponent<UI_MenuController>();
        UI_ViewController = UiView.GetComponent<UI_ViewController>();

        parent = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {

        // If "k" is pressed,
        if (Input.GetKeyUp("k"))
        {
            if(cameraToggle == false)
            {
                cameraToggle = true;
            }
            else
            {
                cameraToggle = false;
            }

            // Flips active states on cameras (e.g. if cam1 is active, cam1 is now inactive and cam2 is now active, after pressing "k")
            UI_MenuController = UiMain.GetComponent<UI_MenuController>();
            //  cam2.SetActive(!cam2.activeSelf);
            UI_Controller.ResetBuildUI();
            UiToggle = !UiToggle;
            if(UiToggle == false)
            {
                UiView.SetActive(false);
                UiBuild.SetActive(true);
                UI_MenuController.buildMode = true;
            }
            else
            {
                UiView.SetActive(true);
                UiBuild.SetActive(false);
                UI_MenuController.buildMode = false;
            }
            UiMain.SetActive(false);
        }

        if(cameraToggle == false)
        {
            thirdPersonCameraUpdate();
        }
        else
        {
           firstPersonCameraUpdate();
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
    void firstPersonCameraUpdate()
    {
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

        MouseControl();
    }
    void thirdPersonCameraUpdate()
    {
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
