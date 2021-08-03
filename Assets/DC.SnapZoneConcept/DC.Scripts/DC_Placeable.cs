using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This is a demo script:
/// Object follows mouse pointer
/// </summary>
public class DC_Placeable : MonoBehaviour
{
    [Header("Snap zone settings")]
    [Tooltip("The valid layers for this object, choose wall layers for objects that should only be placed on walls")]
    public LayerMask _ValidLayers;

    // The bounds are used to calculate how far an object needs adjusting for directional snap zones
    // It is also used to position the Edit Object GUI
    public Bounds m_EncapsulatedBounds;
    // This boolean determines whether the object is rotated to 90 degrees in Y or not based on which bound size is thinnest
    private bool m_DefaultToX = false;
    public float pushOutAmount = 0;
    private float m_CurrentLocalAngle = 0.0f;

    // The snap zone this object is currently placed on (if any)
    private DC_SnapZone m_SnapZone = null;

    private bool m_BeingPlaced = true;
    private bool m_BeingEdited = false;

    private void Awake()
    {
        // Encapsulate all renderers bounds to get the size of the object
        m_EncapsulatedBounds = GetComponentInChildren<Renderer>().bounds;
        foreach(Renderer renderer in GetComponentsInChildren<Renderer>())
            m_EncapsulatedBounds.Encapsulate(renderer.bounds);

        // If the object is thinner in X than it is in Z (Forward direction for directional snap zones)
        // Then the object will be rotated 90 degrees in Y axis by default when snapped to a directional zone
        if (m_EncapsulatedBounds.extents.x <= m_EncapsulatedBounds.extents.z)
        {
            m_DefaultToX = true;
            m_CurrentLocalAngle = 90.0f;
        }
    }

    // For optimisation
    Ray m_Ray;
    RaycastHit[] m_Hits;

    private void Update()
    {
        // Place object
        if (Input.GetMouseButtonDown(0))
        {
            m_BeingPlaced = false;

            // If snapped to a zone, make it invalid for any future placements (This is reset if the object is subsequently moved)
            if (m_SnapZone)
                m_SnapZone.SetValidity(false);
        }

        if (m_BeingPlaced)
        {
            m_Ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Get all the hits, and order them by distance
            m_Hits = Physics.RaycastAll(m_Ray, 100, _ValidLayers, QueryTriggerInteraction.Collide);// EDIT: no need to order, instead using if it has a Sphere Collider to take precedence.OrderBy(x => x.distance).ToArray();

            // Check all hit points
            foreach (RaycastHit hit in m_Hits)
            {
                // If the hit point had a sphere collider then it takes priority
                if (hit.collider is SphereCollider)
                {
                    // This is probably a snap zone, save for later
                    m_SnapZone = hit.transform.GetComponent<DC_SnapZone>();

                    // If it is a snap zone make sure it's still valid (Hasn't already got something placed there)
                    if (m_SnapZone && m_SnapZone._IsValid)
                    {
                        // If the name contains the word "Directional" Set the objects rotation and use directional offset
                        if (hit.transform.name.Contains("Directional"))
                        {
                            // Set the rotation to the snap zone + If X smaller than Z, rotate 90 degrees
                            transform.rotation = hit.transform.rotation * Quaternion.AngleAxis(m_DefaultToX ? 90.0f : 0.0f, Vector3.up);

                            // Set the position to the snap zone + If X smaller push out in the direction of the snap zone by X extents, otherwise Z is used
                            transform.position = hit.transform.position + hit.transform.forward * (m_DefaultToX ? m_EncapsulatedBounds.extents.x : m_EncapsulatedBounds.extents.z);

                            //Testing
                            pushOutAmount = (m_DefaultToX ? m_EncapsulatedBounds.extents.x : m_EncapsulatedBounds.extents.z);
                            break;
                        }
                        // Otherwise just set the objects position (Normal floor snap point where direction doesn't matter)
                        else
                        {
                            transform.position = hit.transform.position;

                            break;
                        }
                    }
                }
                // Otherwise just move to where you are pointing on a valid surface
                else
                {
                    transform.position = hit.point;
                }
            }
        }
    }

