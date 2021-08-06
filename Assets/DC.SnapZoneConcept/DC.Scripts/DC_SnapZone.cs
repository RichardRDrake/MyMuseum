using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a demo script of a Snap zone as a proof of concept
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public class DC_SnapZone : MonoBehaviour
{
    [Header("Sprite settings")]
    public SpriteRenderer _SpriteRenderer;
    public Color _ValidColour;
    public Color _InvalidColour;

    [HideInInspector] public bool _IsValid = true;
    [HideInInspector] public bool _Directional = false;

    private SphereCollider m_SphereCollider;
    private Vector3 objectScreenPos;

    private void Awake()
    {
        m_SphereCollider = GetComponent<SphereCollider>();
    }

    public void Init(float fThreshold, bool directional = false)
    {
        // Change the sphere colliders radius to match the desired threshold
        // Object being placed will snap to this point when your pointer enters the collider
        m_SphereCollider.radius = fThreshold;

        _Directional = directional;
    }

    private void Update()
    {
        // use alpha to fade colour based on distance to the mouse pointer
        if (Camera.current)
        {
            objectScreenPos = Camera.current.WorldToScreenPoint(transform.position);
        }
        float alpha = Mathf.Lerp(1, 0, Vector3.Distance(objectScreenPos, Input.mousePosition) * 0.01f);

        _SpriteRenderer.color = new Color(_SpriteRenderer.color.r, _SpriteRenderer.color.g, _SpriteRenderer.color.b, alpha);
    }

    public void SetValidity(bool valid)
    {
        _IsValid = valid;

        if(valid)
            _SpriteRenderer.color = _ValidColour;
        else
            _SpriteRenderer.color = _InvalidColour;
    }
}
