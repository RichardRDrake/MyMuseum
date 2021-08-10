using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Inversion controls")]
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

    public enum CurrentMode
    {
        EDIT,
        PERSPECTIVE
    }

    public CurrentMode m_CurrentMode = CurrentMode.EDIT;

    public bool _SwitchingToPerspective = false;

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
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            if (!_SwitchingToPerspective)
            {
                m_TargetFocusPosition = _FocusTransform.localPosition + _PivotTransform.forward;
                m_TargetFocusPosition.y = 1.5f;
                m_TargetPivotRotation = Quaternion.AngleAxis(m_CurrentEulerAngles.y, Vector3.up) * Quaternion.AngleAxis(0, Vector3.right);
                
                // This will actually make it feel like you are moving your head up and down, can bring it closer to 0 (but not 0) for a more traditional perspective view
                m_TargetZoom = -0.5f;

                _SwitchingToPerspective = true;
            }
            else
                Return();
        }

        // Editing controls
        if (m_CurrentMode == CurrentMode.EDIT)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
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
                if (_SwitchingToPerspective)
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
            if(!_SwitchingToPerspective)
                m_TargetZoom = Mathf.Clamp(m_TargetZoom, Mathf.Min(_MinMaxZoom.x, _MinMaxZoom.y), Mathf.Max(_MinMaxZoom.x, _MinMaxZoom.y));

            // Smoothly transition towards targets
            // This is a much easier way to smooth out the transformations

            // The top most transsform moves everything under it
            _FocusTransform.localPosition = Vector3.Lerp(_FocusTransform.localPosition, m_TargetFocusPosition, Time.deltaTime * _SmoothTransformationSpeed);

            // The pivot is the middle transform, it deals with the rotations whilst fixated on the Focus transform
            _PivotTransform.localRotation = Quaternion.Lerp(_PivotTransform.localRotation, m_TargetPivotRotation, Time.deltaTime * _SmoothTransformationSpeed);

            // The zoom transform can be the actual camera and is the last transform, it changes local Z position to move in/out towards the focus transform
            _ZoomTransform.localPosition = Vector3.Lerp(_ZoomTransform.localPosition, new Vector3(0, 0, m_TargetZoom), Time.deltaTime * _SmoothTransformationSpeed);

            if(_SwitchingToPerspective)
            {
                if(_FocusTransform.localPosition ==  m_TargetFocusPosition &&
                    _PivotTransform.localRotation == m_TargetPivotRotation &&
                    Mathf.Approximately(_ZoomTransform.localPosition.z, m_TargetZoom))
                {
                    m_CurrentMode = CurrentMode.PERSPECTIVE;


                    freeAngles.y = _PivotTransform.localRotation.eulerAngles.y;
                    _PivotTransform.localRotation = Quaternion.identity;
                    
                }
            }
        }
        // Perspective controls (Not finished)
        else
        {
            //Move the camera faster if the shift key is held
            float moveSpeed = 10.0f;
            float sprintMultiplier = 5.0f;
            float freeSensitivity = 400.0f;
            float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1.0f);

            freeAngles.x += Input.GetAxisRaw("Mouse Y") * Time.deltaTime * -freeSensitivity;
            freeAngles.x = Mathf.Clamp(freeAngles.x, -80, 80);
            freeAngles.y += Input.GetAxisRaw("Mouse X") * Time.deltaTime * freeSensitivity;

            _FocusTransform.localRotation = Quaternion.AngleAxis(freeAngles.y, Vector3.up);
            _FocusTransform.localRotation *= Quaternion.AngleAxis(freeAngles.x, Vector3.right);

            Vector3 moveDirection = _FocusTransform.forward * Input.GetAxisRaw("Vertical") + transform.right * moveSpeed * Input.GetAxisRaw("Horizontal");
            moveDirection.y = 0;
            moveDirection.Normalize();
            freePosition = moveDirection * Time.deltaTime * speed;

            _FocusTransform.localPosition += freePosition;
        }
    }

    private Vector3 freeAngles;
    private Vector3 freePosition;

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
        // Go back to edit camera
        m_CurrentMode = CurrentMode.EDIT;
        _SwitchingToPerspective = false;
        _FocusTransform.localRotation = Quaternion.identity;

        // If not focused on a target, then go back to initial
        if (!m_CurrentlyFocused)
        {
            m_TargetFocusPosition = m_Initial_TargetFocusPosition;
            m_CurrentEulerAngles = m_Initial_CurrentEulerAngles;
            m_TargetPivotRotation = m_Initial_TargetPivotRotation;
            m_TargetZoom = m_Initial_TargetZoom;
        }
        // Otherwise just go back to where you was before focusing
        else
        {
            m_TargetFocusPosition = m_Saved_TargetFocusPosition;
            m_CurrentEulerAngles = m_Saved_CurrentEulerAngles;
            m_TargetPivotRotation = m_Saved_TargetPivotRotation;
            m_TargetZoom = m_Saved_TargetZoom;
        }

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
}
