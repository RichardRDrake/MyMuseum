using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class DC_PictureFraming : MonoBehaviour
{
    // For testing
    public Texture _TestImage;
    private Texture m_SavedImage;

    // The canvas GameObject, this should be a Quad, the texture is applied as the mainTexture
    public Transform _CanvasTransform;

    // The pixel density is how many pixels represent 1 meter (Should really use real-life sizes)
    public int _PixelDensity = 100;

    // The size of the gap when all anchors are at 0,0,0
    public Vector2 _ContractedSize;

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
            if(anchor.name.Equals("Top"))
                anchor.localPosition = new Vector3(0.0f, offset.y, 0.0f);
            else if(anchor.name.Equals("Bottom"))
                anchor.localPosition = new Vector3(0.0f, -offset.y, 0.0f);
            else if(anchor.name.Equals("Left"))
                anchor.localPosition = new Vector3(-offset.x, 0.0f, 0.0f);
            else if(anchor.name.Equals("Right"))
                anchor.localPosition = new Vector3(offset.x, 0.0f, 0.0f);
        }

        // Set the Canvas size (Quad)
        _CanvasTransform.localScale = new Vector3(imageSizeInWorld.x, imageSizeInWorld.y, 1.0f);

        // Extract the Material
        Renderer renderer = _CanvasTransform.GetComponent<Renderer>();
        if (renderer)
        {
            // Note: You don't want to actually do this in Edit mode as it will leak instances
            // But in real-time you want the instanced material otherwise changing on texture would change it for all paintings in the scene
            //Material canvasMaterial = renderer.material;
            Material canvasMaterial = renderer.sharedMaterial; // So shared material for in Editor (Will apply it to all in the scene)

            // Apply the image to the Quads material
            if (canvasMaterial)
                canvasMaterial.mainTexture = image;
        }
    }

    // Draw a simple gizmo to confirm contracted size
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(Vector3.zero, _ContractedSize);
    }

    private void Update()
    {
        // Just for testing, usually just use the function Apply
        if(_TestImage != m_SavedImage)
        {
            m_SavedImage = _TestImage;

            Apply(m_SavedImage);
        }
    }

    public void GetFramesFromFolders()
    {
      var objects = Resources.LoadAll("Frames");

        Debug.Log(objects[0].name);
    }
}
