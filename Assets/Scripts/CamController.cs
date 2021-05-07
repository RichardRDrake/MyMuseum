using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    #region variables
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
    public bool canHotkey = true;
    public bool inspection = false;
    //camera movement variables

    private Transform parent;
    CharacterController cController;
   // Rigidbody rb;
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

    //Determines whether the camera can pan
    private bool canPan = false;

    //headbob variables
    private float movementCounter;
    private float idleCounter;
    private Vector3 headBobMovement;

    //clamp variables
    public GameObject posX;
    public GameObject negX;
    public GameObject posZ;
    public GameObject negZ;

    //zoom variables
    float minFov = 15f;
    float maxFov = 90f;
    float sensitivity = 10f;

    int rotateCounter;

    //camera reference
    private Camera cameraRef;

    //use mudolo
    int mod(int a, int n)
    {
        int result = a % n;
        if ((result < 0 && n > 0) || (result > 0 && n < 0))
        {
            result += n;
        }
        return result;
    }
    public void ZoomIn()
    {
        float fov = cameraRef.fieldOfView;
        fov -= Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        fov = Mathf.Clamp(fov, minFov, maxFov);
        cameraRef.fieldOfView = fov;
    }
    #endregion


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
    private void doubleDisplacement()
    {
        if (Input.GetAxis("Vertical") != 0 && Input.GetAxis("Horizontal") != 0)
        {
            moveSpeed = 2.5f;
        }
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
            Cursor.visible = true;
            UI_MenuController.buildMode = true;
        }
        else
        {
            UiView.SetActive(true);
            UiBuild.SetActive(false);
            Cursor.visible = false;
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
        //Debug.Log(thirdPersonRotation);
        gameObject.transform.localPosition = firstPersonPosition;
        

        cameraToggle = true;
        //Debug.Log(cameraToggle);

        parent.transform.position = resetParent;
        UI_SwitchPerson();
    }
    public void switchToThirdPerson()
    {
       // parent.Rotate(new Vector3(0.0f, 0.0f, 0.0f));
        parent.transform.rotation = Quaternion.LookRotation(startThirdPersonRotation, new Vector3(0, 1, 0));
       
        gameObject.transform.localPosition = thirdPersonPosition;

        cameraToggle = false;
        //Debug.Log(cameraToggle);
        parent.transform.position = resetParent;
        UI_SwitchPerson();
        rotateCounter = 0;
    }
    // Start is called before the first frame update
    void Start()
    {
        // By default, the game begins using the third person cam
        cameraRef = GetComponent<Camera>();
       
        UI_Controller = UiBuild.GetComponent<UI_Controller>();
        UI_MenuController = UiMain.GetComponent<UI_MenuController>();
        UI_ViewController = UiView.GetComponent<UI_ViewController>();

        parent = transform.parent;
        //Debug.Log(parent);
        firstPersonPosition.x = 0.0f;
        firstPersonPosition.y = 2.1f;
        firstPersonPosition.z = 0.0f;
        gameObject.transform.position = new Vector3(0.0f, 25.0f, -25.0f);
        thirdPersonPosition.x = 0.0f;
        thirdPersonPosition.y = 25.0f;
        thirdPersonPosition.z = -25.0f;

       //rb = parent.GetComponent<Rigidbody>();
        cController = parent.GetComponent<CharacterController>();

    }

    // Update is called once per frame
    void Update()
    {
        
        // If "k" is pressed,
        if (Input.GetKeyUp("k") && canHotkey)
        {
                if (cameraToggle == false)  //switch to first person mode
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
            if (inspection == false)
            {
                firstPersonCameraUpdate(); // first person mode
            }
        }

    }

   
    void MouseControl()
    {
        float mouseX = Input.GetAxis("Mouse X") * horSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * verSensitivity;

        if (canHotkey)
        {
            yRotation += mouseX;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -60, 60);

            parent.eulerAngles = new Vector3(xRotation, yRotation, 0.0f);
        }

    }
    void firstPersonCameraUpdate()
    {
       
        //if (Input.GetAxis("Vertical") == 0 && Input.GetAxis("Horizontal") == 0)
        //{
        //    HeadBob(idleCounter, 0.025f, 0.025f);
        //    idleCounter += Time.deltaTime;
        //    parent.localPosition = movementPosition + headBobMovement;
        //}
        //else 
        //{
        //    HeadBob(movementCounter, 0.05f, 0.05f);
        //    movementCounter += Time.deltaTime;
           
            if (Input.GetKey("w") && canHotkey)
            {
          
            //  doubleDisplacement();
            // Sets y value to 0 so the player won't move based on tilt.
            var forward = parent.forward;
                forward.y = 0;

                // Applys the translation based on the objects rotation.
                //parent.Translate((forward + headBobMovement) * moveSpeed * Time.deltaTime, Space.World);
                // rb.MovePosition(parent.transform.position + (parent.transform.forward * moveSpeed * Time.deltaTime));
             cController.Move(forward * moveSpeed * Time.deltaTime);
            }

            if (Input.GetKey("a") && canHotkey)
            {
                // doubleDisplacement();
                //  rb.MovePosition(parent.transform.position + (-parent.transform.right * moveSpeed * Time.deltaTime));
                //   parent.Translate((-transform.right + headBobMovement) * moveSpeed * Time.deltaTime, Space.World);
                cController.Move( -parent.right * moveSpeed * Time.deltaTime);
            }

            if (Input.GetKey("s") && canHotkey)
            {
              //  doubleDisplacement();
                var forward = parent.forward;
                forward.y = 0;
                //  parent.Translate((-forward + headBobMovement) * moveSpeed * Time.deltaTime, Space.World);
                //   rb.MovePosition(parent.transform.position + (-parent.transform.forward * moveSpeed * Time.deltaTime));
                cController.Move( -forward * moveSpeed * Time.deltaTime);
            }

            if (Input.GetKey("d") && canHotkey)
            {
                //doubleDisplacement();

                //parent.Translate((transform.right + headBobMovement) * moveSpeed * Time.deltaTime, Space.World);
                //   rb.MovePosition(parent.transform.position + (parent.transform.right * moveSpeed * Time.deltaTime));
              
                cController.Move(parent.right * moveSpeed * Time.deltaTime);
            }
            movementPosition = parent.localPosition;
          
        //}

        MouseControl();

        //headBob

        firstPersonPosition.x = gameObject.transform.localPosition.x;
        firstPersonPosition.y = 2.1f;
        firstPersonPosition.z = gameObject.transform.localPosition.z;

        
    }
    void thirdPersonCameraUpdate()
    {
        ZoomIn();
        if (Input.GetKeyDown("q") && canHotkey)
        {
            // Applys a rotation of 90 degrees to the object
            rotateCounter++;
            rotateLeft();
        }

        if (Input.GetKeyDown("e") && canHotkey)
        {
            rotateCounter--;
            rotateRight();
        }

        if (Input.GetMouseButtonDown(2) && canPan == false)
        {
            canPan = true;
        }

        if (Input.GetMouseButtonUp(2))
        {
            canPan = false;
        }

        //hard wired clamp values, not efficient but effective
        if (Mathf.Abs(mod(rotateCounter, 4)) == 0)
        {
            //Debug.Log("Rotation 1");
            //Debug.Log(rotateCounter);
            //we are  at rotation 1

            if (Input.GetKey("a") && canHotkey)
            {
                if (parent.transform.position.x > negZ.transform.position.x)
                {
                    parent.Translate((-transform.right) * 4 * Time.deltaTime, Space.World);
                }
            }
            if (Input.GetKey("d") && canHotkey)
            {
               
                if (parent.transform.position.x < posZ.transform.position.x)
                {
                    parent.Translate((transform.right) * 4 * Time.deltaTime, Space.World);
                }
            }
            if (Input.GetKey("w") && canHotkey)
            {
                var forward = parent.forward;
                forward.y = 0;
                if (parent.transform.position.z < negX.transform.position.z + 5)
                {
                    parent.Translate(forward * 4 * Time.deltaTime, Space.World);
                }
            }
            if (Input.GetKey("s") && canHotkey)
            {
                var forward = parent.forward;
                forward.y = 0;
                if (parent.transform.position.z > posX.transform.position.z + 10)
                {
                    parent.Translate(-forward * 4 * Time.deltaTime, Space.World);
                }
            }
        } // rotation 1

        if (Mathf.Abs(mod(rotateCounter, 4)) == 1) 
        {
            //Debug.Log("Rotation 2");
            //Debug.Log(rotateCounter);
            //we are  at rotation 2
            if (Input.GetKey("a") && canHotkey)
            {
                if (parent.transform.position.z > posX.transform.position.z)
                {
                    parent.Translate((-transform.right) * 4 * Time.deltaTime, Space.World);
                }
            }
            if (Input.GetKey("d") && canHotkey)
            { 
                if (parent.transform.position.z < negX.transform.position.z)
                {
                    parent.Translate((transform.right) * 4 * Time.deltaTime, Space.World);
                }
            }
            if (Input.GetKey("w") && canHotkey)
            {
                var forward = parent.forward;
                forward.y = 0;
                if (parent.transform.position.x > negZ.transform.position.x )
                {
                    parent.Translate(forward * 4 * Time.deltaTime, Space.World);
                }
            }
            if (Input.GetKey("s") && canHotkey)
            {
                var forward = parent.forward;
                forward.y = 0;
                if (parent.transform.position.x < posZ.transform.position.x )
                {
                    parent.Translate(-forward * 4 * Time.deltaTime, Space.World);
                }
            }


        } // rotation 2

        else if (Mathf.Abs(mod(rotateCounter, 4)) == 2)
        {
            //Debug.Log("Rotation 3");
            //Debug.Log(rotateCounter);
            //we are  at rotation 3
            if (Input.GetKey("a") && canHotkey)
            {
                if (parent.transform.position.x < posZ.transform.position.x)
                {
                    parent.Translate((-transform.right) * 4 * Time.deltaTime, Space.World);
                }
            }
            if (Input.GetKey("d") && canHotkey)
            {
                if (parent.transform.position.x > negZ.transform.position.x)
                {
                    parent.Translate((transform.right) * 4 * Time.deltaTime, Space.World);
                }
            }
            if (Input.GetKey("w") && canHotkey)
            {
                var forward = parent.forward;
                forward.y = 0;
                if (parent.transform.position.z > posX.transform.position.z + 5)
                {
                    parent.Translate(forward * 4 * Time.deltaTime, Space.World);
                }
            }
            if (Input.GetKey("s") && canHotkey)
            {
                var forward = parent.forward;
                forward.y = 0;
                if (parent.transform.position.z < negX.transform.position.z + 10)
                {
                    parent.Translate(-forward * 4 * Time.deltaTime, Space.World);
                }
            }
        } // rotation 3

       else if (Mathf.Abs(mod(rotateCounter,4)) == 3)
        {
            //Debug.Log("Rotation 4");
            //Debug.Log(rotateCounter);
            //we are  at rotation 4
            if (Input.GetKey("a") && canHotkey)
            {
                if (parent.transform.position.z < negX.transform.position.z)
                {
                    parent.Translate((-transform.right) * 4 * Time.deltaTime, Space.World);
                }
            }
            if (Input.GetKey("d") && canHotkey)
            {
                if (parent.transform.position.z > posX.transform.position.z)
                {
                    parent.Translate((transform.right) * 4 * Time.deltaTime, Space.World);
                }
            }
            if (Input.GetKey("w") && canHotkey)
            {
                var forward = parent.forward;
                forward.y = 0;
                if (parent.transform.position.x < posZ.transform.position.x)
                {
                    parent.Translate(forward * 4 * Time.deltaTime, Space.World);
                }
            }
            if (Input.GetKey("s") && canHotkey)
            {
                var forward = parent.forward;
                forward.y = 0;
                if (parent.transform.position.x > negZ.transform.position.x)
                {
                    parent.Translate(-forward * 4 * Time.deltaTime, Space.World);
                }
            }
        } // rotation 4
        thirdPersonPosition.x = 0.0f;
        //Debug.Log(parent.transform.position.x);
        //Debug.Log(parent.transform.position.y);
        thirdPersonPosition.y = 22.0f;
        thirdPersonPosition.z = -25.0f;
    }
}
