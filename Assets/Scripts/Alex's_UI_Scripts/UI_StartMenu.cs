using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_StartMenu : MonoBehaviour
{
    //Script controlling inputs on the start menu
    //Should be very familiar to anyone who's seen UI_Controller or UI_MenuController, as the latter has very similar functionality
    #region Menus
    [SerializeField] private GameObject MenuMain;
    [SerializeField] private GameObject MenuOptions;
    [SerializeField] private GameObject LoadRoom;
    [SerializeField] private GameObject Confirm;
    #endregion

    #region Main Menu Contents
    [SerializeField] private GameObject LoadEditButton;
    [SerializeField] private GameObject LoadViewButton;
    [SerializeField] private GameObject OptionsButton;
    [SerializeField] private GameObject QuitButton;
    private List<Vector3> MainLocations = new List<Vector3>();
    #endregion

    #region Load screen variables
    //Allows save files to be independently hidden when loading a new page
    [SerializeField] private GameObject ObjectsHide;

    //determines which save file is currently selected
    public int saveLoadIdentity;

    //Script managing save files
    private Level saves;

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
    [SerializeField] private GameObject HighlightSave;
    [SerializeField] private GameObject HighlightSaveHover;
    [SerializeField] private GameObject HighlightSaveNavHover;
    #endregion

    #region Options Contents
    [SerializeField] private GameObject MusicSliderObject;
    private Slider musicSlider;
    [SerializeField] private GameObject SfxSliderObject;
    private Slider sfxSlider;
    private UI_Options uiOptions;
    [SerializeField] private GameObject SlidersParent;
    [SerializeField] private GameObject OptionsBack;
    #endregion

    #region Confirm Contents
    [SerializeField] private GameObject ConfirmButton;
    [SerializeField] private GameObject CancelButton;
    [SerializeField] private TextMeshProUGUI ConfirmText;
    #endregion

    #region Highlights
    [SerializeField] private GameObject HighlightTop1;
    [SerializeField] private GameObject HighlightTop2;
    [SerializeField] private GameObject HighlightTopHover1;
    [SerializeField] private GameObject HighlightTopHover2;
    [SerializeField] private GameObject HighlightConfirm;
    [SerializeField] private GameObject HighlightConfirmHover;
    [SerializeField] private GameObject HighlightOptionsSlider;
    [SerializeField] private GameObject HighlightOptionsBack;
    [SerializeField] private GameObject HighlightOptionsBackHover;
    #endregion

    //For loading the game in edit vs view mode
    private bool isEditable = false;

    //For whether the confirm button is for quitting the game or loading a room
    private bool isLoading = false;

    #region enums
    //Tracks which UI level the user is on
    public enum WindowFinder { MenuTop = 1, MenuLoad = 2, MenuOptions = 3, Confirm = 4 }
    public WindowFinder windowCurrent = WindowFinder.MenuTop;

    //Tracks which object on the main window the user is on
    public enum MainFinder { Null = 0, LoadEdit = 1, LoadView = 2, Options = 3, Quit = 4 }
    public MainFinder mainCurrent = MainFinder.Null;

    //Tracks which object in the Save/Load window the user is on
    public int paneCurrent = 0;

    //Tracks which object in the Options window the user is on
    public enum OptionsFinder { Null = 0, Music = 1, SFX = 2, Back = 3 }
    public OptionsFinder optionsCurrent = OptionsFinder.Null;

    //Tracks which object in the confirm window the user is on
    public enum ConfirmFinder { Null = 0, Confirm = 1, Cancel = 2 }
    public ConfirmFinder confirmCurrent = ConfirmFinder.Null;
    #endregion

    private MainMenu sceneNavigation;

    // Start is called before the first frame update
    void Start()
    {
        #region Set variables
        
        sceneNavigation = GetComponent<MainMenu>();
        saves = GetComponent<Level>();

        #region Audio variables
        //Gets the slider components of the music and sfx slider objects
        //Then sets the slider starting values to either the value in PlayerPrefs or, if those do not exist, 0.8f
        musicSlider = MusicSliderObject.GetComponent<Slider>();
        sfxSlider = SfxSliderObject.GetComponent<Slider>();
        if (PlayerPrefs.HasKey("BGM"))
        {
            musicSlider.value = PlayerPrefs.GetFloat("BGM");
            sfxSlider.value = PlayerPrefs.GetFloat("SFX");
        }
        else
        {
            musicSlider.value = 0.8f;
            sfxSlider.value = 0.8f;
        }
        #endregion

        #region Lists of Gameobjects, for positioning highlights
        MainLocations.Add(LoadEditButton.transform.position);
        MainLocations.Add(LoadViewButton.transform.position);
        MainLocations.Add(OptionsButton.transform.position);
        MainLocations.Add(QuitButton.transform.position);
        #endregion

        #region Variables and lists involved with save file management
        countText = PageCounter.GetComponent<TextMeshProUGUI>();
        SaveLoadLocations.Add(SaveLoad1.transform.position);
        SaveLoadLocations.Add(SaveLoad2.transform.position);
        SaveLoadLocations.Add(SaveLoad3.transform.position);
        saveTexts.Add(SaveTitle1.GetComponent<TextMeshProUGUI>());
        saveTexts.Add(SaveTitle2.GetComponent<TextMeshProUGUI>());
        saveTexts.Add(SaveTitle3.GetComponent<TextMeshProUGUI>());
        saveDates.Add(SaveDate1.GetComponent<TextMeshProUGUI>());
        saveDates.Add(SaveDate2.GetComponent<TextMeshProUGUI>());
        saveDates.Add(SaveDate3.GetComponent<TextMeshProUGUI>());
        saveNews.Add(SaveNew1.GetComponent<TextMeshProUGUI>());
        saveNews.Add(SaveNew2.GetComponent<TextMeshProUGUI>());
        saveNews.Add(SaveNew3.GetComponent<TextMeshProUGUI>());
        #endregion
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        //Moves the player back up a menu level from wherever they were.
        if (Input.GetKeyDown("escape") || Input.GetKeyDown("backspace"))
        {
            NavigateUp();
        }

        //Determines what to do when the player presses space or enter.
        if (Input.GetKeyDown("space") || Input.GetKeyDown("enter") || Input.GetKeyDown("return"))
        {
            NavigateDown();
        }

        //Determines what to do if the player hits tab or down
        if (Input.GetKeyDown("tab") || Input.GetKeyDown("down"))
        {
            CycleThrough();
        }

        //Determines what to do if the player hits up
        if (Input.GetKeyDown("up"))
        {
            CycleBack();
        }

        //Pertaining to use of arrow keys to change pages in catalogue
        #region Catalogue Scrolling
        if (Input.GetKeyDown("left") && windowCurrent == WindowFinder.MenuLoad)
        {
            DecrementPage();
        }
        //Or adjust volume sliders
        else if (Input.GetKey("left") && windowCurrent == WindowFinder.MenuOptions)
        {
            if (optionsCurrent == OptionsFinder.Music)
            {
                musicSlider.value -= 0.6f * Time.deltaTime;
            }
            else if (optionsCurrent == OptionsFinder.SFX)
            {
                sfxSlider.value -= 0.6f * Time.deltaTime;
            }
        }

        if (Input.GetKeyDown("right") && windowCurrent == WindowFinder.MenuLoad)
        {
            IncrementPage();
        }
        else if (Input.GetKey("right") && windowCurrent == WindowFinder.MenuOptions)
        {
            if (optionsCurrent == OptionsFinder.Music)
            {
                musicSlider.value += 0.6f * Time.deltaTime;
            }
            else if (optionsCurrent == OptionsFinder.SFX)
            {
                sfxSlider.value += 0.6f * Time.deltaTime;
            }
        }
        #endregion
    }

    #region catalogue page voids
    public void IncrementPage()
    {

    }

    public void DecrementPage()
    {

    }
    #endregion

    public void NavigateUp()
    {
        #region Move up the hierarchy
        //determine which menu layer the player is on
        int windowInt = (int)windowCurrent;
        switch (windowInt)
        {
            case 1:
                //if the player is on the main menu, brings up the quit game popup
                isLoading = false;
                DisplayConfirmation();
                break;
            case 3:
                //If the player is in the options menu, returns them to the main menu
                //after saving the data in the options sliders to PlayerPrefs
                uiOptions = SlidersParent.GetComponent<UI_Options>();
                PlayerPrefs.SetFloat("BGM", uiOptions.musicVolume);
                Debug.Log("BGM set to " + uiOptions.musicVolume);
                PlayerPrefs.SetFloat("SFX", uiOptions.sfxVolume);
                DisableHovers();
                windowCurrent = WindowFinder.MenuTop;
                mainCurrent = MainFinder.Null;
                optionsCurrent = OptionsFinder.Null;
                MenuMain.SetActive(true);
                MenuOptions.SetActive(false);
                break;
            case 4:
                //If the player is in a confirm menu
                //derermines where the confirm menu is, then reverts to the prior menu
                if (isLoading == false)
                {
                    windowCurrent = WindowFinder.MenuTop;
                }
                else
                {
                    windowCurrent = WindowFinder.MenuLoad;
                }
                confirmCurrent = ConfirmFinder.Null;
                HighlightConfirm.SetActive(false);
                HighlightConfirmHover.SetActive(false);
                Confirm.SetActive(false);
                break;
        }
        #endregion
    }

    public void NavigateDown()
    {
        #region Move down the hierarchy
        //determines which menu layer the player is on
        int windowInt = (int)windowCurrent;
        switch (windowInt)
        {
            case 1:
                //navigates to other menus from the top level menu
                int mainInt = (int)mainCurrent;
                switch (mainInt)
                {
                    case 1:
                        LoadEdit();
                        break;
                    case 2:
                        LoadView();
                        break;
                    case 3:
                        DisplayOptions();
                        break;
                    case 4:
                        isLoading = false;
                        DisplayConfirmation();
                        break;
                    default:
                        break;
                }
                break;
            case 3:
                //Navigates back to the main menu from the options menu
                int optionsInt = (int)optionsCurrent;
                if(optionsInt == 3)
                {
                    NavigateUp();
                }
                break;
            case 4:
                int confirmInt = (int)confirmCurrent;
                if(confirmInt == 1)
                //checks whether the confirm menu is being accessed from the main or load menus
                //and reacts accordingly
                {
                    if (isLoading == true)
                    {

                    }
                    else
                    {
                        sceneNavigation.QuitGame();
                    }
                }
                if(confirmInt == 2)
                //Otherwise, closes the confirm menu
                {
                    NavigateUp();
                }
                break;
        }
        #endregion
    }

    public void CycleThrough()
    {
        #region Cycles through options on a given page
        //determines which menu layer the player is on
        int windowInt = (int)windowCurrent;
        switch (windowInt)
        {
            case 1:
                //cycles through available options in the top level menu
                //sets the highlight over the current target
                int mainInt = (int)mainCurrent;
                mainInt++;
                if(mainInt > 4)
                {
                    mainInt = 1;
                }
                if(mainInt > 2)
                {
                    HighlightTop1.transform.position = MainLocations[mainInt - 1];
                    HighlightTop1.SetActive(true);
                    HighlightTop2.SetActive(false);
                }
                else
                {
                    HighlightTop2.transform.position = MainLocations[mainInt - 1];
                    HighlightTop2.SetActive(true);
                    HighlightTop1.SetActive(false);
                }
                mainCurrent = (MainFinder)mainInt;
                break;
            case 3:
                //Cycles through available options in the options menu
                //sets the highlight over the current target
                int optionsInt = (int)optionsCurrent;
                optionsInt++;
                if(optionsInt > 3)
                {
                    optionsInt = 1;
                }
                if(optionsInt == 1)
                {
                    HighlightOptionsSlider.transform.position = MusicSliderObject.transform.position;
                    HighlightOptionsSlider.SetActive(true);
                    HighlightOptionsBack.SetActive(false);
                }
                else if(optionsInt == 2)
                {
                    HighlightOptionsSlider.transform.position = SfxSliderObject.transform.position;
                    HighlightOptionsSlider.SetActive(true);
                    HighlightOptionsBack.SetActive(false);
                }
                else
                {
                    HighlightOptionsBack.transform.position = OptionsBack.transform.position;
                    HighlightOptionsBack.SetActive(true);
                    HighlightOptionsSlider.SetActive(false);
                }
                optionsCurrent = (OptionsFinder)optionsInt;
                break;
            case 4:
                //Cycles through available options in the confirm menu
                //sets the highlight over the current target
                int confirmInt = (int)confirmCurrent;
                confirmInt++;
                if(confirmInt > 2)
                {
                    confirmInt = 1;
                }
                if(confirmInt == 1)
                {
                    HighlightConfirm.transform.position = ConfirmButton.transform.position;
                    HighlightConfirm.SetActive(true);
                }
                else
                {
                    HighlightConfirm.transform.position = CancelButton.transform.position;
                    HighlightConfirm.SetActive(true);
                }
                confirmCurrent = (ConfirmFinder)confirmInt;
                break;
        }
        #endregion
    }

    public void CycleBack()
    {
        #region Cycles backwards through options on a given page
        //determines which menu layer the player is on
        int windowInt = (int)windowCurrent;
        switch (windowInt)
        {
            case 1:
                //Cycles backwards through available options in the top level menu
                //sets the highlight over the current target
                int mainInt = (int)mainCurrent;
                mainInt--;
                if(mainInt < 1)
                {
                    mainInt = 4;
                }
                if(mainInt > 3)
                {
                    HighlightTop1.transform.position = MainLocations[mainInt - 1];
                    HighlightTop1.SetActive(true);
                    HighlightTop2.SetActive(false);
                }
                else
                {
                    HighlightTop2.transform.position = MainLocations[mainInt - 1];
                    HighlightTop2.SetActive(true);
                    HighlightTop1.SetActive(false);
                }
                mainCurrent = (MainFinder)mainInt;
                break;
            case 3:
                //Cycles backwards through available options in the options menu
                //sets the highlight over the current target
                int optionsInt = (int)optionsCurrent;
                optionsInt--;
                if(optionsInt < 1)
                {
                    optionsInt = 3;
                }
                if (optionsInt == 1)
                {
                    HighlightOptionsSlider.transform.position = MusicSliderObject.transform.position;
                    HighlightOptionsSlider.SetActive(true);
                    HighlightOptionsBack.SetActive(false);
                }
                else if (optionsInt == 2)
                {
                    HighlightOptionsSlider.transform.position = SfxSliderObject.transform.position;
                    HighlightOptionsSlider.SetActive(true);
                    HighlightOptionsBack.SetActive(false);
                }
                else
                {
                    HighlightOptionsBack.transform.position = OptionsBack.transform.position;
                    HighlightOptionsBack.SetActive(true);
                    HighlightOptionsSlider.SetActive(false);
                }
                optionsCurrent = (OptionsFinder)optionsInt;
                break;
            case 4:
                //Cycles backwards through available options in the confirm menu
                //sets the highlight over the current target
                int confirmInt = (int)mainCurrent;
                confirmInt--;
                if(confirmInt < 1)
                {
                    confirmInt = 2;
                }
                if(confirmInt == 1)
                {
                    HighlightConfirm.transform.position = ConfirmButton.transform.position;
                    HighlightConfirm.SetActive(true);
                }
                else
                {
                    HighlightConfirm.transform.position = CancelButton.transform.position;
                    HighlightConfirm.SetActive(true);
                }
                confirmCurrent = (ConfirmFinder)confirmInt;
                break;

        }
        #endregion
    }

    #region Button voids
    public void LoadEdit()
    {
        #region Activates the load menu in edit mode
        windowCurrent = WindowFinder.MenuLoad;
        DisableHovers();
        isEditable = true;
        MenuMain.SetActive(false);
        LoadRoom.SetActive(true);
        #endregion
    }

    public void MenuSetup()
    {
        #region Determines the data displayed in the load game menus
        /*
        //Hides display while loading menus
        MenuMain.SetActive(false);
        LoadRoom.SetActive(true);
        windowCurrent = WindowFinder.MenuLoad;
        */
        #endregion
    }

    public void LoadView()
    {
        #region Activates the load menu in view mode
        windowCurrent = WindowFinder.MenuLoad;
        DisableHovers();
        isEditable = false;
        MenuMain.SetActive(false);
        LoadRoom.SetActive(true);
        #endregion
    }

    public void DisplayOptions()
    {
        #region Navigates to the options menu
        windowCurrent = WindowFinder.MenuOptions;
        DisableHovers();
        MenuMain.SetActive(false);
        MenuOptions.SetActive(true);
        #endregion
    }

    public void DisplayConfirmation()
    {
        #region Displays the confirmation menu
        windowCurrent = WindowFinder.Confirm;
        if(!isLoading)
        {
            ConfirmText.text = "Quit MyMuseum?";
        }
        else if(isEditable)
        {
            ConfirmText.text = "Load this room in edit mode?";
        }
        else
        {
            ConfirmText.text = "Load this room in view mode?";
        }
        Confirm.SetActive(true);
        #endregion
    }

    #endregion

    public void DisableHovers()
    {
        HighlightTop1.SetActive(false);
        HighlightTop2.SetActive(false);
        HighlightTopHover1.SetActive(false);
        HighlightTopHover2.SetActive(false);
        HighlightConfirm.SetActive(false);
        HighlightConfirmHover.SetActive(false);
        HighlightOptionsSlider.SetActive(false);
        HighlightOptionsBack.SetActive(false);
        HighlightOptionsBackHover.SetActive(false);
        HighlightSave.SetActive(false);
        HighlightSaveHover.SetActive(false);
        HighlightSaveNavHover.SetActive(false);
    }
}
