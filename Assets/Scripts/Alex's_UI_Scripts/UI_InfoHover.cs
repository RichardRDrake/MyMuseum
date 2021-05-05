using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_InfoHover : MonoBehaviour
{
    //Script handling the "delete" and "upload" tooltips in the load page of the main menu

    [SerializeField] private GameObject Canvas;
    [SerializeField] private GameObject Tooltip;
    private UI_Highlight ui_Highlight;
    private UI_StartMenu ui_StartMenu;
    private bool hoverDisplay = false;
    private float keepTime;

    // Start is called before the first frame update
    void Start()
    {
        ui_Highlight = GetComponent<UI_Highlight>();
        if(!ui_Highlight)
        {
            Debug.Log("Could not find UI Highlight");
        }
        ui_StartMenu = Canvas.GetComponent<UI_StartMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        if(hoverDisplay)
        {
            keepTime += Time.deltaTime;
            if(keepTime > 0.4f)
            {
                Tooltip.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y + 5, Input.mousePosition.z);
                float var = Tooltip.transform.position.y;
                Tooltip.SetActive(true);
                hoverDisplay = false;
            }
        }
    }

    public void HoverIn()
    {
        hoverDisplay = true;
        ui_Highlight.HoverIn();
        Debug.Log("Using ui_Highlight");
    }

    public void HoverOut()
    {
        hoverDisplay = false;
        keepTime = 0.0f;
        Tooltip.SetActive(false);
        ui_StartMenu.RemoveDeleteHover();
        ui_StartMenu.RemoveUploadHover();
    }
}
