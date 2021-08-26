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

    private float m_StretchHorizontalOS = 0;
    private float m_StretchVerticalOS = 0;

    private Color oldColour;

    private void Start()
    {
        
    }

    private void Apply(Texture image)
    {
        // For the demo it is just calculating the size based on pixel width/height and a given pixel density (Pixels per meter)
        Vector2 imageSizeInPixels = new Vector2(image.width, image.height);
        Vector2 imageSizeInWorld = imageSizeInPixels / _PixelDensity;


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
                        rend.sharedMaterial.mainTextureScale = new Vector2(imageSizeInWorld.x / _ContractedSize.x, 1.0f);
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
                        rend.sharedMaterial.mainTextureScale = new Vector2(1.0f, imageSizeInWorld.y / _ContractedSize.y);
                    }
                }
            }

        }
        

        // Set the Canvas size (Quad)
        _CanvasTransform.localScale = new Vector3(imageSizeInWorld.x, imageSizeInWorld.y, 1.0f);

        // Extract the Material
        Renderer renderer = _CanvasTransform.GetComponent<Renderer>();
        if (renderer)
        {
            // Note: You don't want to actually do this in Edit mode as it will leak instances
            // But in real-time you want the instanced material otherwise changing on texture would change it for all paintings in the scene
            Material canvasMaterial;
            if (_FrameType == FrameType.STRETCHING)
            {
               canvasMaterial = renderer.sharedMaterial;
            }
            else
            {
               canvasMaterial = renderer.sharedMaterial; // So shared material for in Editor (Will apply it to all in the scene)
            }
            // Apply the image to the Quads material
            if (canvasMaterial)
                canvasMaterial.mainTexture = image;
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
                        rend.sharedMaterial.mainTextureScale = Vector2.one;
                    }
                }
                else if (anchor.name.Equals("Vertical"))
                {
                    // Stretch in Y
                    anchor.localScale = Vector3.one;

                    // Adjust tilling in Y
                    foreach (Renderer rend in anchor.GetComponentsInChildren<Renderer>())
                    {
                        rend.sharedMaterial.mainTextureScale = Vector2.one;
                    }
                }
            }

        }

        // Set the Canvas size (Quad)
        _CanvasTransform.localScale = Vector3.one;
    }

    // Draw a simple gizmo to confirm contracted size
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, _ContractedSize);
    }

    private void Update()
    {
        // Just for testing, usually just use the function Apply
        if(_TestImage == null)
        {
            oldColour = _CanvasTransform.gameObject.GetComponent<Renderer>().material.color;
            _CanvasTransform.gameObject.GetComponent<Renderer>().material.color = new Color(oldColour.r, oldColour.g, oldColour.b, 0.0f); 
        }
        else 
        {
            Color newColour = new Color(1, 1, 1, 1.0f);
            _CanvasTransform.gameObject.GetComponent<Renderer>().material.color = newColour;
        }

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
