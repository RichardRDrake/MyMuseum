using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class DC_PictureFraming : MonoBehaviour
{
    public enum FrameType
    {
        SLIDING,
        STRETCHING
    }
    public FrameType _FrameType = FrameType.SLIDING;

    public bool _SwapLeftRight = false;


    // For testing
    public Texture _TestImage;
    private Texture m_SavedImage;

    // The canvas GameObject, this should be a Quad, the texture is applied as the mainTexture
    public Transform _CanvasTransform;

    // The pixel density is how many pixels represent 1 meter (Should really use real-life sizes)
    public int _PixelDensity = 1000;

    // The size of the gap when all anchors are at 0,0,0
    public Vector2 _ContractedSize;

    public Vector2 MinContractedSize;
    public Vector2 MaxContractedSize;

    private float m_StretchHorizontalOS = 0;
    private float m_StretchVerticalOS = 0;

    public Material m_CanvasMaterial;
    public Material _TransparentPlaceholderMaterial;
    public Vector2 imageSizeInWorld = new Vector2(1.0f, 1.0f);

    private void Start()
    {
        ResetFrame();
    }

    private void Apply(Texture image)
    {
        // Using the size at the start in X/Y (So all Movable transforms are @(0,0,0))
        // Divided by 2 because both sides should go out by half etc.
        Vector2 offset = (imageSizeInWorld - _ContractedSize) / 2.0f;

        // Set all the anchors
        foreach (Transform anchor in GetComponentsInChildren<Transform>())
        {
            if (anchor.name.Equals("Top"))
                anchor.localPosition = new Vector3(0.0f, offset.y, 0.0f);
            else if (anchor.name.Equals("Bottom"))
                anchor.localPosition = new Vector3(0.0f, -offset.y, 0.0f);
            else if (anchor.name.Equals("Left"))
                anchor.localPosition = new Vector3(_SwapLeftRight ? offset.x : -offset.x, 0.0f, 0.0f);
            else if (anchor.name.Equals("Right"))
                anchor.localPosition = new Vector3(_SwapLeftRight ? -offset.x : offset.x, 0.0f, 0.0f);

            // If stretching scale the Center pieces
            if (_FrameType == FrameType.STRETCHING)
            {
                if(anchor.name.Equals("Horizontal"))
                {
                    // If m_StretchHorizontalOS == 0, then it's not been set yet
                    if (m_StretchHorizontalOS == 0)
                        m_StretchHorizontalOS = anchor.localScale.x;

                    // Stretch in X
                    anchor.localScale = new Vector3(imageSizeInWorld.x / _ContractedSize.x, 1.0f, 1.0f);

                    // Adjust tilling in X
                    foreach(Renderer rend in anchor.GetComponentsInChildren<Renderer>())
                    {
#if UNITY_EDITOR
                        rend.sharedMaterial.mainTextureScale = new Vector2(imageSizeInWorld.x / _ContractedSize.x, 1.0f);
#else
                        rend.material.mainTextureScale = new Vector2(imageSizeInWorld.x / _ContractedSize.x, 1.0f);
#endif
                    }
                }
                else if(anchor.name.Equals("Vertical"))
                {
                    // If m_StretchVerticalOS == 0, then it's not been set yet
                    if (m_StretchVerticalOS == 0)
                        m_StretchVerticalOS = anchor.localScale.y;

                    // Stretch in Y
                    anchor.localScale = new Vector3(1.0f, imageSizeInWorld.y / _ContractedSize.y, 1.0f);

                    // Adjust tilling in Y
                    foreach (Renderer rend in anchor.GetComponentsInChildren<Renderer>())
                    {
#if UNITY_EDITOR
                        rend.sharedMaterial.mainTextureScale = new Vector2(1.0f, imageSizeInWorld.y / _ContractedSize.y);
#else
                        rend.material.mainTextureScale = new Vector2(1.0f, imageSizeInWorld.y / _ContractedSize.y);
#endif
                    }
                }
            }

        }
        

        // Set the Canvas size (Quad)
        _CanvasTransform.localScale = new Vector3(imageSizeInWorld.x, imageSizeInWorld.y, 1.0f);

        // Apply image
        Renderer renderer = _CanvasTransform.GetComponent<Renderer>();
        if (renderer && m_CanvasMaterial)
        {
            m_CanvasMaterial.mainTexture = image;

#if UNITY_EDITOR
            renderer.sharedMaterial = m_CanvasMaterial;
#else
            renderer.material = m_CanvasMaterial;
#endif
               
        }
    }

    private void ResetFrame()
    {
        foreach (Transform anchor in GetComponentsInChildren<Transform>())
        {
            if (anchor.name.Equals("Top"))
                anchor.localPosition = Vector3.zero;
            else if (anchor.name.Equals("Bottom"))
                anchor.localPosition = Vector3.zero;
            else if (anchor.name.Equals("Left"))
                anchor.localPosition = Vector3.zero;
            else if (anchor.name.Equals("Right"))
                anchor.localPosition = Vector3.zero;

            // If stretching scale the Center pieces
            if (_FrameType == FrameType.STRETCHING)
            {
                if (anchor.name.Equals("Horizontal"))
                {
                    // Stretch in X
                    anchor.localScale = Vector3.one;

                    // Adjust tilling in X
                    foreach (Renderer rend in anchor.GetComponentsInChildren<Renderer>())
                    {
#if UNITY_EDITOR
                        rend.sharedMaterial.mainTextureScale = Vector2.one;
#else
                        rend.material.mainTextureScale = Vector2.one;
#endif
                    }
                }
                else if (anchor.name.Equals("Vertical"))
                {
                    // Stretch in Y
                    anchor.localScale = Vector3.one;

                    // Adjust tilling in Y
                    foreach (Renderer rend in anchor.GetComponentsInChildren<Renderer>())
                    {
#if UNITY_EDITOR
                        rend.sharedMaterial.mainTextureScale = Vector2.one;
#else
                        rend.material.mainTextureScale = Vector2.one;
#endif
                    }
                }
            }

        }

        // Set the Canvas size (Quad)
        _CanvasTransform.localScale = Vector3.one;

        // Extract the Material
        Renderer renderer = _CanvasTransform.GetComponent<Renderer>();
        if (renderer)
        {
            // Set it to the placeholder
#if UNITY_EDITOR
            renderer.sharedMaterial = _TransparentPlaceholderMaterial;
#else
            renderer.material = _TransparentPlaceholderMaterial;
#endif
        }
    }

    // Draw a simple gizmo to confirm contracted size
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, _ContractedSize);
    }

    private void Update()
    {
        if(_TestImage != m_SavedImage)
        {
            m_SavedImage = _TestImage;

            if (m_SavedImage)
                Apply(m_SavedImage);
            else
                ResetFrame();
        }
    }
}
