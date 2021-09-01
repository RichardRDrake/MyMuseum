using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

/// <summary>
/// This is a demo script:
/// Displays basic functions on object selected for edit
/// and then feeds back the information to the placeable currently being edited
/// </summary>
public class DC_EditObject : MonoBehaviour
{
    [Header("Editor camera for focusing on object being edited")]
    public DC_EditorCamera _EditorCamera;

    [Header("Canvas settings")]
    [Tooltip("Canvas to enable/disable Edit GUI")]
    public Canvas _Canvas;
    public GameObject _PaintingButton;
    public GameObject _RotateClockwiseButton;
    public GameObject _RotateAntiClockwiseButton;
    [Tooltip("Positioner transform to set location and scale based on the selected object")]
    public RectTransform _Positioner;
    public List<GameObject> objectDisplay;
    public Sprite emptyImage;

    [Header("Scaling settings")]
    public Vector2 _MinRectSize = new Vector2(32.0f, 32.0f);
    public Vector2 _MaxRectSize = new Vector2(128.0f, 128.0f);

    // This is the current placeable object that this editor is editing
    private DC_Placeable m_CurrentPlaceableObject = null;
    private GameObject m_CurrentGAmeObject;
    private bool m_EnabledThisFrame = false;

    // Check to see if we're about to be destroyed.
    private static bool m_ShuttingDown = false;
    private static object m_Lock = new object();
    private static DC_EditObject m_Instance;

    // Current bounds and forward vector of the selected object
    private Bounds m_CurrentSelectedBounds;
    private Vector3 m_CurrentSelectedForward;

    private GameObject menuItem;
    private List<AssetPlacerScriptableObject> readFrom;
    private int listLength;
    private int pageCount;
    private int pageCurrent;
    public TextMeshProUGUI countText;

    private int pageNumber;

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
    private void OnDestroy()
    {
        m_ShuttingDown = true;
    }

