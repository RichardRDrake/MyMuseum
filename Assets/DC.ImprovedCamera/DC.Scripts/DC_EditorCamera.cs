using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DC_EditorCamera : MonoBehaviour
{
    [Header("Transforms to Control")]
    [Tooltip("Should be Top most transform")]
    public Transform _FocusTransform;
    [Tooltip("Should be a child of _FocusTransform")]
    public Transform _PivotTransform;
    [Tooltip("Should be a child of _PivotTransform")]
    public Transform _ZoomTransform;

    [Header("Optional")]
    public GameObject _FocusPointRepresentation;

    [Header("Sensitivity settings")]
    [Range(0.1f, 1.5f)] public float _FocusSensitivity = 0.5f;
    [Range(0.1f, 1.0f)] public float _RotationalSensitivity = 0.25f;
    [Range(0.1f, 1.0f)] public float _ZoomSensitivity = 0.5f;

    [Header("Inversion controls (EDIT mode)")]
    public bool _InvertedPanning = false;
    public bool _InvertedRotation = true;
    public bool _InvertedPitch = true;
    public bool _InvertedZoom = false;

    [Header("Control limitations")]
    public Vector2 _MinMaxZoom = new Vector2(-5.0f, -20.0f);
    public Vector2 _WorldLimitsX;
    public Vector2 _WorldLimitsZ;
    public Vector2 _MinMaxPitch = new Vector2(15.0f, 80.0f);

    [Header("Smooth transitioning speed")]
    [Range(5.0f, 25.0f)] public float _SmoothTransformationSpeed = 5.0f;

    [Header("Perspective view settings")]
    [Tooltip("Speed the player moves, in Meters per second")]
    [Range(1.0f, 25.0f)] public float _MovementSpeed = 10.0f;
    [Tooltip("Movement speed multiplier when left shift button held down")]
    [Range(1.0f, 5.0f)] public float _SprintMultiplier = 5.0f;
    [Tooltip("Look speed sensitivity")]
    [Range(200.0f, 500.0f)] public float _LookSensitivity = 400.0f;
    public bool _InvertedLookX = false;
    public bool _InvertedLookY = false;
    [Tooltip("The height of the player (Camera) in perspective view (Meters)")]
    [Range(0.5f, 2.0f)] public float _PlayerHeight = 1.5f;
    [Range(5.0f, 40.0f)] public float _PerspectiveMoveSmoothingSpeed = 10.0f;
    [Range(5.0f, 60.0f)] public float _PerspectiveRotationSmoothingSpeed = 20.0f;

    // These are the targets you are directly controlling, the transformations smoothly go towards these
    private Vector3 m_TargetFocusPosition;
    private Vector3 m_CurrentEulerAngles;
    private Quaternion m_TargetPivotRotation;
    private float m_TargetZoom;

    // Saved Initial targets, To go back to on Return(true)
    private Vector3 m_Initial_TargetFocusPosition;
    private Vector3 m_Initial_CurrentEulerAngles;
    private Quaternion m_Initial_TargetPivotRotation;
    private float m_Initial_TargetZoom;

    // Saved targets, To go back to on Return(false) from being focused on an object
    private Vector3 m_Saved_TargetFocusPosition;
    private Vector3 m_Saved_CurrentEulerAngles;
    private Quaternion m_Saved_TargetPivotRotation;
    private float m_Saved_TargetZoom;

    // For rotation we just calculate mouse offsets in screen space
    private Vector3 m_MouseReference;
    private Vector3 m_MouseOffset;

    // For panning we use the mouse position in world space so that it takes rotation of the camera into account
    private Vector3 m_MouseReferenceWorldSpace;
    private Vector3 m_CurrentMousePositionWorldSpace;

    // Is the camera currently focused on an object
    private bool m_CurrentlyFocused = false;

    private Image img;
    [Header("UI Canvas")]
    public GameObject UI;
    public GameObject UI_BuildMode;
    public GameObject UI_ViewMode;
    public GameObject UI_MainMenu;

    private GameObject[] objects;
    private List<float> distances;
    private Asset inspectedAsset;
    [Header("Model Viewer")]
    public DC_ModelViewer model_viewer;

    private bool toggle = true;

    // The following is public just for testing
    public enum CurrentMode
    {
        EDIT,
        PERSPECTIVE
    }
    public CurrentMode m_CurrentMode = CurrentMode.EDIT;
    // Switching to perspective will lock the camera controls and instead it will swoop down from it's current orientation ddown to perspective view
    public bool _SwitchingToPerspective = false;
    // If the camera is locked, the cursor will appear and mouse/keyboard input for controlling the camera is ignored
    public bool _CameraLocked = false;

    private Vector3 m_PerspectiveViewEulerAngles;
    private Vector3 m_PerspectiveViewPosition;
    private Vector3 m_PerspectiveDirection;
    private Vector3 m_PerspectiveTargetPosition;
    private Quaternion m_PerspectiveTargetRotation;

    private void Awake()
    {
        // By default if no Focus point is set, then this transform is the focus point
        if (!_FocusTransform)
            _FocusTransform = transform;

        // Get the initial Focus point position
        m_TargetFocusPosition = m_Initial_TargetFocusPosition = _FocusTransform.localPosition;

        // By default if no Pivot point is set, then the first child of this transform is the pivot point
        if (!_PivotTransform)
            _PivotTransform = transform.GetChild(0);

        // Get the initial Pivot point rotations
        if (_PivotTransform)
        {
            m_CurrentEulerAngles = m_Initial_CurrentEulerAngles = _PivotTransform.localEulerAngles;
            m_TargetPivotRotation = m_Initial_TargetPivotRotation = _PivotTransform.localRotation;
        }

        // By default if no Zoom transform set and pivot transform has been found, then it is the first child of the pivot transform
        if (!_ZoomTransform && _PivotTransform)
            _ZoomTransform = _PivotTransform.GetChild(0);

        // Get the initial Zoom position in Z
        if (_ZoomTransform)
            m_TargetZoom = m_Initial_TargetZoom = _ZoomTransform.localPosition.z;

        img = UI.GetComponent<Transform>().Find("objectPrompt").GetComponent<Image>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            if(inspectedAsset == null)
            FirstPersonMode();
        }

        if(Input.GetKeyDown("escape"))
        {
            UI_MainMenu.SetActive(!UI_MainMenu.active);
            UI_BuildMode.SetActive(false);
            UI_ViewMode.SetActive(false);
        }

        // Editing controls
        if (m_CurrentMode == CurrentMode.EDIT)
        {
            img.gameObject.SetActive(false);
            if (!UI_MainMenu.activeInHierarchy)
            {
                UI_BuildMode.SetActive(true);
                UI_ViewMode.SetActive(false);
            }

            if (!_SwitchingToPerspective)
            {
                // Make sure cursor is unlocked
                Cursor.lockState = CursorLockMode.None;

                if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
                {
                    // Hide the cursor
                    Cursor.visible = false;

                    // Mouse position in screen space (For Rotations)
                    m_MouseReference = Input.mousePosition;

                    // Mouse position in world space (For Panning)
                    // 20.0f, is the Z distance into the world, just a number I found works best
                    // Could potentially use the Z distance from the surface you clicked to make it relative to your distance from that point
                    // With a sensitivity of 1, it would then feel like your literally grabbing that point and dragging yourself along
                    m_MouseReferenceWorldSpace = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20.0f));

                    // Enable Optional representation of the focus point
                    // EDIT: Dont show representation if focused on an object
                    if (_FocusPointRepresentation && !m_CurrentlyFocused)
                        _FocusPointRepresentation.SetActive(true);
                }

                // Right mouse button rotates the pivot transform target
                if (Input.GetMouseButton(1))
                {
                    // Calculate the mouse offset from the last frame
                    m_MouseOffset = (Input.mousePosition - m_MouseReference);

                    // Rotate pivot in X and Y
                    m_CurrentEulerAngles += new Vector3(m_MouseOffset.y * (_InvertedPitch ? -1 : 1), m_MouseOffset.x * (_InvertedRotation ? -1 : 1), 0.0f) * _RotationalSensitivity;

                    // Clamp X rotation by _MinMaxPitch
                    if (!_SwitchingToPerspective)
                    {
                        if (m_CurrentEulerAngles.x <= _MinMaxPitch.x)
                            m_CurrentEulerAngles.x = _MinMaxPitch.x;
                        else if (m_CurrentEulerAngles.x >= _MinMaxPitch.y)
                            m_CurrentEulerAngles.x = _MinMaxPitch.y;
                    }

                    // Set the target quaternion rotation
                    m_TargetPivotRotation = Quaternion.AngleAxis(m_CurrentEulerAngles.y, Vector3.up) * Quaternion.AngleAxis(m_CurrentEulerAngles.x, Vector3.right);

                    // Save the mouse position for the next frame
                    m_MouseReference = Input.mousePosition;
                }
                // Middle mouse button adjusts the focus transform target
                else if (Input.GetMouseButton(2))
                {
                    // No longer focused on a target object
                    if (_FocusPointRepresentation && !m_CurrentlyFocused)
                        _FocusPointRepresentation.SetActive(true);
                    m_CurrentlyFocused = false;

                    // Get the current mouse position in world space
                    m_CurrentMousePositionWorldSpace = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20.0f));

                    // Calculate the offset from the last frame
                    m_MouseOffset = (m_CurrentMousePositionWorldSpace - m_MouseReferenceWorldSpace);

                    // Set the focus position
                    m_TargetFocusPosition += new Vector3(m_MouseOffset.x, 0.0f, m_MouseOffset.z) * _FocusSensitivity * (_InvertedPanning ? -1 : 1);

                    // Clamp where the focus position can be to within the confines of the room
                    m_TargetFocusPosition = new Vector3(Mathf.Clamp(m_TargetFocusPosition.x, _WorldLimitsX.x, _WorldLimitsX.y), 0.0f, Mathf.Clamp(m_TargetFocusPosition.z, _WorldLimitsZ.x, _WorldLimitsZ.y));

                    // Save the mouse position for the next frame
                    m_MouseReferenceWorldSpace = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20.0f));
                }
                else
                {
                    // Unhide the cursor
                    Cursor.visible = true;

                    // Disable Optional representation of the focus point
                    if (_FocusPointRepresentation)
                        _FocusPointRepresentation.SetActive(false);
                }

                // Scroll wheel zooms in/out
                m_TargetZoom += Input.mouseScrollDelta.y * _ZoomSensitivity * (_InvertedZoom ? -1 : 1);

                // Limit the zooming
                m_TargetZoom = Mathf.Clamp(m_TargetZoom, Mathf.Min(_MinMaxZoom.x, _MinMaxZoom.y), Mathf.Max(_MinMaxZoom.x, _MinMaxZoom.y));
            }
            else
            {
                // Hide the cursor while transitioning and lock it
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            // Smoothly transition towards targets
            // This is a much easier way to smooth out the transformations

            // The top most transform moves everything under it
            _FocusTransform.localPosition = Vector3.Lerp(_FocusTransform.localPosition, m_TargetFocusPosition, Time.deltaTime * _SmoothTransformationSpeed);

            // The pivot is the middle transform, it deals with the rotations whilst fixated on the Focus transform
            _PivotTransform.localRotation = Quaternion.Lerp(_PivotTransform.localRotation, m_TargetPivotRotation, Time.deltaTime * _SmoothTransformationSpeed);

            // The zoom transform can be the actual camera and is the last transform, it changes local Z position to move in/out towards the focus transform
            _ZoomTransform.localPosition = Vector3.Lerp(_ZoomTransform.localPosition, new Vector3(0, 0, m_TargetZoom), Time.deltaTime * _SmoothTransformationSpeed);

            // If still in Edit mode but were switching to perspective mode
            if(_SwitchingToPerspective)
            {
                // And the edit mode camera has reached it's destination
                // RD EDIT: Now using a threshold to make this more likely to happen
                if(Approximately(_FocusTransform.localPosition, m_TargetFocusPosition, 0.01f) &&
                    Approximately(_PivotTransform.localRotation.eulerAngles, m_TargetPivotRotation.eulerAngles, 0.01f) &&
                    Approximately(_ZoomTransform.localPosition.z, m_TargetZoom, 0.01f))
                {
                    // Switch the mode
                    m_CurrentMode = CurrentMode.PERSPECTIVE;
                    if (!UI_MainMenu.activeInHierarchy)
                    {
                        UI_BuildMode.SetActive(false);
                        UI_ViewMode.SetActive(true);
                    }
                    // Use the pivot rotation in X and Y as the starting rotation for free look
                    m_PerspectiveViewEulerAngles.x = _PivotTransform.localRotation.eulerAngles.x;
                    m_PerspectiveViewEulerAngles.y = _PivotTransform.localRotation.eulerAngles.y;

                    // Set the rotation to the focus transform and target rotation
                    _FocusTransform.localRotation = _PivotTransform.localRotation;
                    m_PerspectiveTargetRotation = _PivotTransform.localRotation;

                    // Set the target position to match
                    m_PerspectiveTargetPosition = _FocusTransform.localPosition;

                    // Remove the rotation from the Pivot Transform as this transform is just not used during perspective mode
                    _PivotTransform.localRotation = Quaternion.identity;

                    // Camera not locked by default unless space is pressed
                    _CameraLocked = false;
                }
            }
        }
        // Perspective controls
        else
        {
            m_CurrentMode = CurrentMode.PERSPECTIVE;
            if (!UI_MainMenu.activeInHierarchy)
            {
                UI_BuildMode.SetActive(false);
                UI_ViewMode.SetActive(true);
            }
            bool check = CheckSurround();
            if (check == true)
                img.gameObject.SetActive(true);
            else
            {
                img.gameObject.SetActive(false);
                inspectedAsset = null;
            }

            // If Space pressed, toggle the locking of the camera and change the cursor visibility
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _CameraLocked = !_CameraLocked;

                if (_CameraLocked)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;

                    // ROMAIN: This is where you could enable the Model viewer if you are looking at an object
                    //
                    // Toggle variable is checked to determine whether we are currently inspecting an object,
                    if (toggle == true) // If an object isn't currently being inspected,
                    {
                        // Call CheckSurround 
                        if (check == true)
                        {
                            toggle = false;
                            if(inspectedAsset != null)
                            model_viewer.Activate(inspectedAsset.asset, inspectedAsset.Content);
                        }
                    }


                }
                else
                {
                    toggle = true;
                    model_viewer.Deactivate();
                    inspectedAsset = null;
                }
            }

            if (toggle == false)
            {
                
                img.gameObject.SetActive(false);
            }

            if (!_CameraLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                // Move the camera faster if the shift key is held
                float speed = _MovementSpeed * (Input.GetKey(KeyCode.LeftShift) ? _SprintMultiplier : 1.0f);

                // Adjust the viewing angles based on Mouse
                m_PerspectiveViewEulerAngles.x += Input.GetAxisRaw("Mouse Y") * Time.deltaTime * -_LookSensitivity * (_InvertedLookX ? -1 : 1);
                m_PerspectiveViewEulerAngles.x = Mathf.Clamp(m_PerspectiveViewEulerAngles.x, -80, 80);
                m_PerspectiveViewEulerAngles.y += Input.GetAxisRaw("Mouse X") * Time.deltaTime * _LookSensitivity * (_InvertedLookY ? -1 : 1);

                // Apply that to the perspective rotatioon (Top most transform, the only transform used for Perspective view)
                m_PerspectiveTargetRotation = Quaternion.AngleAxis(m_PerspectiveViewEulerAngles.y, Vector3.up) * Quaternion.AngleAxis(m_PerspectiveViewEulerAngles.x, Vector3.right);

                // Calculate the movement direction based on the direction you are looking in
                m_PerspectiveDirection = _FocusTransform.forward * Input.GetAxisRaw("Vertical") + transform.right * _MovementSpeed * Input.GetAxisRaw("Horizontal");
                m_PerspectiveDirection.y = 0;
                m_PerspectiveDirection.Normalize();
                m_PerspectiveViewPosition = m_PerspectiveDirection * Time.deltaTime * speed;

                // Add this to the perspective target position
                m_PerspectiveTargetPosition += m_PerspectiveViewPosition;

                // Rotate the focus transform
                _FocusTransform.localRotation = Quaternion.Lerp(_FocusTransform.localRotation, m_PerspectiveTargetRotation, Time.deltaTime * _PerspectiveRotationSmoothingSpeed);

                // Move the Focus transfrom in the calculated direction
                _FocusTransform.localPosition = Vector3.Lerp(_FocusTransform.localPosition, m_PerspectiveTargetPosition, Time.deltaTime * _PerspectiveMoveSmoothingSpeed);
            }
        }
    }

    public void FirstPersonMode()
    {
        if (!_SwitchingToPerspective)
        {
            // RD EDIT: Added so that the camera zoom and pitch go back to where you had it in Edit mode
            // But from your new position (Because you may have moved around)
            // However all is saved just incase you decide to exit perspective mode before the camera even made it to perspective mode
            // In which case it will go back to where it was before the transitioning started
            m_Saved_TargetFocusPosition = m_TargetFocusPosition;
            m_Saved_CurrentEulerAngles = m_CurrentEulerAngles;
            m_Saved_TargetPivotRotation = m_TargetPivotRotation;
            m_Saved_TargetZoom = m_TargetZoom;

            // Set the target focus position to where it currently is, plus a little in front of it
            m_TargetFocusPosition = _FocusTransform.localPosition + _PivotTransform.forward;
            // Set the player height
            m_TargetFocusPosition.y = _PlayerHeight;
            // Set the new target rotation (Swooping down from current position)
            m_TargetPivotRotation = Quaternion.AngleAxis(m_CurrentEulerAngles.y, Vector3.up) * Quaternion.AngleAxis(0, Vector3.right);

            // This will actually make it feel like you are moving your head up and down, can bring it closer to 0 (but not 0) for a more traditional perspective view
            m_TargetZoom = -0.5f;

            // Now switching to perspective mode, camera controls locked during transition (Cursor is always visible)
            _SwitchingToPerspective = true;
        }
        else
            Return();
    }

    bool CheckSurround()
    {
        // Populate the GameObject array "objects" with all objects found in the scene with the tag "object"
        objects = GameObject.FindGameObjectsWithTag("object");

        Vector3 pos, pos2;
        float min;
        // If this array has at least 1 object within it,
        if (objects.Length > 0)
        {
            // Reset the distances list
            distances = new List<float>();

            // For every object in the array,
            for (int i = 0; i < objects.Length; i++)
            {
                // Get the object/camera positions
                pos = objects[i].transform.position;
                pos2 = transform.position;

                // Determine the total distance between the camera and object, add it to the distances list
                distances.Add(Mathf.Sqrt(Mathf.Pow(pos2.x - pos.x, 2) + Mathf.Pow(pos2.z - pos.z, 2)));
            }

            // Get the minimum distance from the list
            min = distances.Min();

            // Check if the distance is below 1.5. If it is,
            if (min < 3 && min > 0)
            {
                inspectedAsset = objects[distances.IndexOf(min)].GetComponent<DC_Placeable>().asset;
                return true; // Return true

            }
            else // If it is less than 1.5
            {
                return false; // Return false
            }
        }
        return false;
    }

    /// <summary>
    /// Using an objects bounds the camera will focus on the center of that object
    /// change the zoom so the object fits nicely
    /// and save it's original values to go back to on Return(false)
    /// </summary>
    /// <param name="gameObject">Object to focus on</param>
    public void FocusOnObject(GameObject gameObject)
    {
        // Save current targets, to go back to when done
        if (!m_CurrentlyFocused)
        {
            m_Saved_TargetFocusPosition = m_TargetFocusPosition;
            m_Saved_CurrentEulerAngles = m_CurrentEulerAngles;
            m_Saved_TargetPivotRotation = m_TargetPivotRotation;
            m_Saved_TargetZoom = m_TargetZoom;
        }

        // Calculate the bounds based on the objects current position
        Bounds bounds = gameObject.GetComponentInChildren<Renderer>().bounds;
        foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
            bounds.Encapsulate(renderer.bounds);

        // Set the focus point to the center
        m_TargetFocusPosition = bounds.center;

        // Zoom to fit object
        Vector3 objectSizes = bounds.max - bounds.min;
        float objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
        float cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * Camera.main.fieldOfView);
        float distance = objectSize / cameraView;
        distance += 0.5f * objectSize;

        m_TargetZoom = distance;

        // Rotate to face the object
        // RD: Doesn't work because some objects face in Z some in X :(
        //Vector3 relativePos = gameObject.transform.position - (gameObject.transform.position - gameObject.transform.forward));

        //// the second argument, upwards, defaults to Vector3.up
        //Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);

        //m_TargetPivotRotation = rotation * Quaternion.AngleAxis(m_CurrentEulerAngles.x, Vector3.right);
        //m_CurrentEulerAngles = new Vector3(m_CurrentEulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z);

        // Now focused on a target
        m_CurrentlyFocused = true;
    }

    /// <summary>
    /// Return the camera to either where it wass before focusing on an object
    /// or if initial set, back to where the camera was at the very beginning
    /// </summary>
    public void Return()
    {
        // If you was focused on a target, then go back to where you was
        if (m_CurrentlyFocused)
        {
            m_TargetFocusPosition = m_Initial_TargetFocusPosition;
            m_CurrentEulerAngles = m_Initial_CurrentEulerAngles;
            m_TargetPivotRotation = m_Initial_TargetPivotRotation;
            m_TargetZoom = m_Initial_TargetZoom;
        }
        // Else if you were in perspective mode keep the focus point where you are
        // and go to a position where your pitch and zoom matches where you was before perspective mode
        else if(m_CurrentMode == CurrentMode.PERSPECTIVE)
        {
            // Where you are right now
            m_TargetFocusPosition = _FocusTransform.localPosition; 
            // Remove player height
            m_TargetFocusPosition.y = 0.0f; 
            // Where you are looking right now
            m_CurrentEulerAngles = _FocusTransform.localRotation.eulerAngles; 
            // Saved pitch
            m_CurrentEulerAngles.x = m_Saved_CurrentEulerAngles.x;
            // New rotation based on where you are looking in Y
            m_TargetPivotRotation = Quaternion.AngleAxis(m_CurrentEulerAngles.y, Vector3.up);
            // Set the pivot rotation in Y straight away (Because Focus transform rotation will be zeroed out immedately
            _PivotTransform.localRotation = m_TargetPivotRotation;
            // Now set the target pitch to where you had it before switching to perspective
            m_TargetPivotRotation *= Quaternion.AngleAxis(m_CurrentEulerAngles.x, Vector3.right);
            // Saved Zoom
            m_TargetZoom = m_Saved_TargetZoom; 
        }
        // Otherwise just go back to where you was before focusing
        else
        {
            m_TargetFocusPosition = m_Saved_TargetFocusPosition;
            m_CurrentEulerAngles = m_Saved_CurrentEulerAngles;
            m_TargetPivotRotation = m_Saved_TargetPivotRotation;
            m_TargetZoom = m_Saved_TargetZoom;
        }

        // Go back to edit camera
        m_CurrentMode = CurrentMode.EDIT;
        _SwitchingToPerspective = false;
        _FocusTransform.localRotation = Quaternion.identity;

        m_CurrentlyFocused = false;
    }

    /// <summary>
    /// Rotates the camera's pivot to the nearest next 90 degree angle going clockwise
    /// </summary>
    public void RotateClockwise()
    {
        // Change the target rotation to the nearest 90 degree angle
        // If the value is negative (due to gimbol lock, add 360)
        if (m_CurrentEulerAngles.y < 0)
            m_CurrentEulerAngles.y += 360.0f;

        if (m_CurrentEulerAngles.y < 90)
            SetRotation(90.0f);
        else if (m_CurrentEulerAngles.y < 180)
            SetRotation(180.0f);
        else if (m_CurrentEulerAngles.y < 270)
            SetRotation(270.0f);
        else
            SetRotation(0.0f);
    }

    /// <summary>
    /// Rotates the camera's pivot to the nearest next 90 degree angle going anti-clockwise
    /// </summary>
    public void RotateAnitclockwise()
    {
        // Change the target rotation to the nearest 90 degree angle
        // If the value is negative (due to gimbol lock, add 360)
        if (m_CurrentEulerAngles.y < 0)
            m_CurrentEulerAngles.y += 360.0f;

        if (m_CurrentEulerAngles.y > 270)
            SetRotation(270.0f);
        else if (m_CurrentEulerAngles.y > 180)
            SetRotation(180.0f);
        else if (m_CurrentEulerAngles.y > 90)
            SetRotation(90.0f);
        else if (m_CurrentEulerAngles.y == 0)
            SetRotation(270.0f);
        else
            SetRotation(0.0f);
    }

    /// <summary>
    /// Can be used to set an absolute angle
    /// </summary>
    /// <param name="angle"></param>
    public void SetRotation(float angle)
    {
        m_CurrentEulerAngles.y = angle;
        m_TargetPivotRotation = Quaternion.AngleAxis(m_CurrentEulerAngles.y, Vector3.up) * Quaternion.AngleAxis(m_CurrentEulerAngles.x, Vector3.right);
    }

    public void UiSwitchMode()
    {
        if (!_SwitchingToPerspective)
            _SwitchingToPerspective = true;
        else
            Return();
    }

    /// <summary>
    /// Checks if all elements of a Vector is approximately the same, within a given threshold
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="threshold"></param>
    /// <returns></returns>
    private static bool Approximately(Vector3 a, Vector3 b, float threshold)
    {
        if (Approximately(a.x, b.x, threshold) && Approximately(a.y, b.y, threshold) && Approximately(a.z, b.z, threshold))
            return true;

        return false;
    }

    /// <summary>
    /// Checks if a float is approximately the same as another, within a given threshold
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="threshold"></param>
    /// <returns></returns>
    private static bool Approximately(float a, float b, float threshold)
    {
        if (threshold > 0f)
        {
            return Mathf.Abs(a - b) <= threshold;
        }
        else
        {
            return Mathf.Approximately(a, b);
        }
    }
}
