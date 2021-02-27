using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Highlight : MonoBehaviour
{
    //These have been removed, for now. Don't need them yet
    //[SerializeField] private GameObject Controller;
    //private UI_Controller UI_Controller;

    //So this UI component knows which stage in the UI hierarchy it belongs to.
    //Refer to UI_Controller.windowFinder Enum for a list of hierarchy tiers and their associated int values.
    //[SerializeField] private int levelInt;

    //The highlight object associated with the above layer.
    //The first two layers use UI_HighlightMenuTopH
    //The catalogue uses UI_HighlightCatalogueH
    [SerializeField] private GameObject Highlight;


    // Start is called before the first frame update
    void Start()
    {
        //UI_Controller = Controller.GetComponent<UI_Controller>();
    }

    public void HoverIn()
    {
        //If the user hovers over a button, moves the highlight there
        //UI_Controller = Controller.GetComponent<UI_Controller>();
        //int windowInt = (int)UI_Controller.windowCurrent;
        Highlight.SetActive(true);
        Highlight.transform.position = transform.position;
        
    }

    public void HoverOut()
    {
        //If the user hovers off a button **and they are on the same UI layer**
        //deletes the highlight
        //UI_Controller = Controller.GetComponent<UI_Controller>();
        //int windowInt = (int)UI_Controller.windowCurrent;
        Highlight.SetActive(false);
        Highlight.transform.position = transform.position;
    }
}
