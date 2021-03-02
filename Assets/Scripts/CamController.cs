using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{

    public GameObject cam1;
    public GameObject cam2;
    
    //Top object in each UI hierarchy
    [SerializeField] private GameObject UiBuild;
    [SerializeField] private GameObject UiView;
    private UI_Controller UI_Controller;
    private UI_ViewController UI_ViewController;

    //Main menu object and script
    [SerializeField] private GameObject UiMain;
    private UI_MenuController UI_MenuController;

    bool UiToggle = false;

    // Start is called before the first frame update
    void Start()
    {
        // By default, the game begins using the third person cam
        cam1.SetActive(true);
        cam2.SetActive(false);
        UI_Controller = UiBuild.GetComponent<UI_Controller>();
        UI_MenuController = UiMain.GetComponent<UI_MenuController>();
        UI_ViewController = UiView.GetComponent<UI_ViewController>();
    }

    // Update is called once per frame
    void Update()
    {
        // If "k" is pressed,
        if (Input.GetKeyUp("k"))
        {
            // Flips active states on cameras (e.g. if cam1 is active, cam1 is now inactive and cam2 is now active, after pressing "k")
            UI_MenuController = UiMain.GetComponent<UI_MenuController>();
            cam1.SetActive(!cam1.activeSelf);
            cam2.SetActive(!cam2.activeSelf);
            UI_Controller.ResetBuildUI();
            UiToggle = !UiToggle;
            if(UiToggle == false)
            {
                UiView.SetActive(false);
                UiBuild.SetActive(true);
                UI_MenuController.buildMode = true;
            }
            else
            {
                UiView.SetActive(true);
                UiBuild.SetActive(false);
                UI_MenuController.buildMode = false;
            }
            UiMain.SetActive(false);
        }
    }
}