    public void Init(Bounds encapsulatedBounds, GameObject placeableObjectToEdit, Vector2 pixelSize)
    {
        // Enable the Edit HUD
        _Canvas.enabled = true;

        if(placeableObjectToEdit.GetComponent<DC_PictureFraming>())
        {
            _PaintingButton.SetActive(true);
            _RotateClockwiseButton.SetActive(false);
            _RotateAntiClockwiseButton.SetActive(false);
        }
        else 
        {
            _PaintingButton.SetActive(false);
            _RotateClockwiseButton.SetActive(true);
            _RotateAntiClockwiseButton.SetActive(true);
        }

        // Save the bounds and forward direction
        m_CurrentSelectedBounds = encapsulatedBounds;
        m_CurrentSelectedForward = placeableObjectToEdit.transform.forward;

        // Calculate position and scale of object in screen space
        Rect rect = GUIRectWithObject(encapsulatedBounds, _MinRectSize, _MaxRectSize);

        // Scale to move the buttons out around the selected object
        //_Positioner.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.width);
        //_Positioner.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.height);

        // Set the position in screen space
        _Positioner.position = rect.center;

        // Make the inputted object the object buttons edit (Send feedback to)
        m_CurrentPlaceableObject = placeableObjectToEdit.GetComponent<DC_Placeable>();
        m_CurrentGAmeObject = placeableObjectToEdit;

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

    public void OpenMenu(GameObject menu)
    {
        if (menu.activeSelf == false)
        {
            menuItem = menu;
            menuItem.SetActive(true);
        }
        else
            menuItem.SetActive(false);
    }

    public void SetAssetList()
    {
        if (menuItem.GetComponent<TempListScript>())
        {
            readFrom = menuItem.GetComponent<TempListScript>().GetList(ArtefactCategory.Images, "Images");
           

            if (m_CurrentGAmeObject.GetComponent<DC_PictureFraming>()._FrameType == DC_PictureFraming.FrameType.SLIDING)
            {
                for (int t = 0; t < readFrom.Count; t++)
                {
                    if (readFrom[t].PaintingPixelSize.x > m_CurrentGAmeObject.GetComponent<DC_PictureFraming>().MaxContractedSize.x ||
                         readFrom[t].PaintingPixelSize.y > m_CurrentGAmeObject.GetComponent<DC_PictureFraming>().MaxContractedSize.y ||
                         readFrom[t].PaintingPixelSize.x < m_CurrentGAmeObject.GetComponent<DC_PictureFraming>().MinContractedSize.x ||
                         readFrom[t].PaintingPixelSize.y < m_CurrentGAmeObject.GetComponent<DC_PictureFraming>().MinContractedSize.y)
                    {
                        readFrom.RemoveAt(t);
                    }
                }
            }
            listLength = readFrom.Count + 1;
            Debug.Log(listLength);
            pageCount = listLength / 4;
            if (listLength % 4 > 0)
            {
                pageCount++;
            }

            //Makes sure there is always one page
            if (listLength == 0)
            {
                pageCount = 1;
            }

            for (int i = 0; i < objectDisplay.Count; i++)
                objectDisplay[i].SetActive(false);
            pageCurrent = 1;
            ShowList();
            for (int i = 0; i < objectDisplay.Count; i++)
                objectDisplay[i].SetActive(true);
        }
       
    }
    public void IncrementPage()
    {
        //Cycles pages upward
        pageCurrent++;
        pageCurrent = Mathf.Clamp(pageCurrent, 1, pageCount);
       
        Debug.Log("Current page is: " + pageCurrent.ToString() + ". Max page is: " + pageCount.ToString() + ".");
        //Sets currently selected pane to 0
        for (int i = 0; i < objectDisplay.Count; i++)
            objectDisplay[i].SetActive(false);
        ShowList();
        for (int i = 0; i < objectDisplay.Count; i++)
            objectDisplay[i].SetActive(true);
    }

    public void DecrementPage()
    {
        //Cycles pages downward
        pageCurrent--;
        pageCurrent = Mathf.Clamp(pageCurrent, 1, pageCount);
        Debug.Log("Current page is: " + pageCurrent.ToString() + ". Max page is: " + pageCount.ToString() + ".");
        //Sets currently selected pane to 0
        for (int i = 0; i < objectDisplay.Count; i++)
            objectDisplay[i].SetActive(false);
        ShowList();
        for (int i = 0; i < objectDisplay.Count; i++)
            objectDisplay[i].SetActive(true);
    }

    void ShowList()
    {
        countText.text = pageCurrent.ToString() + " / " + pageCount.ToString();
        for (int i = 0; i <= 3; i++)
        {
            //Debug.Log(pageCurrent);
            pageNumber = ((pageCurrent - 1) * 4) + i;
            if (pageNumber == (listLength - 1))
            {
                objectDisplay[i].GetComponent<Image>().sprite = emptyImage;
            }
            else if (pageNumber > (readFrom.Count - 1))
            {
                objectDisplay[i].GetComponent<Image>().sprite = emptyImage;
            }
            else
            {
                if (readFrom[pageNumber] == null)
                {
                    continue;
                }
                //Debug.Log(Resources.readFrom[pageNumber].ArtefactName);
                objectDisplay[i].GetComponent<Image>().sprite = Sprite.Create(readFrom[pageNumber].PreviewImages[0], new Rect(0.0f, 0.0f, readFrom[pageNumber].PreviewImages[0].width,
              readFrom[pageNumber].PreviewImages[0].height), new Vector2(0.0f, 0.0f));
            }
        }
    }
     
    public void OnClickedAsset(int panelNumber)
    {
      
        if (pageCurrent <= 1)
        {
            if (panelNumber - 1 < readFrom.Count)
            {
                m_CurrentGAmeObject.GetComponent<DC_PictureFraming>()._TestImage = readFrom[panelNumber - 1].PreviewImages[0];
                m_CurrentGAmeObject.GetComponent<DC_PictureFraming>().imageSizeInWorld = readFrom[panelNumber - 1].PaintingPixelSize;
                m_CurrentGAmeObject.GetComponent<DC_Placeable>().asset.Content = readFrom[panelNumber - 1].ArtefactContent;
                m_CurrentGAmeObject.GetComponent<DC_Placeable>().asset.Name = readFrom[panelNumber - 1].ArtefactName;
                m_CurrentGAmeObject.GetComponent<DC_Placeable>().asset.paintingIndex = panelNumber - 1;
            }
        }
        else
        {
            if ((panelNumber + (4 * (pageCurrent - 1)) - 1) < readFrom.Count)
            {
                m_CurrentGAmeObject.GetComponent<DC_PictureFraming>()._TestImage = readFrom[(panelNumber + (4 * (pageCurrent - 1)) - 1)].PreviewImages[0];
                m_CurrentGAmeObject.GetComponent<DC_PictureFraming>().imageSizeInWorld = readFrom[(panelNumber + (4 * (pageCurrent - 1)) - 1)].PaintingPixelSize;
                m_CurrentGAmeObject.GetComponent<DC_Placeable>().asset.Content = readFrom[(panelNumber + (4 * (pageCurrent - 1)) - 1)].ArtefactContent;
                m_CurrentGAmeObject.GetComponent<DC_Placeable>().asset.Name = readFrom[(panelNumber + (4 * (pageCurrent - 1)) - 1)].ArtefactName;
                m_CurrentGAmeObject.GetComponent<DC_Placeable>().asset.paintingIndex = (panelNumber + (4 * (pageCurrent - 1)) - 1);
            }
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

        Vector2 scale = new Vector2(Mathf.Clamp(max.x - min.x, minScale.x, maxScale.x), Mathf.Clamp( max.y - min.y, minScale.y, maxScale.y ) );
        Vector2 center = Camera.main.WorldToScreenPoint(cen);
        center = new Vector2(center.x - (scale.x * 0.5f), center.y - (scale.y * 0.5f));

        return new Rect(center, scale);
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

        // If 'F' button pressed, focus the camera on the object
        if(Input.GetKeyDown(KeyCode.F) && m_CurrentSelectedBounds != null && _EditorCamera)
        {
            _EditorCamera.FocusOnObject(m_CurrentPlaceableObject.gameObject);
        }


        // Calculate position and scale of object in screen space
        if (m_CurrentPlaceableObject)
        {
            Bounds encapsulatedBounds = m_CurrentPlaceableObject.GetComponentInChildren<Renderer>().bounds;
            foreach (Renderer renderer in m_CurrentPlaceableObject.GetComponentsInChildren<Renderer>())
                encapsulatedBounds.Encapsulate(renderer.bounds);
            Rect rect = GUIRectWithObject(encapsulatedBounds, _MinRectSize, _MaxRectSize);

            // Scale to move the buttons out around the selected object
            //_Positioner.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.width);
            //_Positioner.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.height);

            // Set the position in screen space
            _Positioner.position = rect.center;
        }
    }
}