    private void OnMouseDown()
    {
        if (!m_BeingPlaced)
        {
            // Create a temporary encapsulated bounds (Not to be confused with the one created at the start)
            // This one is used to calculate it's current position and size on the screen for the GUI placement
            // Updating the old one would mean the encapsulated bound would no longer be local to the object
            // Update the encapsulated bounds to where the object is now
            Bounds tempBound = GetComponentInChildren<Renderer>().bounds;
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
                tempBound.Encapsulate(renderer.bounds);

            // Set the position and scale for the Edit Object toolbox
            DC_EditObject.Instance.Init(tempBound, this);

            m_BeingEdited = true;
        }
    }

    /// <summary>
    /// Rotates object and if snapped to a directional point, changes how far away from the snap zone origin
    /// using the encapsulated bounds
    /// </summary>
    /// <param name="angle"></param>
    public void Rotate(float angle)
    {
        transform.rotation *= Quaternion.AngleAxis(angle, transform.up);

        m_CurrentLocalAngle += angle;

        // If currently snapped to a directional snap zone, switch which bounding box size to use for moving the object away from the snap point
        if (m_SnapZone && m_SnapZone._Directional)
        {
            // Because some objects are thinner in X than they are in Z, when the object was created we worked out which is thinnest
            // So here we know if 90 and 270 is classed as X or Z
            // EDIT: The push out amount is now caculated using the current angle and is a percentage of X and Z so angles don't have to be in 90 degree increments :)
            transform.position = m_SnapZone.transform.position + m_SnapZone.transform.forward * CalculatePushOutAmount(m_CurrentLocalAngle);
        }
    }

    public void Reposition()
    {
        m_BeingPlaced = true;

        // If was attached to a snap zone then make it a valid option again
        if (m_SnapZone)
        {
            m_SnapZone.SetValidity(true);
            m_SnapZone = null;
        }
    }

    public void DestroyObject()
    {
        // If was attached to a snap zone then make it a valid option again
        if (m_SnapZone)
        {
            m_SnapZone.SetValidity(true);
            m_SnapZone = null;
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Based on the inputted angle, calculate the push out amount for directional snap zones
    /// Fully accurate for 0, 90, 180 and 270 etc
    /// Semi accurate for and angle in between, rather than doing expensive ellipsoid calculations)
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    private float CalculatePushOutAmount(float angle)
    {
        // If Defaulted to X then we know that 90 degrees is X extents (Use Sin())
        if (m_DefaultToX)
        {
            float percentage = Mathf.Sin(angle * Mathf.Deg2Rad);

            pushOutAmount = Mathf.Lerp(m_EncapsulatedBounds.extents.z, m_EncapsulatedBounds.extents.x, Mathf.Abs(percentage));

            // If the angle isn't divisible by 90, then add a little extra (To avoid clipping as much as possible without calculating expensive ellipsoids)
            if (angle % 90 != 0)
                pushOutAmount += new Vector2(m_EncapsulatedBounds.extents.x, m_EncapsulatedBounds.extents.z).magnitude * 0.5f;

            return pushOutAmount;
        }
        // Otherwise 0 degrees is X extents (Use Cos())
        else
        {
            float percentage = Mathf.Cos(angle * Mathf.Deg2Rad);

            pushOutAmount = Mathf.Lerp(m_EncapsulatedBounds.extents.x, m_EncapsulatedBounds.extents.z, Mathf.Abs(percentage));

            // If the angle isn't divisible by 90, then add a little extra
            // To avoid clipping as much as possible without calculating expensive ellipsoids or a load of pythagoras
            // If wanting it completely accurate: Maybe this will work...
            // 1. Use collision boxes (As these are rotated with the object, Bounds are not)
            // 2. Find the closest point (locally with regard to the snap point) and push out by any negative distance in Z
            if (angle % 90 != 0)
                pushOutAmount += new Vector2(m_EncapsulatedBounds.extents.x, m_EncapsulatedBounds.extents.z).magnitude * 0.5f;

            return pushOutAmount;
        }
    }
}
