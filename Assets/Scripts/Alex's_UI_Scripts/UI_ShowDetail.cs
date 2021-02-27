using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ShowDetail : MonoBehaviour
{
    //Gets its relevant text field, the detail panel, and the text field within
    [SerializeField] private GameObject MyTextField;
    [SerializeField] private GameObject DetailPanel;
    [SerializeField] private GameObject DetailTextField;
    [SerializeField] private GameObject UIControllerHost;
    private TextMeshProUGUI myText;
    private TextMeshProUGUI detailText;
    private UI_Controller UI_Controller;

    //The number of the detail panel, from 1-6 (because the useage in UI_Controller considers 0 "null")
    //Used to centre the highlight over the relevant panel
    [SerializeField] private int panelNumber;

    // Start is called before the first frame update
    void Start()
    {
        UI_Controller = UIControllerHost.GetComponent<UI_Controller>();
        myText = MyTextField.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPress()
    {
        //Hides panel while resetting variables
        DetailPanel.SetActive(false);
        detailText = DetailTextField.GetComponent<TextMeshProUGUI>();

        //First, checks that this panel contains details
        //If it doesn't, clicking this does nothing.
        if (myText.text != " ")
        {
            detailText.text = myText.text;

            //Sets correct variables in the UI controller
            UI_Controller.windowCurrent = UI_Controller.windowFinder.Detail;
            UI_Controller.detailCurrent = UI_Controller.detailFinder.Null;
            UI_Controller.paneCurrent = panelNumber;

            //Then prompts it to place the highlight in the correct place
            UI_Controller.CataloguePaneHighlight();

            DetailPanel.SetActive(true);
        }
        else
        {
            UI_Controller.windowCurrent = UI_Controller.windowFinder.Catalogue;
        }
    }
}
