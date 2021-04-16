using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_SaveLoad : MonoBehaviour
{
    //UI SaveLoad is used by the three buttons associated with save files.
    //Found under UI_MainMenu/UI_SaveLoad/UI_ObjectsHide

    #region variables
    //Connects to the 
    //Or the canvas in the main scene
    [SerializeField] private GameObject MainMenu;
    private UI_MenuController mainMenuController;
    private UI_StartMenu startMenuController;

    //Self-identifier
    public int identifier;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //Should find this if it's within the game scene 
        mainMenuController = MainMenu.GetComponent<UI_MenuController>();
        if(!mainMenuController)
        {
            //Otherwise, it'll assume it's in the main menu
            //Debug.Log("Couldn't find mainMenuController.");
            //Debug.Log("Searching for startMenuController");
            startMenuController = MainMenu.GetComponent<UI_StartMenu>();
            if (!startMenuController)
            {
                //Debug.Log("Couldn't find startMenuController");
            }
            else
            {
                //Debug.Log("Found startMenuController");
            }
        }
    }

    public void OnPressed()
    {
        if (!mainMenuController)
        {
            //Behaviour in the main menu
            startMenuController.saveLoadIdentity = identifier;
            startMenuController.DisplayConfirmation();
        }
        else
        {
            //Behaviour in-game
            mainMenuController.saveLoadIdentity = identifier;
            Debug.Log("Changed to" + mainMenuController.saveLoadIdentity);
            mainMenuController.ShowConfirmation();
        }
    }
}
