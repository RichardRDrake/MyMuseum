using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ShowDetail : MonoBehaviour
{
    //Gets its relevant text field, the detail panel, and the text field within
    [SerializeField] private GameObject MyIcon;
    private Image iconImage;
    [SerializeField] private GameObject UIControllerHost;
    private UI_Controller UI_Controller;

    //The number of the detail panel, from 1-6 (because the useage in UI_Controller considers 0 "null")
    //Used to centre the highlight over the relevant panel
    [SerializeField] private int panelNumber;

    // Start is called before the first frame update
    void Start()
    {
        UI_Controller = UIControllerHost.GetComponent<UI_Controller>();
    }

    public void OnPress()
    {
        Debug.Log("Pressed!");
        iconImage = MyIcon.GetComponent<Image>();
        //First, checks that this panel contains details
        //If it doesn't, clicking this does nothing.
        if (iconImage.sprite != null)
        {
            UI_Controller = UIControllerHost.GetComponent<UI_Controller>();
            //Sets the current window to "Catalogue" to allow NavigateDown to function correctly.
            UI_Controller.windowCurrent = UI_Controller.windowFinder.Catalogue;
            //Sets paneCurrent to the button's identifier
            UI_Controller.paneCurrent = panelNumber;
            //Then sets the highlight over this object
            UI_Controller.CataloguePaneHighlight();
            //Then prompts UI_Controller to behave as if this button was selected by keyboard command
            UI_Controller.NavigateDown();
        }
        else
        {
            UI_Controller.windowCurrent = UI_Controller.windowFinder.Catalogue;
        }
    }
}
