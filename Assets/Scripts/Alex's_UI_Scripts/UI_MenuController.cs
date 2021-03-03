using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_MenuController : MonoBehaviour
{
    #region variables

    #region Game objects and relevant variables
    //Build Menu UI
    [SerializeField] private GameObject BuildMenu;
    private UI_Controller UI_Controller;
    [SerializeField] private GameObject ViewMenu;
    private UI_ViewController UI_ViewController;

    //Determines whether it should return to build or view mode UI when dismissed
    public bool buildMode = true;

    //Object with saves
    [SerializeField] private GameObject SaveObject;
    private TempSavesScript Saves;

    //Menu main display
    [SerializeField] private GameObject Main;

    //Menu save and load menu
    [SerializeField] private GameObject SaveLoad;

    //Menu options window
    [SerializeField] private GameObject Options;
    #endregion

    #region Main menu contents
    //List of Main display buttons
    [SerializeField] private GameObject SaveButton;
    [SerializeField] private GameObject LoadButton;
    [SerializeField] private GameObject OptionsButton;
    [SerializeField] private GameObject QuitButton;
    private List<Vector3> MainLocations = new List<Vector3>();

    //Main display highlights
    [SerializeField] private GameObject HighlightTop;
    [SerializeField] private GameObject HighlightTopHover;
    #endregion

    #region Save/load contents
    //The text field at the top of that window
    [SerializeField] private GameObject SaveLoadText;

    //Allows save files to be independently hidden when loading a new page
    [SerializeField] private GameObject ObjectsHide;

    //Save is false, load is true
    public bool saveOrLoad = false;

    //Integers determining page count
    private int listLength;
    private int pageCount;

    //Current page
    private int pageCurrent;

    //List of Save/Load options
    [SerializeField] private GameObject SaveLoad1;
    [SerializeField] private GameObject SaveLoad2;
    [SerializeField] private GameObject SaveLoad3;
    private List<Vector3> SaveLoadLocations = new List<Vector3>();

    //Page counter
    [SerializeField] private GameObject PageCounter;
    private TextMeshProUGUI countText;

    //Save/Load text fields
    [SerializeField] private GameObject SaveTitle1;
    [SerializeField] private GameObject SaveTitle2;
    [SerializeField] private GameObject SaveTitle3;
    private List<TextMeshProUGUI> saveTexts = new List<TextMeshProUGUI>();

    //Save/Load date fields
    [SerializeField] private GameObject SaveDate1;
    [SerializeField] private GameObject SaveDate2;
    [SerializeField] private GameObject SaveDate3;
    private List<TextMeshProUGUI> saveDates = new List<TextMeshProUGUI>();

    //Save/Load image overlay fields
    [SerializeField] private GameObject SaveNew1;
    [SerializeField] private GameObject SaveNew2;
    [SerializeField] private GameObject SaveNew3;
    private List<TextMeshProUGUI> saveNews = new List<TextMeshProUGUI>();

    //Save display highlights
    [SerializeField] private GameObject HightlightSave;
    [SerializeField] private GameObject HighlightSaveHover;

    #endregion

    #region Options contents
    //Options display highlights
    [SerializeField] private GameObject HighlightOptions;
    [SerializeField] private GameObject HighlightOptionsHover;
    #endregion

    #region enums
    //Tracks which UI level the user is on
    public enum WindowFinder { MenuTop = 1, MenuSaveLoad = 2, MenuOptions = 3, Confirm = 4}
    public WindowFinder windowCurrent = WindowFinder.MenuTop;

    //Tracks which object on the main window the user is on
    public enum MainFinder { Null = 0, Save = 1, Load = 2, Options = 3, Quit = 4}
    public MainFinder mainCurrent = MainFinder.Null;

    //Tracks which object in the Save/Load window the user is on
    public int paneCurrent = 0;

    //Tracks which object in the Options window the user is on
    public enum OptionsFinder { Null = 0, SFX = 1, Music = 2}
    public OptionsFinder optionsCurrent = OptionsFinder.Null;
    #endregion

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //UI controller script attached to build menu
        UI_Controller = BuildMenu.GetComponent<UI_Controller>();
        UI_ViewController = ViewMenu.GetComponent<UI_ViewController>();
        Saves = SaveObject.GetComponent<TempSavesScript>();

        //Save/Load variables
        saveTexts.Add(SaveTitle1.GetComponent<TextMeshProUGUI>());
        saveTexts.Add(SaveTitle2.GetComponent<TextMeshProUGUI>());
        saveTexts.Add(SaveTitle3.GetComponent<TextMeshProUGUI>());
        saveDates.Add(SaveDate1.GetComponent<TextMeshProUGUI>());
        saveDates.Add(SaveDate2.GetComponent<TextMeshProUGUI>());
        saveDates.Add(SaveDate3.GetComponent<TextMeshProUGUI>());
        saveNews.Add(SaveNew1.GetComponent<TextMeshProUGUI>());
        saveNews.Add(SaveNew2.GetComponent<TextMeshProUGUI>());
        saveNews.Add(SaveNew3.GetComponent<TextMeshProUGUI>());
        countText = PageCounter.GetComponent<TextMeshProUGUI>();

        //Sets the locations the highlights can occupy
        MainLocations.Add(SaveButton.transform.position);
        MainLocations.Add(LoadButton.transform.position);
        MainLocations.Add(OptionsButton.transform.position);
        MainLocations.Add(QuitButton.transform.position); 

        SaveLoadLocations.Add(SaveLoad1.transform.position);
        SaveLoadLocations.Add(SaveLoad2.transform.position);
        SaveLoadLocations.Add(SaveLoad3.transform.position);

    }

    // Update is called once per frame
    void Update()
    {
        //Moves the player back up a menu level from wherever they were.
        if (Input.GetKeyDown("escape") || Input.GetKeyDown("backspace"))
        {
            NavigateUp();
        }

        //Determines what to do if the player hits enter or space
        if (Input.GetKeyDown("space") || Input.GetKeyDown("enter") || Input.GetKeyDown("return"))
        {
            NavigateDown();
        }
    }

    public void Activate()
    {
        //Sets other UI menus inactive once activated
        BuildMenu.SetActive(false);
    }

    public void NavigateUp()
    {
        //Similar in function to NavigateUp in the UI_Controller script
        //Moves back up the hierarchy based on windowCurrent
        #region Backing Up
        int windowInt = (int)windowCurrent;
        if (windowInt <4 && windowInt >1)
        {
            //Handles navigation from the save, load or options menus
            windowCurrent = (WindowFinder)1;
            DisableSubmenus();
        }
        else if (windowInt == 4)
        {
            //Handles navigation from a confirmation popup in the save or load menus
            windowCurrent = (WindowFinder)2;
            DisableConfirmation();
        }
        else if (windowInt == 1)
        {
            //Returns the user to the game
            if (buildMode == true)
            {
                BuildMenu.SetActive(true);
                UI_Controller.ResetBuildUI();
            }
            else
            {
                ViewMenu.SetActive(true);
                UI_ViewController.DisableMain();
            }
        }
        #endregion
    }

    private void NavigateDown()
    {
        //Moves into/further down submenus
        #region down hierarchy
        int windowInt = (int)windowCurrent;
        switch (windowInt)
        {
            case 1:
                int mainInt = (int)mainCurrent;
                if (mainInt > 0)
                {
                    DisableMainMenu();
                }
                switch (mainInt)
                {
                    case 1:
                        saveOrLoad = false;
                        break;
                    case 2:
                        saveOrLoad = true;
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    default:
                        break;
                }
                break;
        }


        #endregion
    }

    public void DisableSubmenus()
    {
        //Disables menus lower than the main menu in the heirarchy
        //Resets variables therein
        #region disable submenus
        SaveLoad.SetActive(false);
        paneCurrent = 0;
        HighlightSaveHover.SetActive(false);
        HightlightSave.SetActive(false);

        Options.SetActive(false);
        optionsCurrent = OptionsFinder.Null;
        HighlightOptions.SetActive(false);
        HighlightOptionsHover.SetActive(false);
        #endregion
    }

    private void DisableMainMenu()
    {
        //Disables the main menu
        //Resets variables therein
        #region disable mainmenu
        Main.SetActive(false);
        mainCurrent = MainFinder.Null;
        HighlightTop.SetActive(false);
        HighlightTopHover.SetActive(false);
        #endregion
    }

    public void DisableConfirmation()
    {
        
    }

    private void MenuSetup()
    {
        //Loads details for save file display
        #region sets up saves display
        //Hides display while loading data
        ObjectsHide.SetActive(false);

        //Creates a total list length based on number of existing saves
        //Includes all full pages, plus a page for the remainder
        listLength = Saves.SavesList.Count + 1;
        pageCount = listLength / 6;
        if (listLength % 6 > 0)
        {
            pageCount++;
        }

        //Makes sure there is always one page
        if (listLength == 0)
        {
            pageCount = 1;
        }

        pageCurrent = 1;
        DisplayPageDetails();


        ObjectsHide.SetActive(true);

        windowCurrent = WindowFinder.MenuSaveLoad;

        #endregion
    }

    private void DisplayPageDetails()
    {
        //Displays save data read from the save file list
    }


}
