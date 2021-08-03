using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// This is a demo script:
/// Displays basic functions on object selected for edit
/// and then feeds back the information to the placeable currently being edited
/// </summary>
public class DC_EditObject : MonoBehaviour
{
    [Header("Canvas settings")]
    [Tooltip("Canvas to enable/disable Edit GUI")]
    public Canvas _Canvas;
    [Tooltip("Positioner transform to set location and scale based on the selected object")]
    public RectTransform _Positioner;

    [Header("Scaling settings")]
    public Vector2 _MinRectSize = new Vector2(32.0f, 32.0f);
    public Vector2 _MaxRectSize = new Vector2(128.0f, 128.0f);

    // This is the current placeable object that this editor is editing
    private DC_Placeable m_CurrentPlaceableObject = null;

    // Check to see if we're about to be destroyed.
    private static bool m_ShuttingDown = false;
    private static object m_Lock = new object();
    private static DC_EditObject m_Instance;

    private bool m_EnabledThisFrame = false;

    /// <summary>
    /// Access singleton instance through this propriety.
    /// Only one object should be editable at any one time, so just have this one object in the scene
    /// </summary>
    public static DC_EditObject Instance
    {
        get
        {
            if (m_ShuttingDown)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(DC_EditObject) +
                    "' already destroyed. Returning null.");
                return null;
            }

            lock (m_Lock)
            {
                if (m_Instance == null)
                {
                    // Search for existing instance.
                    m_Instance = (DC_EditObject)FindObjectOfType(typeof(DC_EditObject));
                }

                return m_Instance;
            }
        }
    }

    public void Init(Bounds encapsulatedBounds, DC_Placeable placeableObjectToEdit)
    {
        // Enable the Edit HUD
        _Canvas.enabled = true;

        // Calculate position and scale of object in screen space
        Rect rect = GUIRectWithObject(encapsulatedBounds, _MinRectSize, _MaxRectSize);

        // Scale to move the buttons out around the selected object
        _Positioner.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.width);
        _Positioner.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.height);

        // Set the position in screen space
        _Positioner.position = rect.center;

        // Make the inputted object the object buttons edit (Send feedback to)
        m_CurrentPlaceableObject = placeableObjectToEdit;

        // Stops the disabling of the HUD from working on this frame, otherwise it would immediately get rid of it
        m_EnabledThisFrame = true;
    }

    /// <summary>
    /// Call via UI button or key press
    /// The object to edit is asssigned during Init
    /// Calls rotate on the object by deltaAngle degrees
    /// </summary>
    /// <param name="deltaAngle"></param>
    public void OnRotateObject(float deltaAngle)
    {
        if(m_CurrentPlaceableObject)
        {
            m_CurrentPlaceableObject.Rotate(deltaAngle);
        }
    }

    /// <summary>
    /// Call via UI button or key press
    /// The object to edit is asssigned during Init
    /// Calls DestroyObject on the object
    /// </summary>
    public void OnDeleteObject()
    {
        if (m_CurrentPlaceableObject)
        {
            m_CurrentPlaceableObject.DestroyObject();

            // Disable the Edit menu
            _Canvas.enabled = false;
        }
    }

    /// <summary>
    /// Call via UI button or key press
    /// The object to edit is asssigned during Init
    /// Calls Reposition on the object, it then acts like it does when being initially placed
    /// </summary>
    public void OnRepositionObject()
    {
        if (m_CurrentPlaceableObject)
        {
            m_CurrentPlaceableObject.Reposition();

            // Disable the Edit menu
            _Canvas.enabled = false;
        }
    }

    /// <summary>
    /// Sets the position and scale of the Edit HUD
    /// </summary>
    /// <param name="encapsulatedBounds"></param>
    /// <param name="minScale"></param>
    /// <param name="maxScale"></param>
    /// <returns></returns>
    private static Rect GUIRectWithObject(Bounds encapsulatedBounds, Vector2 minScale, Vector2 maxScale)
    {
        Vector3 cen = encapsulatedBounds.center;
        Vector3 ext = encapsulatedBounds.extents;

        Vector2[] extentPoints = new Vector2[8]
         {
               Camera.main.WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z-ext.z)),
               Camera.main.WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z-ext.z)),
               Camera.main.WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z+ext.z)),
               Camera.main.WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z+ext.z)),
               Camera.main.WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z-ext.z)),
               Camera.main.WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z-ext.z)),
               Camera.main.WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z+ext.z)),
               Camera.main.WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z+ext.z))
         };

        Vector2 min = extentPoints[0];
        Vector2 max = extentPoints[0];

        foreach (Vector2 v in extentPoints)
        {
            min = Vector2.Min(min, v);
            max = Vector2.Max(max, v);
        }

        Vector2 scale = new Vector2(Mathf.Clamp(max.x - min.x, minScale.x, maxScale.x), Mathf.Clamp(max.x - min.x, minScale.x, maxScale.x));// Mathf.Clamp( max.y - min.y, minScale.y, maxScale.y ) );
        Vector2 center = Camera.main.WorldToScreenPoint(cen);
        center = new Vector2(center.x - (scale.x * 0.5f), center.y - (scale.y * 0.5f));

        return new Rect(center, scale);
    }

    private void OnDestroy()
    {
        m_ShuttingDown = true;
    }

    /// <summary>
    /// During update check if the user has presssed down anywhere other than a button
    /// Disable the canvas if it wasn't just enabled by clicking a placeable object
    /// Note any other buttons will also have to manually do this action
    /// </summary>
    private void Update()
    {
        // If user has pressed down the left or right mouse button, check to see if they pressed a button.
        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && !m_EnabledThisFrame)
        {
            // If they didnt then hide this edit menu
            if (!EventSystem.current.IsPointerOverGameObject())
                _Canvas.enabled = false;
        }
        else
            m_EnabledThisFrame = false;
    }
}
