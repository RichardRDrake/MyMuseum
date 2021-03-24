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
    [SerializeField] private GameObject MainMenu;
    private UI_MenuController mainMenuController;

    //Self-identifier
    public int identifier;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        mainMenuController = MainMenu.GetComponent<UI_MenuController>();
        if(!mainMenuController)
        {
            Debug.Log("Couldn't find mainMenuController");
        }
    }

    public void OnPressed()
    {
        mainMenuController.saveLoadIdentity = identifier;
        Debug.Log("Changed to" + mainMenuController.saveLoadIdentity);
        mainMenuController.ShowConfirmation();
    }
}
