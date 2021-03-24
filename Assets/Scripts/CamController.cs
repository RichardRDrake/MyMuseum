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


    private Vector3 firstPersonPosition;
    private Vector3 movementPosition;
    private Vector3 thirdPersonPosition;
    private Vector3 thirdPersonRotation;
    private Vector3 startFirstPersonRotation = new Vector3(0.0f,0.0f,1.0f);
    private Vector3 startThirdPersonRotation = new Vector3(0.0f,-0.3f,1f).normalized; 
    private Vector3 resetParent = new Vector3(0.0f, 0.0f, 0.0f);
    public float horSensitivity = 1.0f;
    public float verSensitivity = 1.0f;

    private float xRotation = 0.0f;
    private float yRotation = 0.0f;

    //headbob variables
    private float movementCounter;
    private float idleCounter;
    private Vector3 headBobMovement;
    private void idleHeadBobber()
    {
            HeadBob(idleCounter, 0.025f, 0.025f);
            idleCounter += Time.deltaTime;   
    }
    private void movementHeadBobber()
    {
            HeadBob(movementCounter, 0.05f, 0.05f);
            movementCounter += Time.deltaTime;
    }

    private void UI_SwitchPerson()
    {

        // Flips active states on cameras (e.g. if cam1 is active, cam1 is now inactive and cam2 is now active, after pressing "k")
        UI_MenuController = UiMain.GetComponent<UI_MenuController>();
        //  cam2.SetActive(!cam2.activeSelf);
        UI_Controller.ResetBuildUI();
        UiToggle = !UiToggle;
        if (UiToggle == false)
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

    //Special movement functions
    private void HeadBob(float z, float xIntensity, float yIntensity)
    {
        //this is a modular function that simulates the player's body movements
        headBobMovement = new Vector3(Mathf.Cos(z) * xIntensity, Mathf.Sin(z * 4f) * yIntensity, 0.0f);

    }
    //button functions
    public void rotateLeft()
    {
        parent.Rotate(-Vector3.up * rotation, Space.World);
    }
    public void rotateRight()
    {
        parent.Rotate(Vector3.up * rotation, Space.World);
    }
    public void switchToFirstPerson()
    {
        parent.transform.rotation = Quaternion.LookRotation(startFirstPersonRotation, new Vector3(0, 1, 0));

        // parent.Rotate(new Vector3(0.0f, 0.0f, 0.0f));
        thirdPersonRotation = gameObject.transform.forward; //saves the forward-facing direction of the rotation
        Debug.Log(thirdPersonRotation);
        gameObject.transform.localPosition = firstPersonPosition;
        

        cameraToggle = true;
        Debug.Log(cameraToggle);

        parent.transform.position = resetParent;
        UI_SwitchPerson();
    }
    public void switchToThirdPerson()
    {
       // parent.Rotate(new Vector3(0.0f, 0.0f, 0.0f));
        parent.transform.rotation = Quaternion.LookRotation(startThirdPersonRotation, new Vector3(0, 1, 0));
       
        gameObject.transform.localPosition = thirdPersonPosition;

        cameraToggle = false;
        Debug.Log(cameraToggle);
        parent.transform.position = resetParent;
        UI_SwitchPerson();
    }
    // Start is called before the first frame update
    void Start()
    {
        // By default, the game begins using the third person cam
       
        UI_Controller = UiBuild.GetComponent<UI_Controller>();
        UI_MenuController = UiMain.GetComponent<UI_MenuController>();
        UI_ViewController = UiView.GetComponent<UI_ViewController>();

        parent = transform.parent;
        Debug.Log(parent);
        firstPersonPosition.x = 0.0f;
        firstPersonPosition.y = 1.1f;
        firstPersonPosition.z = 0.0f;
        gameObject.transform.position = new Vector3(0.0f, 10.0f, -15.0f);
        thirdPersonPosition.x = 0.0f;
        thirdPersonPosition.y = 10.0f;
        thirdPersonPosition.z = -15.0f;
      
    }

    // Update is called once per frame
    void Update()
    {
        
        // If "k" is pressed,
        if (Input.GetKeyUp("k"))
        {
          
            if(cameraToggle == false)  //switch to first person mode
            {

                switchToFirstPerson();
                
            }
            else // switch to third person mode
            {

                switchToThirdPerson();
            }
           
        }

        if (cameraToggle == false)
        {
           
            thirdPersonCameraUpdate(); //third person mode
        }
        else
        {
           
            firstPersonCameraUpdate(); // first person mode
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
        if (Input.GetAxis("Vertical") == 0 && Input.GetAxis("Horizontal") == 0)
        {
            HeadBob(idleCounter, 0.025f, 0.025f);
            idleCounter += Time.deltaTime;
            parent.localPosition = movementPosition + headBobMovement;
        }
        else
        {
            HeadBob(movementCounter, 0.05f, 0.05f);
            movementCounter += Time.deltaTime;
            if (Input.GetKey("w"))
            {
                // Sets y value to 0 so the player won't move based on tilt.
                var forward = parent.forward;
                forward.y = 0;
                
                // Applys the translation based on the objects rotation.
                parent.Translate((forward + headBobMovement) * moveSpeed * Time.deltaTime, Space.World);
            }

            if (Input.GetKey("a"))
            {
                parent.Translate((-transform.right + headBobMovement) * moveSpeed * Time.deltaTime, Space.World);
            }

            if (Input.GetKey("s"))
            {
                var forward = parent.forward;
                forward.y = 0;
                parent.Translate((-forward + headBobMovement) * moveSpeed * Time.deltaTime, Space.World);
            }

            if (Input.GetKey("d"))
            {
                
                parent.Translate((transform.right + headBobMovement) * moveSpeed * Time.deltaTime, Space.World);
            }
            movementPosition = parent.localPosition;
          
        }

        MouseControl();

        //headBob

        firstPersonPosition.x = gameObject.transform.localPosition.x;
        firstPersonPosition.y = 1.1f;
        firstPersonPosition.z = gameObject.transform.localPosition.z;

        
    }
    void thirdPersonCameraUpdate()
    {
        if (Input.GetKeyDown("q"))
        {
            // Applys a rotation of 90 degrees to the object
            rotateLeft();
        }

        if (Input.GetKeyDown("e"))
        {
            rotateRight();
        }
        thirdPersonPosition.x = 0.0f;
        //Debug.Log(parent.transform.position.x);
        //Debug.Log(parent.transform.position.y);
        thirdPersonPosition.y = 10.0f;
        thirdPersonPosition.z = -15.0f;
    }
}
