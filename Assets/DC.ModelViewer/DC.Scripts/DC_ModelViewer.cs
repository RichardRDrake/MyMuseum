using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DC_ModelViewer : MonoBehaviour
{
    #region Public Settings

    [Header("Panel settings")]
    [Tooltip("The main panel's canvas group (Alpha set to fade in/out)")]
    public CanvasGroup _MainPanelCanvasGroup;
    [Tooltip("The background image that blurs the scene view")]
    public RawImage _BackgroundImage;
    [Tooltip("How long to take to fade in [Seconds]")]
    public float _FadeInTime = 0.5f;
    [Tooltip("How long to take to fade out [Seconds]")]
    public float _FadeOutTime = 0.5f;

    [Header("Text assignments")]
    [Tooltip("The Sub header is the name of the object being previewed")]
    public TMPro.TextMeshProUGUI _SubHeader;
    [Tooltip("The description of the artefact")]
    public TMPro.TextMeshProUGUI _Description;

    [Header("3D Model view settings")]
    [Tooltip("The camera used for rendering the raw image (Position is set at the beginning based on model size and can be moved up and down, the field of view is used to zoom in/out)")]
    public Camera _Camera;
    [Tooltip("The camera pivot is used to pitch the camera up/down")]
    public Transform _CameraPivot;
    [Tooltip("The GameObject of the Raw image that displays the 3D content (This is enabled/disabled as it also detects if the user has their mouse pointer onver the viewing area)")]
    public GameObject _3DContentViewGameObject;
    [Tooltip("The 3D Model Anchor used for placing the object (This is what is rotated based on controls)")]
    public Transform _3DModelAnchor;
    [Tooltip("The default camera distance in meters")]
    [Range(0.5f, 2.0f)] public float _DefaultCameraDistance = 1.0f;
    [Tooltip("Rotational sensitivity (Also used for pitch, feels better if they are the same)")]
    [Range(0.1f, 1.0f)] public float _RotationalSensitivity = 0.4f;
    [Tooltip("Height offset sensitivity")]
    [Range(0.005f, 0.05f)] public float _HeightSensitivity = 0.01f;
    [Tooltip("Zoom sensitivity")]
    [Range(1.0f, 10.0f)] public float _ZoomSensitivity = 5.0f;
    [Tooltip("Inverted rotation?")]
    public bool _InvertMouseX = true;
    [Tooltip("Inverted pitch?")]
    public bool _InvertMouseY = true;
    [Tooltip("The layer used to isolate the viewing object")]
    public string _ViewingObjectLayerName = "ViewingObject";

    [Header("Optional")]
    [Tooltip("Optional stand (Scale based on object size)")]
    public Transform _OptionalStand;
    [Tooltip("Optional horizontal scrollbar for rotation")]
    public Scrollbar _HorizontalScrollbar;
    [Tooltip("Optional vertical scrollbar for height offset")]
    public Scrollbar _VerticalScrollbar;

    #endregion

    #region Private Storage Variables

    // Instantiated previewed model (So it can be removed when the menu is closed)
    private GameObject m_PreviewedModelCopy = null;

    // Saved Calculated Object size
    private float m_ObjectSize = 1.0f;

    // Saved Camera starting position
    private Vector3 m_CameraStartingPosition;

    // When true the 3D model anchor can be manipulated
    private bool m_ManipulationEnabled = false;
    // When true the manipulation is set to be disabled once the user has stopped any current manipulation
    private bool m_ManipulationSetToEnd = false;
    // Where the mouse iss at the start of the manipulation and after any movement to get the difference between frames
    private Vector3 m_MouseReference;
    // The calculated difference between last frame mouse position and the current one
    private Vector3 m_MouseOffset;

    // Shader Property ID got at the beginning for much quicker access
    private int m_BlurRadiusShaderID;

    #endregion

    #region Unity Functions

    private void Awake()
    {
        // Convert times into multipliers
        if (_FadeInTime != 0)
            _FadeInTime = 1.0f / _FadeInTime;
        if (_FadeOutTime != 0)
            _FadeOutTime = 1.0f / _FadeOutTime;

        if (_BackgroundImage)
            m_BlurRadiusShaderID = Shader.PropertyToID("_AlphaStrength");
    }    

    private void Update()
    {
        if (m_ManipulationEnabled)
        {
            // If either mouse button is down this frame then make the cursor invisible and store it's position
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                Cursor.visible = false;
                m_MouseReference = Input.mousePosition;
            }

            // Rotate the 3D model anchor in Y axis using mouse left/right difference
            // Pitch camera up/down in X axis using mouse up/down difference
            if (Input.GetMouseButton(0))
            {
                m_MouseOffset = (Input.mousePosition - m_MouseReference);

                _3DModelAnchor.Rotate(0.0f, m_MouseOffset.x * _RotationalSensitivity * (_InvertMouseX? -1:1), 0.0f);

                _CameraPivot.Rotate(m_MouseOffset.y * _RotationalSensitivity * (_InvertMouseY ? -1 : 1), 0.0f, 0.0f);
                // Limit camera pivot
                Vector3 cameraPitch = _CameraPivot.eulerAngles;
                cameraPitch.x = ClampAngle(_CameraPivot.eulerAngles.x, -35.0f, 35.0f);
                _CameraPivot.eulerAngles = cameraPitch;

                m_MouseReference = Input.mousePosition;

            }
            // Move camera up/down in World space (limiting it to +/- half the object size
            else if (Input.GetMouseButton(1))
            {
                m_MouseOffset = (Input.mousePosition - m_MouseReference);

                _CameraPivot.Translate(0.0f, (m_MouseOffset.x + m_MouseOffset.y) * _HeightSensitivity * m_ObjectSize, 0.0f, Space.World);

                // Limit the positioning
                Vector3 tempPos = _CameraPivot.position;
                tempPos.y = Mathf.Clamp(tempPos.y, - m_ObjectSize * 0.5f, m_ObjectSize * 0.5f);
                _CameraPivot.position = tempPos;

                m_MouseReference = Input.mousePosition;
            }
            else
            {
                // Make the cursor visible again
                Cursor.visible = true;

                // If marked to stop manipulation, now's the time to do it
                if (m_ManipulationSetToEnd)
                {
                    m_ManipulationEnabled = false;
                    m_ManipulationSetToEnd = false;
                }
            }

            // Zoom by adjusting the cameras field of view
            _Camera.fieldOfView -= Input.mouseScrollDelta.y * _ZoomSensitivity;

            // Clamp the zoom
            _Camera.fieldOfView = Mathf.Clamp(_Camera.fieldOfView, 15.0f, 60.0f);

            // Optional
            // Update horizontal scrollbar to match current rotation
            //if (_HorizontalScrollbar)
            //    _HorizontalScrollbar.value = 0.5f + (_3DModelAnchor.rotation.eulerAngles.y - 180.0f) / 360.0f;
            //// Update vertical scrollbar to match current height offset
            //if (_VerticalScrollbar)
            //    _VerticalScrollbar.value = 0.5f + (m_CameraStartingPosition.y - _Camera.transform.position.y) / m_ObjectSize;
        }
    }
    #endregion

    #region Async Functions
    /// <summary>
    /// Over time do whatever transitions you want on Activation/Deactivation
    /// </summary>
    /// <param name="fadeIn">Are we fading in or out?</param>
    /// <returns></returns>
    private IEnumerator AnimatedFade(bool fadeIn)
    {
        // If fading in, then enable the raw image that displays the 3D content
        if(fadeIn && _3DContentViewGameObject)
            _3DContentViewGameObject.SetActive(true);

        // Time is 0, so immediate
        if (fadeIn ? _FadeInTime == 0.0f : _FadeOutTime == 0.0f)
        {
            if(_MainPanelCanvasGroup)
                _MainPanelCanvasGroup.alpha = fadeIn ? 1.0f : 0.0f;

            if (_BackgroundImage)
            {
                if (fadeIn)
                {
                    _BackgroundImage.enabled = true;
                    _BackgroundImage.material.SetFloat(m_BlurRadiusShaderID, 1.0f);
                }
                else
                {
                    _BackgroundImage.material.SetFloat(m_BlurRadiusShaderID, 1.0f);
                    _BackgroundImage.enabled = false;
                }
            }
        }
        // Do everything in the time alotted
        else
        {
            if (fadeIn)
            {
                // Enable and blur the background
                if (_BackgroundImage)
                {
                    _BackgroundImage.enabled = true;

                    while (_BackgroundImage.material.GetFloat(m_BlurRadiusShaderID) < 1.0f)
                    {
                        _BackgroundImage.material.SetFloat(m_BlurRadiusShaderID, _BackgroundImage.material.GetFloat(m_BlurRadiusShaderID) + Time.deltaTime * _FadeInTime * 2.0f); // 0 - 1 in half fade in time
                        yield return null;
                    }
                }

                // Then fade in the panel
                if (_MainPanelCanvasGroup)
                {
                    while (_MainPanelCanvasGroup.alpha < 1.0f)
                    {
                        _MainPanelCanvasGroup.alpha += Time.deltaTime * _FadeInTime * 2.0f; // 0 - 1 in half fade in time 
                        yield return null;
                    }
                }
            }
            else
            {
                // Fade out the panel
                if (_MainPanelCanvasGroup)
                {
                    while (_MainPanelCanvasGroup.alpha > 0.0f)
                    {
                        _MainPanelCanvasGroup.alpha += Time.deltaTime * -_FadeOutTime * 2.0f; // 1 - 0 in half fade out time 
                        yield return null;
                    }
                }

                // then Unblur the background and finally disable it again
                if (_BackgroundImage)
                {
                    while (_BackgroundImage.material.GetFloat(m_BlurRadiusShaderID) > 0.0f)
                    {
                        _BackgroundImage.material.SetFloat(m_BlurRadiusShaderID, _BackgroundImage.material.GetFloat(m_BlurRadiusShaderID) + Time.deltaTime * -_FadeOutTime * 2.0f); // 1 - 0 in half fade out time
                        yield return null;
                    }

                    _BackgroundImage.enabled = false;
                }
            }
        }

        // If fading out, then disable the raw image that displays the 3D content
        if (!fadeIn && _3DContentViewGameObject)
            _3DContentViewGameObject.SetActive(false);
    }
    #endregion

    #region Private Helper Functions

    /// <summary>
    /// Clamp any euler angle between two values
    /// </summary>
    /// <param name="angle">Angle to clamp</param>
    /// <param name="from">Minimum angle</param>
    /// <param name="to">Maximum angle</param>
    /// <returns></returns>
    float ClampAngle(float angle, float from, float to)
    {
        // accepts e.g. -80, 80
        if (angle < 0f) angle = 360 + angle;
        if (angle > 180f) return Mathf.Max(angle, 360 + from);
        return Mathf.Min(angle, to);
    }

    #endregion

    #region Main Public Functions
    /// <summary>
    /// Activate the model viewer
    /// </summary>
    /// <param name="model">The model you want to preview</param> 
    /// <param name="description">Descriptive text associated with the model</param>
    /// <param name="initialYRotation">The initial rotation of the model to best show it off by default (Optional)</param>
    public void Activate(GameObject model, string description, float initialYRotation = 0.0f)
    {
        // If for whatever reason there is already a model instantiated (Menu wasn't closed down before being activated again)
        // Destroy that one first
        if (m_PreviewedModelCopy)
            Destroy(m_PreviewedModelCopy);

        // Make a copy of the gameobject and attach to the 3D Model Anchor
        m_PreviewedModelCopy = Instantiate(model, _3DModelAnchor, false);

        // If the object/prefab contains the DC_Placeable script, make sure to disable it in the copy
        DC_Placeable unwantedPlacementScript = m_PreviewedModelCopy.GetComponentInChildren<DC_Placeable>();
        if (unwantedPlacementScript)
            unwantedPlacementScript.enabled = false;

        // Set position to 0,0,0 to "sit" on the optional stand
        m_PreviewedModelCopy.transform.position = Vector3.zero;

        // Set the layer so it can be seen by our special camera
        // Using DC_GameObjectExtensions to easily apply the layer to all children as well as the parent
        m_PreviewedModelCopy.SetLayer(LayerMask.NameToLayer(_ViewingObjectLayerName), true);

        // Set the Sub header to match the name of the object
        // Replace Underscores with spaces for better readability
        _SubHeader.text = model.name.Replace("_", " ");

        // Set it's initial rotation (Some objects look better at certain angles - so make that the default)
        _3DModelAnchor.rotation = Quaternion.AngleAxis(initialYRotation, Vector3.up);

        // Set the descriptive text
        if (_Description)
            _Description.text = description;

        // Calculate the optimum distance for the initial camera by using the gameobjects bounds + a bit of padding
        Bounds bounds = m_PreviewedModelCopy.GetComponentInChildren<Renderer>().bounds;
        foreach (Renderer renderer in m_PreviewedModelCopy.GetComponentsInChildren<Renderer>())
            bounds.Encapsulate(renderer.bounds);
        // Object size in each axis is the difference between the max and min bounds
        Vector3 objectSizes = bounds.max - bounds.min;
        // The overall object size is based on the maximum value
        m_ObjectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
        // Reset the camera FOV to the default 40
        _Camera.fieldOfView = 40.0f;
        // Take the Cameras FOV into account
        float cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * _Camera.fieldOfView);
        // Combined wanted distance from the object
        float distance = _DefaultCameraDistance * m_ObjectSize / cameraView;
        // Estimated offset from the center to the outside of the object
        distance += 0.5f * m_ObjectSize; 
        // Position the camera
        _Camera.transform.position = bounds.center - distance * _Camera.transform.forward;
        // Make sure always 0 in X axis
        _Camera.transform.position = new Vector3(0.0f, _Camera.transform.position.y, _Camera.transform.position.z);
        // Save the starting position for Zooming and Height offsetting
        m_CameraStartingPosition = _Camera.transform.position;

        // Optional stand scaling based on the model size
        if (_OptionalStand)
            _OptionalStand.localScale = new Vector3(m_ObjectSize, m_ObjectSize * 0.2f, m_ObjectSize);

        // Now fade in the model viewer canvas
        StartCoroutine(AnimatedFade(true));
    }

    /// <summary>
    /// Deactivate the model viewer
    /// </summary>
    public void Deactivate()
    {
        // Stop all coroutines as you may have closed it before it had fully transitioned in
        StopAllCoroutines();

        // Destroy the previewed object (Delayed until canvas has faded out)
        if (m_PreviewedModelCopy)
            Destroy(m_PreviewedModelCopy, _FadeOutTime);

        // Cannot be started if GameObject is no longer active (Destroyed/Shutdown)
        if (gameObject.activeSelf)
            StartCoroutine(AnimatedFade(false));
    }

    /// <summary>
    /// Activate mouse manipulations (Use on pointer enter event for when the pointer is over the 3D model viewing area)
    /// </summary>
    public void OnPointerEnter()
    {
        // No longer set to end manipulation
        m_ManipulationSetToEnd = false;

        m_ManipulationEnabled = true;
    }

    /// <summary>
    /// Set the mouse manipulation to deactivate (Doesn't do this immediately as you may still be manipulating the object)
    /// </summary>
    public void OnPointerExit()
    {
        m_ManipulationSetToEnd = true;
    }

    #endregion

    #region Optional UI Setters
    public void UISetRotation(Scrollbar scrollbar)
    {
        //_3DModelAnchor.rotation = Quaternion.AngleAxis(scrollbar.value * 360.0f, Vector3.up);
    }

    public void UISetHeight(Scrollbar scrollbar)
    {
        //_Camera.transform.position = m_CameraStartingPosition - new Vector3(0.0f, (scrollbar.value - 0.5f) * m_ObjectSize, 0.0f);
    }

    public void UIZoomIn()
    {
        _Camera.fieldOfView = Mathf.Clamp(_Camera.fieldOfView - _ZoomSensitivity, 15.0f, 60.0f);
    }
    public void UIZoomOut()
    {
        _Camera.fieldOfView = Mathf.Clamp(_Camera.fieldOfView + _ZoomSensitivity, 15.0f, 60.0f);
    }
    #endregion

    #region Demo Activation
    public void DemoActivate(GameObject model)
    {
        Activate(model, "At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium " +
                "voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident, " +
                "similique sunt in culpa qui officia deserunt mollitia animi, id est laborum et dolorum fuga. Et harum quidem rerum facilis " +
                "est et expedita distinctio. Nam libero tempore, cum soluta nobis est eligendi optio cumque nihil impedit quo minus id " +
                "quod maxime placeat facere possimus, omnis voluptas assumenda est, omnis dolor repellendus. Temporibus autem quibusdam et " +
                "aut officiis debitis aut rerum necessitatibus saepe eveniet ut et voluptates repudiandae sint et molestiae non recusandae. " +
                "Itaque earum rerum hic tenetur a sapiente delectus, ut aut reiciendis voluptatibus maiores alias consequatur aut perferendis " +
                "doloribus asperiores repellat.", 180.0f);
    }
    #endregion
}
