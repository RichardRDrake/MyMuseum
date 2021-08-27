using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class UI_StartMenu : MonoBehaviour
{
    //Script controlling inputs on the start menu
    //Should be very familiar to anyone who's seen UI_Controller or UI_MenuController, as the latter has very similar functionality
    #region Initialising variables
    #region Menus
    [SerializeField] private GameObject MenuMain;
    [SerializeField] private GameObject MenuOptions;
    [SerializeField] private GameObject LoadRoom;
    [SerializeField] private GameObject Confirm;
    [SerializeField] private GameObject Upload;
    private string uploadFilename = "";
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
    private SaveLoadRoom saves;

    //The last accessed save's position in saves.savesList
    public int lastSaved;

    //Integers determining page count
    private int listLength;
    private int pageCount;

    //Current page
    private int pageCurrent;

    //The back button
    [SerializeField] private GameObject SaveBack;

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
    [SerializeField] private GameObject HighlightDelete;
    [SerializeField] private GameObject HighlightDeleteHover;
    [SerializeField] private GameObject HighlightUploadHover;

    //Variables pertaining to the delete save files button
    private bool deleteSaveFiles = false;
    [SerializeField] private GameObject DeleteButton;
    private UI_Highlight ui_Highlight;
    private bool uploadSaveFiles = false;
    [SerializeField] private GameObject UploadButton;
    private UI_Highlight ui_Highlight2;
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

    [SerializeField] private GameObject InputObject;
    private TMP_InputField inputField;
    private bool isSaving;
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
    [SerializeField] private GameObject HighlightSaveBack;
    [SerializeField] private GameObject HighlightSaveBackHover;
    #endregion

    #region Audio
    [SerializeField] private AudioMixer bgmMixer;
    [SerializeField] private AudioMixer sfxMixer;
    private AudioManager audioManager;
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
    //Note that, opposed to other instances of paneCurrent, pane 4 is the back button and not outside the acceptable range
    public int paneCurrent = 0;

    //Tracks which object in the Options window the user is on
    public enum OptionsFinder { Null = 0, Music = 1, SFX = 2, Back = 3 }
    public OptionsFinder optionsCurrent = OptionsFinder.Null;

    //Tracks which object in the confirm window the user is on
    public enum ConfirmFinder { Null = 0, Confirm = 1, Cancel = 2, Text = 3 }
    public ConfirmFinder confirmCurrent = ConfirmFinder.Null;
    #endregion

    #region Server side Room List
    [Header("Download/Upload panel")]
    public DC_RoomListPanel RoomListPanel;
    #endregion

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        #region Set variables
        saves = GetComponent<SaveLoadRoom>();

        #region Audio variables
        //Gets the slider components of the music and sfx slider objects
        //Then sets the slider starting values to either the value in PlayerPrefs or, if those do not exist, 0.8f
        musicSlider = MusicSliderObject.GetComponent<Slider>();
        sfxSlider = SfxSliderObject.GetComponent<Slider>();
        audioManager = FindObjectOfType<AudioManager>();
        if(!audioManager)
        {
            Debug.Log("Could not find AudioManager");
        }
        if (PlayerPrefs.HasKey("BGM"))
        {
            musicSlider.value = PlayerPrefs.GetFloat("BGM");
            bgmMixer.SetFloat("bgmVol", Mathf.Log10(PlayerPrefs.GetFloat("BGM")) * 20);
            sfxSlider.value = PlayerPrefs.GetFloat("SFX");
            sfxMixer.SetFloat("sfxVol", Mathf.Log10(PlayerPrefs.GetFloat("SFX")) * 20);
        }
        else
        {
            musicSlider.value = 0.8f;
            sfxSlider.value = 0.8f;
            bgmMixer.SetFloat("bgmVol", Mathf.Log10(0.8f) * 20);
            sfxMixer.SetFloat("bgmVol", Mathf.Log10(0.8f) * 20);
        }
        audioManager.Play("Title_Track_BGM");
        #endregion

        #region Lists of Gameobjects, for positioning highlights
        MainLocations.Add(LoadEditButton.transform.position);
        MainLocations.Add(LoadViewButton.transform.position);
        MainLocations.Add(OptionsButton.transform.position);
        MainLocations.Add(QuitButton.transform.position);
        #endregion

        #region Variables and lists involved with save file management
        PlayerPrefs.SetInt("RoomSetup", 1);

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
        inputField = InputObject.GetComponent<TMP_InputField>();
        ui_Highlight = DeleteButton.GetComponent<UI_Highlight>();
        ui_Highlight2 = UploadButton.GetComponent<UI_Highlight>();
        #endregion
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        #region Menu navigation
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
        #endregion

        #region Catalogue Scrolling
        //Pertaining to use of arrow keys to change pages in catalogue
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

    #region Catalogue scrolling
    public void IncrementPage()
    {
        pageCurrent++;
        pageCurrent = Mathf.Clamp(pageCurrent, 1, pageCount);
        //Debug.Log(pageCurrent);
        ObjectsHide.SetActive(false);
        DisplayPageDetails();
        ObjectsHide.SetActive(true);
        HighlightSave.SetActive(false);
        paneCurrent = 0;
    }

    public void DecrementPage()
    {
        pageCurrent--;
        pageCurrent = Mathf.Clamp(pageCurrent, 1, pageCount);
        //Debug.Log(pageCurrent);
        ObjectsHide.SetActive(false);
        DisplayPageDetails();
        ObjectsHide.SetActive(true);
        HighlightSave.SetActive(false);
        paneCurrent = 0;
    }
    #endregion

    #region Menu navigation
    public void NavigateUp()
    {
        #region Move up the hierarchy
        //determine which menu layer the player is on
        audioManager.Play("Button_Pressed_SFX");
        int windowInt = (int)windowCurrent;
        switch (windowInt)
        {
            case 1:
                #region if the player is on the main menu, brings up the quit game popup
                isLoading = false;
                DisplayConfirmation();
                break;
                #endregion
            case 2:
                #region If the player is in a load menu, returns them to the main menu
                DisableHovers();
                windowCurrent = WindowFinder.MenuTop;
                mainCurrent = MainFinder.Null;
                paneCurrent = 0;
                MenuMain.SetActive(true);
                LoadRoom.SetActive(false);
                isLoading = false;
                break;
                #endregion
            case 3:
                #region If the player is in the options menu, returns them to the main menu
                //after saving the data in the options sliders to PlayerPrefs
                uiOptions = SlidersParent.GetComponent<UI_Options>();
                PlayerPrefs.SetFloat("BGM", uiOptions.musicVolume);
                PlayerPrefs.SetFloat("SFX", uiOptions.sfxVolume);
                DisableHovers();
                windowCurrent = WindowFinder.MenuTop;
                mainCurrent = MainFinder.Null;
                optionsCurrent = OptionsFinder.Null;
                MenuMain.SetActive(true);
                MenuOptions.SetActive(false);
                break;
                #endregion
            case 4:
                #region If the player is in a confirm menu
                //derermines where the confirm menu is, then reverts to the prior menu
                if (isLoading == false)
                {
                    windowCurrent = WindowFinder.MenuTop;
                }
                else
                {
                    windowCurrent = WindowFinder.MenuLoad;
                    isSaving = false;
                    inputField.DeactivateInputField();
                }
                confirmCurrent = ConfirmFinder.Null;
                HighlightConfirm.SetActive(false);
                HighlightConfirmHover.SetActive(false);
                Confirm.SetActive(false);
                break;
                #endregion
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
                #region navigates to other menus from the top level menu
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
                #endregion
            case 2:
                #region nagivates to other menus from the load menus
                if (paneCurrent < 4)
                {
                    DisplayConfirmation();
                }
                else
                {
                    switch(paneCurrent)
                    {
                        case 4:
                            NavigateUp();
                            break;
                        case 5:
                            SetToUpload();
                            Debug.Log("Setting Upload");
                            break;
                        case 6:
                            SetToDelete();
                            Debug.Log("Setting Delete");
                            break;
                    }
                }
                break;
                #endregion
            case 3:
                #region navigates back to the main menu from the options menu
                int optionsInt = (int)optionsCurrent;
                if(optionsInt == 3)
                {
                    NavigateUp();
                }
                break;
                #endregion
            case 4:
                #region navigates from the confirmation popup
                int confirmInt = (int)confirmCurrent;
                if(confirmInt == 1)
                //checks whether the confirm menu is being accessed from the main or load menus
                //and reacts accordingly
                {
                    ConfirmPressed();
                }
                if(confirmInt == 2)
                //Otherwise, closes the confirm menu
                {
                    NavigateUp();
                }
                break;
                #endregion
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
                #region top menu cycling
                //cycles through available options in the top level menu
                //sets the highlight over the current target
                int mainInt = (int)mainCurrent;
                mainInt++;
                if (mainInt > 4)
                {
                    mainInt = 1;
                }
                if (mainInt > 2)
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
                #endregion
            case 2:
                #region load menu cycling
                //Cycles through available options in the load menu
                //sets the highlight over the current target
                paneCurrent++;

                if (paneCurrent > 6)
                {
                    paneCurrent = 1;
                }
                //If the player attempts to select an option beyond the available save files...
                else if (isEditable && ((pageCurrent - 1) * 3) + paneCurrent > listLength)
                {
                    paneCurrent = Mathf.Clamp(paneCurrent, 4, 6);
                }
                //The value of available options is one fewer when attempting to view a room
                else if (!isEditable && ((pageCurrent - 1) * 3) + paneCurrent > (listLength - 1))
                {
                    paneCurrent = Mathf.Clamp(paneCurrent, 4, 6);
                }

                Debug.Log(paneCurrent);

                HighlightSave.SetActive(false);
                HighlightSaveBack.SetActive(false);
                HighlightDelete.SetActive(false);

                if (paneCurrent < 4)
                {
                    HighlightSave.SetActive(true);
                    HighlightSave.transform.position = SaveLoadLocations[paneCurrent - 1];
                }
                else
                {
                    switch (paneCurrent)
                    {
                        case 4:
                            HighlightSaveBack.SetActive(true);
                            HighlightSaveBack.transform.position = SaveBack.transform.position;
                            break;
                        case 5:
                            Debug.Log("Set paneCurrent to " + paneCurrent);
                            HighlightDelete.SetActive(true);
                            HighlightDelete.transform.position = UploadButton.transform.position;
                            break;
                        case 6:
                            HighlightDelete.SetActive(true);
                            HighlightDelete.transform.position = DeleteButton.transform.position;
                            break;
                    }
                }
                break;
                #endregion
            case 3:
                #region options menu cycling
                //Cycles through available options in the options menu
                //sets the highlight over the current target
                int optionsInt = (int)optionsCurrent;
                optionsInt++;
                if (optionsInt > 3)
                {
                    optionsInt = 1;
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
                #endregion
            case 4:
                #region confirm popup cycling
                //Cycles through available options in the confirm menu
                //sets the highlight over the current target
                int confirmInt = (int)confirmCurrent;
                confirmInt++;
                if (!isSaving)
                {
                    if (confirmInt > 2)
                    {
                        confirmInt = 1;
                    }
                    if (confirmInt == 1)
                    {
                        HighlightConfirm.transform.position = ConfirmButton.transform.position;
                        HighlightConfirm.SetActive(true);
                    }
                    else
                    {
                        HighlightConfirm.transform.position = CancelButton.transform.position;
                        HighlightConfirm.SetActive(true);
                    }
                }
                else 
                { 
                   if (confirmInt > 3)
                   {
                        confirmInt = 1;
                   }
                   switch(confirmInt)
                   {
                        case 1:
                            HighlightConfirm.transform.position = ConfirmButton.transform.position;
                            HighlightConfirm.SetActive(true);
                            inputField.DeactivateInputField();
                            break;
                        case 2:
                            HighlightConfirm.transform.position = CancelButton.transform.position;
                            HighlightConfirm.SetActive(true);
                            break;
                        case 3:
                            HighlightConfirm.SetActive(false);
                            inputField.ActivateInputField();
                            break;
                   }
                }
                confirmCurrent = (ConfirmFinder)confirmInt;
                break;
                #endregion
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
                #region top menu cycling
                //Cycles backwards through available options in the top level menu
                //sets the highlight over the current target
                int mainInt = (int)mainCurrent;
                mainInt--;
                if (mainInt < 1)
                {
                    mainInt = 4;
                }
                if (mainInt > 3)
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
                #endregion
            case 2:
                #region load menu cycling
                //Cycles backwards through available options in the load menu
                //sets highlight over the current target
                paneCurrent--;
                if (paneCurrent < 1)
                {
                    paneCurrent = 6;
                }

                //Checks that the last option isn't out of range (if the last selected pane was 4)
                if (((pageCurrent - 1) * 3) + (paneCurrent) > (listLength - 1) && paneCurrent < 4)
                {
                    //If it is, cycles to the last populated list entry if there's less than one full page
                    //Or the delete saves button
                    if (pageCount - 1 < 1)
                    {
                        if (isEditable)
                        {
                            paneCurrent = listLength;
                        }
                        else
                        {
                            paneCurrent = listLength - 1;
                        }
                    }
                    else
                    {
                        //or higher than the remainder of the asset list divided by the number of full pages
                        paneCurrent = ((listLength - 1) % ((pageCount - 1) * 3));
                    }
                }

                HighlightSave.SetActive(false);
                HighlightSaveBack.SetActive(false);
                HighlightDelete.SetActive(false);

                if (paneCurrent < 4)
                {
                    HighlightSave.SetActive(true);
                    HighlightSave.transform.position = SaveLoadLocations[paneCurrent - 1];
                }
                else
                {
                    switch (paneCurrent)
                    {
                        case 4:
                            HighlightSaveBack.SetActive(true);
                            HighlightSaveBack.transform.position = SaveBack.transform.position;
                            break;
                        case 5:
                            HighlightDelete.SetActive(true);
                            HighlightDelete.transform.position = UploadButton.transform.position;
                            break;
                        case 6:
                            HighlightDelete.SetActive(true);
                            HighlightDelete.transform.position = DeleteButton.transform.position;
                            break;
                    }
                }

                break;
                #endregion
            case 3:
                #region options menu cycling
                //Cycles backwards through available options in the options menu
                //sets the highlight over the current target
                int optionsInt = (int)optionsCurrent;
                optionsInt--;
                if (optionsInt < 1)
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
                #endregion
            case 4:
                #region confirm popup cycling
                //Cycles backwards through available options in the confirm menu
                //sets the highlight over the current target
                int confirmInt = (int)mainCurrent;
                confirmInt--;
                if (!isSaving)
                {
                    if (confirmInt < 1)
                    {
                        confirmInt = 2;
                    }
                    if (confirmInt == 1)
                    {
                        HighlightConfirm.transform.position = ConfirmButton.transform.position;
                        HighlightConfirm.SetActive(true);
                    }
                    else
                    {
                        HighlightConfirm.transform.position = CancelButton.transform.position;
                        HighlightConfirm.SetActive(true);
                    }
                }
                else
                {
                    if (confirmInt < 1)
                    {
                        confirmInt = 3;
                    }
                    switch (confirmInt) 
                    {
                        case 1:
                            HighlightConfirm.transform.position = ConfirmButton.transform.position;
                            HighlightConfirm.SetActive(true);
                            inputField.DeactivateInputField();
                            break;
                        case 2:
                            HighlightConfirm.transform.position = CancelButton.transform.position;
                            HighlightConfirm.SetActive(true);
                            break;
                        case 3:
                            HighlightConfirm.SetActive(false);
                            inputField.ActivateInputField();
                            break;
                    }
                }
                confirmCurrent = (ConfirmFinder)confirmInt;
                break;
                #endregion
        }
        #endregion
    }
    #endregion

    #region Voids performing tasks associated with specific UI buttons

    public void LoadView()
    {
        #region Activates the load menu in view mode
        PlayerPrefs.SetInt("isExplore", 1);
        audioManager.Play("Button_Pressed_SFX");
        windowCurrent = WindowFinder.MenuLoad;
        DisableHovers();
        
        //Confirms that it is loading the room in a non-editable form
        isEditable = false;
        isLoading = true;

        MenuMain.SetActive(false);
        LoadRoom.SetActive(true);
        MenuSetup();
        #endregion
    }

    public void LoadEdit()
    {
        #region Activates the load menu in edit mode
        PlayerPrefs.SetInt("isExplore", 0);
        audioManager.Play("Button_Pressed_SFX");
        windowCurrent = WindowFinder.MenuLoad;
        DisableHovers();

        //Confirms that it is loading the room in an editable form
        isEditable = true;
        isLoading = true;
        MenuMain.SetActive(false);
        LoadRoom.SetActive(true);
        MenuSetup();
        #endregion
    }

    public void DisplayOptions()
    {
        #region Navigates to the options menu
        audioManager.Play("Button_Pressed_SFX");
        windowCurrent = WindowFinder.MenuOptions;
        DisableHovers();
        MenuMain.SetActive(false);
        MenuOptions.SetActive(true);
        #endregion
    }

    public void DisplayConfirmation()
    {
        #region Displays the confirmation menu
        //The Mathf.Clamps are to prevent negative values
        //Does nothing if attempting to load an empty save
        audioManager.Play("Button_Pressed_SFX");
        if (!isLoading || (saveTexts[Mathf.Clamp(saveLoadIdentity - 1, 0, 2)].text != "NO DATA"))
        {

            windowCurrent = WindowFinder.Confirm;
            #region Quit game
            if (!isLoading)
            {
                ConfirmText.text = "Quit MyMuseum?";
            }
            #endregion
            #region Create new room
            else if (saveTexts[Mathf.Clamp(paneCurrent - 1, 0, 2)].text == "Create new room" || saveTexts[Mathf.Clamp(saveLoadIdentity - 1, 0, 2)].text == "Create new room")
            {
                isSaving = true;
                ConfirmText.text = "Name your new room";
                inputField.text = "";
                InputObject.SetActive(true);
                confirmCurrent = ConfirmFinder.Text;
                inputField.ActivateInputField();
            }
            #endregion
            #region Download room
            else if (saveTexts[Mathf.Clamp(paneCurrent - 1, 0, 2)].text == "Download room" || saveTexts[Mathf.Clamp(saveLoadIdentity - 1, 0, 2)].text == "Download new room")
            {
                ConfirmText.text = "Browse rooms online?";
                InputObject.SetActive(false);
            }
            #endregion
            #region Delete, load or upload room
            else
            {
                InputObject.SetActive(false);
                if (deleteSaveFiles)
                {
                    ConfirmText.text = "Delete this save file?";
                }
                else if (uploadSaveFiles)
                {
                    ConfirmText.text = "Upload this save file to server?";
                }
                else
                {
                    if (isEditable)
                    {
                        ConfirmText.text = "Load this room in edit mode?";
                    }
                    else
                    {
                        ConfirmText.text = "Load this room in view mode?";
                    }
                }
            }
            #endregion
            Confirm.SetActive(true);
        }
        #endregion
    }

    public void RemoveDeleteHover()
    {
        #region Removes highlight over the delete save button, if deleteSaveFiles is false
        if (deleteSaveFiles == false)
        {
            ui_Highlight.HoverOut();
        }
        #endregion
    }

    public void RemoveUploadHover()
    {
        #region Removes highlight over the upload save button, if uploadSaveFiles is false
        if (uploadSaveFiles == false)
        {
            ui_Highlight2.HoverOut();
        }
        #endregion
    }

    public void ConfirmPressed()
    {
        #region When the confirm button is pressed
        audioManager.Play("Button_Pressed_SFX");
        if (!isLoading)
        {
            //If the confirm menu is active outside the context of the load menus
            Application.Quit();
        }
        else if (saveTexts[Mathf.Clamp(paneCurrent - 1, 0, 2)].text == "Create new room" || saveTexts[Mathf.Clamp(saveLoadIdentity - 1, 0, 2)].text == "Create new room")
        {
            //If this is selected, the player is attempting to create a new save
            //The player will need to load an initial room template.
            PlayerPrefs.SetInt("RoomSetup", 1);
            Destroy(audioManager);
            //The player will need to record the list position of the save file, and write to PlayerPrefs
            PlayerPrefs.SetInt("CurrentSave", lastSaved);
            //Commented out for now
            SceneManager.LoadScene("Main_Scene", LoadSceneMode.Single);
        }
        else if (saveTexts[Mathf.Clamp(paneCurrent - 1, 0, 2)].text == "Download room" || saveTexts[Mathf.Clamp(saveLoadIdentity - 1, 0, 2)].text == "Download new room")
        {
            //If this is selected, the player is attempting to download a room

            // RD TODO: Load download menu
            // Contains a Tab to view list of Public/Private rooms on the server
            // Private: If no user Key found (Set locally when User first launches the app from the website link) then display message to explain what needs to be done [They can still Load/save files locally]
            // Public: Requires internet connection [Can still load local rooms]

            if (RoomListPanel)
                RoomListPanel.gameObject.SetActive(true);

            Confirm.SetActive(false);
        }
        else
        {
            //If this is selected, the player is attempting to load an existing room
            if(deleteSaveFiles)
            {
                //Or attempting to delete one*
                Debug.Log(saves.savesList[(paneCurrent - 1) + (pageCurrent - 1) * 3]);

                saves.DeleteRoom(saves.savesList[(paneCurrent - 1) + (pageCurrent - 1) * 3]);
                
                windowCurrent = WindowFinder.MenuLoad;
                Confirm.SetActive(false);
                MenuSetup();

            }
            else if(uploadSaveFiles)
            {
                //Or uploading one to the online repo

                // RD TODO: Load Upload menu
                // Contians option to upload to your private account "website/Users/MD5Key/SavedRooms/"
                // Private: If not MD5 key known "Message saying why and how to fix" (Needs to launch from website at least once (Website link has the MD5Key embedded in the link and is passed as a Batch file command)
                // Public: Require internet connecton, check sserver staus etc, Save room file with yourNickname/RoomName/DateModified for uniqueness and so other users will have this info when they download the public room
                
                // Activate upload screen
                Upload.SetActive(true);

                // Save the filename wanting to be uploaded
                uploadFilename = saves.savesList[(paneCurrent - 1) + (pageCurrent - 1) * 3];

                // Disable the confirm screen
                Confirm.SetActive(false);
            }
            else
            {
                //Vanilla loading
                PlayerPrefs.SetInt("RoomSetup", 0);

                if (isEditable)
                {
                    //The edit mode has an additional entry in the save file lists
                    PlayerPrefs.SetInt("CurrentSave", (paneCurrent - 1) + (pageCurrent - 1) * 3);
                }

                SceneManager.LoadScene("Main_Scene", LoadSceneMode.Single);
            }
        }
        #endregion
    }

    public void SetToDelete()
    {
        #region Sets the save files to a delete-able state. Also, locks down the highlight over that icon so it always shows as highlighted
        if (!deleteSaveFiles)
        {
            deleteSaveFiles = true;
            uploadSaveFiles = false;
            HighlightDeleteHover.SetActive(true);
            HighlightDeleteHover.transform.position = DeleteButton.transform.position;
            HighlightUploadHover.SetActive(false);
        }
        else
        {
            deleteSaveFiles = false;
            HighlightDeleteHover.SetActive(false);
        }
        #endregion
    }

    public void SetToUpload()
    {
        #region Sets the save files to an uploadable state. Also, locks down the highlight over that icon so it always shows as highlighted
        if (!uploadSaveFiles)
        {
            deleteSaveFiles = false;
            uploadSaveFiles = true;
            HighlightUploadHover.SetActive(true);
            HighlightUploadHover.transform.position = UploadButton.transform.position;
            HighlightDeleteHover.SetActive(false);
        }
        else
        {
            uploadSaveFiles = false;
            HighlightUploadHover.SetActive(false);
        }
        #endregion
    }

    public void ConfirmUploadPrivate()
    {
        saves.UploadRoom(uploadFilename, true);
    }

    public void ConfirmUploadPublic()
    {
        saves.UploadRoom(uploadFilename, false);
    }

    #endregion

    #region Catalogue voids
    public void MenuSetup()
    {
        #region Determines the data displayed in the load game menus     

        //Hides display while loading menus
        MenuMain.SetActive(false);
        LoadRoom.SetActive(true);
        windowCurrent = WindowFinder.MenuLoad;

        //Creates a total list length based on number of existing saves (plus 1)
        //Includes all full pages, plus a page for the remainder
        listLength = saves.savesList.Count + 2;
        pageCount = listLength / 3;
        if (listLength % 3 > 0)
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
        #endregion
    }

    private void DisplayPageDetails()
    {
        #region Displays save data read from the save file list
        countText.text = pageCurrent.ToString() + " / " + pageCount.ToString();

        for (int i = 0; i <= 2; i++)
        {
            if (((pageCurrent - 1) * 3) + i == listLength - 2)
            {
                #region Populates the second-last entry on the list with the option to download a room from an online repository
                saveTexts[i].text = "Download room";
                saveDates[i].text = " ";
                saveNews[i].text = " ";
                #endregion
            }
            else if (((pageCurrent - 1) * 3) + i > (listLength - 2))
            {
                #region Populates the last entry on the list in edit mode with the option to create a new save
                if (((pageCurrent - 1) * 3) + i == (listLength - 1) && isEditable == true)
                {
                    saveTexts[i].text = "Create new room";
                    saveDates[i].text = "----/--/--";
                    saveNews[i].text = " ";
                }
                else
                {
                    //Debug.Log("Empty save");
                    saveTexts[i].text = "NO DATA";
                    saveDates[i].text = "----/--/--";
                    saveNews[i].text = " ";
                }
                #endregion
            }
            else
            {
                #region Displays existing save files
                //Debug.Log("Existing save");
                //Debug.Log(SaveTitle1);
                //Debug.Log(saveTexts);
                //Debug.Log(saveTexts[0]);
                // RD EDIT: Only show room name not all the other parts of the save filename
                saveTexts[i].text = saves.savesList[((pageCurrent - 1) * 3) + i].Split('.')[0];
                saveDates[i].text = saves.GetDate(saves.savesList[((pageCurrent - 1) * 3) + i]);
                saveNews[i].text = " ";
                #endregion
            }
            paneCurrent = 0;

        }
        #endregion
    }
    #endregion

    public void DisableHovers()
    {
        #region Disables all hovers while switching between window panes
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
        HighlightSaveBack.SetActive(false);
        HighlightSaveBackHover.SetActive(false);
        HighlightDelete.SetActive(false);
        HighlightDeleteHover.SetActive(false);
        //Also disables save file delete mode, and disables the text input field
        deleteSaveFiles = false;
        uploadSaveFiles = false;
        InputObject.SetActive(false);
        #endregion
    }
}
