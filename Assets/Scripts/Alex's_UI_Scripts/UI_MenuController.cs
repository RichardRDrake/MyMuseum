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

    [SerializeField] private GameObject Camera;
    private CamController CamController;

    //Determines whether it should return to build or view mode UI when dismissed
    public bool buildMode = true;

    //Object with saves
    [SerializeField] private GameObject SaveObject;
    private SaveLoadRoom Saves;

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
    private Image SaveLoadTitle;
    [SerializeField] private Sprite saveSprite;
    [SerializeField] private Sprite loadSprite;

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
    [SerializeField] private GameObject HighlightSave;
    [SerializeField] private GameObject HighlightSaveHover;
    [SerializeField] private GameObject HighlightSaveNavHover;

    //Int changed by UI_SaveLoad. Mimics functionality of paneCurrent without overwriting it
    public int saveLoadIdentity = 0;

    //Int determining which save file is currently selected
    //Begins at 0
    private int saveFileSelected;

    //Records the last save file accessed, in order to upload the current room to the online repo when this option is selected
    public int lastSaved;
    #endregion

    #region Confirm contents
    [SerializeField] private GameObject Confirm;
    [SerializeField] private GameObject ConfirmField;
    private TextMeshProUGUI ConfirmText;

    [SerializeField] private GameObject YesButton;
    //Text object child of the yes button
    [SerializeField] private GameObject YesChild;
    private TextMeshProUGUI ChildText;
    [SerializeField] private GameObject NoButton;
    private List<Vector3> ConfirmButtonLocations = new List<Vector3>();

    [SerializeField] private GameObject HighlightConfirm;
    [SerializeField] private GameObject HighlightConfirmHover;

    [SerializeField] private GameObject InputObject;
    private TMP_InputField inputField;

    //Determines whether the user has confirmed they wish to save
    //Used to bring up the text input
    private bool isSaving = false;
    #endregion

    #region Options contents
    //Options display highlights
    private UI_Options uiOptions;
    [SerializeField] private GameObject SlidersParent;
    [SerializeField] private GameObject HighlightOptionsMusic;
    [SerializeField] private GameObject HighlightOptionsMusicHover;
    [SerializeField] private GameObject HighlightOptionsSfx;
    [SerializeField] private GameObject HighlightOptionsSfxHover;
    [SerializeField] private GameObject HighlightOptionsCloseHover;

    [SerializeField] private GameObject MusicSliderObject;
    private Slider musicSlider;
    [SerializeField] private GameObject SfxSliderObject;
    private Slider sfxSlider;
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
    public enum OptionsFinder { Null = 0, Music = 1, SFX = 2}
    public OptionsFinder optionsCurrent = OptionsFinder.Null;

    //Tracks which object in the confirm window the user is on
    public enum ConfirmFinder { Null = 0, Confirm = 1, Cancel = 2, Text = 3}
    public ConfirmFinder confirmCurrent = ConfirmFinder.Null;
    #endregion

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        #region set variables
        //UI controller script attached to build menu
        UI_Controller = BuildMenu.GetComponent<UI_Controller>();
        UI_ViewController = ViewMenu.GetComponent<UI_ViewController>();
        CamController = Camera.GetComponent<CamController>();
        Saves = SaveObject.GetComponent<SaveLoadRoom>();
        if(Saves != null)
        {
             Debug.Log(Saves.savesList.Count);
        }
        else 
        {
            Debug.Log("Saves broken AGAIN");
        }

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
        SaveLoadTitle = SaveLoadText.GetComponent<Image>();

        //Sets the locations the highlights can occupy
        MainLocations.Add(SaveButton.transform.position);
        MainLocations.Add(LoadButton.transform.position);
        MainLocations.Add(OptionsButton.transform.position);
        MainLocations.Add(QuitButton.transform.position); 

        SaveLoadLocations.Add(SaveLoad1.transform.position);
        SaveLoadLocations.Add(SaveLoad2.transform.position);
        SaveLoadLocations.Add(SaveLoad3.transform.position);

        ConfirmButtonLocations.Add(YesButton.transform.position);
        ConfirmButtonLocations.Add(NoButton.transform.position);

        //Gets the text component of the save/load confirm menus
        ConfirmText = ConfirmField.GetComponent<TextMeshProUGUI>();
        ChildText = YesChild.GetComponent<TextMeshProUGUI>();

        //Gets the input field from the confirm menu
        inputField = InputObject.GetComponent<TMP_InputField>();

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

        //Sets lastSaved to the value stored upon leaving the main menu
        if (PlayerPrefs.HasKey("CurrentSave"))
        {
            lastSaved = PlayerPrefs.GetInt("CurrentSave");
        }
        else
        {
            Debug.Log("Failed to load room identity");
        }
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        //Moves the player back up a menu level from wherever they were.
        if (Input.GetKeyDown("escape"))
        {
            NavigateUp();
        }
        //Ensures the backspace button won't create save file naming issues
        else if (Input.GetKeyDown("backspace") && confirmCurrent != ConfirmFinder.Text)
        {
            NavigateUp();
        }

        //Determines what to do if the player hits enter or space
        if (Input.GetKeyDown("enter") || Input.GetKeyDown("return"))
        {
            NavigateDown();
        }
        //Ensures the space button won't create save file naming issues
        else if (Input.GetKeyDown("space") && confirmCurrent != ConfirmFinder.Text)
        {
            NavigateDown();
        }

        //Pertaining to use of arrow keys to change pages in catalogue
        //Or the values of sliders
        #region Catalogue Scrolling
        if (Input.GetKeyDown("left") && windowCurrent == WindowFinder.MenuSaveLoad)
        {
            DecrementPage();
        }
        //Or adjust volume sliders
        else if (Input.GetKey("left") && windowCurrent == WindowFinder.MenuOptions)
        {
            if(optionsCurrent == OptionsFinder.Music)
            {
                musicSlider.value -= 0.6f * Time.deltaTime;
            }
            else if(optionsCurrent == OptionsFinder.SFX)
            {
                sfxSlider.value -= 0.6f * Time.deltaTime;
            }
        }
        //Catalogue again
        if (Input.GetKeyDown("right") && windowCurrent == WindowFinder.MenuSaveLoad)
        {
            IncrementPage();
        }
        //And volume sliders
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

    }

    #region Catalogue page voids
    public void IncrementPage()
    {
        pageCurrent++;
        pageCurrent = Mathf.Clamp(pageCurrent, 1, pageCount);
        ObjectsHide.SetActive(false);
        DisplayPageDetails();
        ObjectsHide.SetActive(true);
        //Debug.Log("Current page is: " + pageCurrent.ToString() + ". Max page is: " + pageCount.ToString() + ".");
        //Sets highlighted pane to 0
        HighlightSave.SetActive(false);
        paneCurrent = 0;
    }

    public void DecrementPage()
    {
        pageCurrent--;
        pageCurrent = Mathf.Clamp(paneCurrent, 1, pageCount);
        ObjectsHide.SetActive(false);
        DisplayPageDetails();
        ObjectsHide.SetActive(true);
        //Debug.Log("Current page is: " + pageCurrent.ToString() + ". Max page is: " + pageCount.ToString() + ".");
        //Sets highlighted pane to 0
        HighlightSave.SetActive(false);
        paneCurrent = 0;
    }
    #endregion

    public void Activate()
    {
        //Sets other UI menus inactive once activated
        BuildMenu.SetActive(false);
        mainCurrent = MainFinder.Null;
        HighlightTop.SetActive(false);
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
            if(windowInt == 3)
            {
                //Gets BGM/SFX script, so its values can be saved when that menu is closed
                uiOptions = SlidersParent.GetComponent<UI_Options>();

                PlayerPrefs.SetFloat("BGM", uiOptions.musicVolume);
                Debug.Log("BGM set to " + uiOptions.musicVolume);
                PlayerPrefs.SetFloat("SFX", uiOptions.sfxVolume);
            }
            windowCurrent = (WindowFinder)1;
            DisableSubmenus();
            Main.SetActive(true);
        }
        else if (windowInt == 4)
        {
            //Handles navigation from a confirmation popup in the save or load menus
            windowCurrent = (WindowFinder)2;
            DisableConfirmation();
            HighlightConfirm.SetActive(false);
            HighlightConfirmHover.SetActive(false);
            saveLoadIdentity = 0;
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
                Cursor.visible = false;
                UI_ViewController.DisableMain();
            }
            CamController.canHotkey = true;
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
                        MenuSetup();
                        break;
                    case 2:
                        saveOrLoad = true;
                        MenuSetup();
                        break;
                    case 3:
                        OptionsSetup();
                        break;
                    case 4:
                        break;
                    default:
                        break;
                }
                break;
            case 2:
                ShowConfirmation();
                break;
            case 4:
                int confirmInt = (int)confirmCurrent;
                if(confirmInt == 1)
                {
                    //Write to appropriate save file
                    ConfirmSave();
                }
                else if (confirmInt == 2)
                {
                    //Disables popup
                    windowCurrent = (WindowFinder)2;
                    DisableConfirmation();
                    HighlightConfirm.SetActive(false);
                    HighlightConfirmHover.SetActive(false);
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
        HighlightSave.SetActive(false);
        HighlightSaveNavHover.SetActive(false);

        Options.SetActive(false);
        optionsCurrent = OptionsFinder.Null;
        HighlightOptionsMusic.SetActive(false);
        HighlightOptionsMusicHover.SetActive(false);
        HighlightOptionsSfx.SetActive(false);
        HighlightOptionsSfxHover.SetActive(false);
        HighlightOptionsCloseHover.SetActive(false);

        confirmCurrent = ConfirmFinder.Null;
        HighlightConfirm.SetActive(false);
        HighlightConfirmHover.SetActive(false);
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

    public void ShowConfirmation()
    {
        //Displays confirm menu for save/load data
        #region show confirm menu

        //Resets saving variables
        inputField.text = "";
        InputObject.SetActive(false);
        ChildText.text = "Confirm";
        isSaving = false;
        windowCurrent = WindowFinder.Confirm;
        confirmCurrent = ConfirmFinder.Null;
        HighlightConfirm.SetActive(false);
        HighlightConfirmHover.SetActive(false);
        Confirm.SetActive(true);

        //if accessed by UI_SaveLoad
        if (saveLoadIdentity != 0)
        {
            if (saveOrLoad == true)
            {
                if (saveLoadIdentity + ((pageCurrent - 1) * 3) < listLength)
                {
                    ConfirmText.text = "Unsaved progress will be lost. Continue?";
                    saveFileSelected = saveLoadIdentity - 1 + ((pageCurrent - 1) * 3);
                }
            }
            else
            {
                if (saveLoadIdentity + ((pageCurrent - 1) * 3) >= listLength)
                {
                    ConfirmText.text = "Create new save file?";

                    saveFileSelected = listLength - 1;
                }
                else
                {
                    ConfirmText.text = "Overwrite save file?";
                    saveFileSelected = saveLoadIdentity - 1 + ((pageCurrent - 1) * 3);
                }
            }
        }
        else
        {
            //Ensures a valid save file is always selected
            if (paneCurrent == 0)
            {
                paneCurrent = 1;
            }
            if (saveOrLoad == true)
            {
                if (paneCurrent + ((pageCurrent - 1) * 3) < listLength)
                {
                    ConfirmText.text = "Unsaved progress will be lost. Continue?";
                    saveFileSelected = paneCurrent - 1 + ((pageCurrent - 1) * 3);
                }
            }
            else
            {
                if (paneCurrent + ((pageCurrent - 1) * 3) >= listLength)
                {
                    ConfirmText.text = "Create new save file?";
                    saveFileSelected = listLength - 1;
                }
                else
                {
                    ConfirmText.text = "Overwrite save file?";
                    saveFileSelected = paneCurrent - 1 + ((pageCurrent - 1) * 3);
                }
            }
        }
        //saveLoadIdentity = 0;
        #endregion
    }

    public void DisableConfirmation()
    {
        Confirm.SetActive(false);
        windowCurrent = WindowFinder.MenuSaveLoad;
    }

    public void MenuSetup()
    {
        //Loads details for save file display
        #region sets up saves display
        //Hides display while loading data
        SaveLoad.SetActive(true);
        ObjectsHide.SetActive(false);

        //Titles the page based on the appropriate function
        if (saveOrLoad == false)
        {
            SaveLoadTitle.sprite = saveSprite;
        }
        else
        {
            SaveLoadTitle.sprite = loadSprite;
        }

        //Creates a total list length based on number of existing saves
        //Includes all full pages, plus a page for the remainder
        listLength = Saves.savesList.Count + 1;
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

        windowCurrent = WindowFinder.MenuSaveLoad;

        #endregion
    }

    public void OptionsSetup()
    {
        windowCurrent = WindowFinder.MenuOptions;
        Options.SetActive(true);
    }

    private void DisplayPageDetails()
    {
        //Displays save data read from the save file list
        #region display save data per page
        countText.text = pageCurrent.ToString() + " / " + pageCount.ToString();
        //If saving
        if (saveOrLoad == false)
        {
            for (int i = 0; i <= 2; i++)
            {
                if (((pageCurrent - 1) * 3) + i == listLength - 1)
                {
                    //This is separate for future use
                    //Allows the first empty save slot to display different detail
                    //Or the others to be hidden
                    //Debug.Log("New save");
                    saveTexts[i].text = "NO DATA";
                    saveDates[i].text = "----/--/--";
                    saveNews[i].text = " ";
                }
                else if (((pageCurrent - 1) * 3) + i > (listLength - 2))
                {
                    //Debug.Log("Empty save");
                    saveTexts[i].text = "NO DATA";
                    saveDates[i].text = "----/--/--";
                    saveNews[i].text = " ";
                }
                else
                {
                    //Debug.Log("Existing save");
                    //Debug.Log(SaveTitle1);
                    //Debug.Log(saveTexts);
                    //Debug.Log(saveTexts[0]);
                    saveTexts[i].text = Saves.savesList[((pageCurrent - 1) * 3) + i];
                    saveDates[i].text = Saves.GetDate(saveTexts[i].text);
                    saveNews[i].text = " ";
                }
                paneCurrent = 0;

            }
        }
        //if loading
        else
        {

            for (int i = 0; i <= 2; i++)
            {
                if (((pageCurrent - 1) * 3) + i > (listLength - 2))
                {
                    //Debug.Log("Empty save");
                    saveTexts[i].text = "NO DATA";
                    saveDates[i].text = "----/--/--";
                    saveNews[i].text = " ";
                }
                else
                {
                    //Debug.Log("Existing save");
                    //Debug.Log(SaveTitle1);
                    //Debug.Log(saveTexts);
                    //Debug.Log(saveTexts[0]);
                    saveTexts[i].text = Saves.savesList[i];
                    saveDates[i].text = Saves.GetDate(saveTexts[i].text);
                    saveNews[i].text = " ";
                }
                paneCurrent = 0;

            }
        }
        #endregion
    }

    private void CycleThrough()
    {
        //Cycles forward through options in currently active window
        #region forward cycle
        int windowInt = (int)windowCurrent;
        switch (windowInt)
        {
            case 1:
                //Cycles through the enum of available main menu options
                int mainInt = (int)mainCurrent + 1;
                //Checks this isn't out of range
                if(mainInt > 4)
                {
                    mainInt = 1;
                }
                mainCurrent = (MainFinder)mainInt;
                HighlightTop.SetActive(true);
                HighlightTop.transform.position = MainLocations[mainInt - 1];
                break;
            case 2:
                //Cycles through the enum of available save files per page
                paneCurrent++;
                if (saveOrLoad == true)
                {
                    if (paneCurrent > 3 || ((pageCurrent - 1) * 3) + paneCurrent > (listLength - 1))
                    {
                        paneCurrent = 1;
                    }
                }
                else
                {
                    if (paneCurrent > 3 || ((pageCurrent - 1) * 3) + paneCurrent > listLength)
                    {
                        paneCurrent = 1;
                    }
                }
                HighlightSave.SetActive(true);
                HighlightSave.transform.position = SaveLoadLocations[paneCurrent - 1];
                break;
            case 3:
                //Cycles through the enum of options on the options page
                int optionsInt = (int)optionsCurrent + 1;
                //checks this isn't out of range
                if(optionsInt > 2)
                {
                    optionsInt = 1;
                }
                //Sets highlight accordingly
                if(optionsInt == 1)
                {
                    HighlightOptionsMusic.SetActive(true);
                    HighlightOptionsSfx.SetActive(false);
                }
                else
                {
                    HighlightOptionsMusic.SetActive(false);
                    HighlightOptionsSfx.SetActive(true);
                }
                optionsCurrent = (OptionsFinder)optionsInt;
                break;
            case 4:
                //Cycles through options on the confirm page
                int confirmInt = (int)confirmCurrent + 1;
                //Checks whether text can be input
                if (isSaving == false)
                {
                    //If it isn't, assumes the text field to be out of range
                    if (confirmInt > 2)
                    {
                        confirmInt = 1;
                    }
                    //Sets highlight over appropriate pane
                    confirmCurrent = (ConfirmFinder)confirmInt;
                    HighlightConfirm.transform.position = ConfirmButtonLocations[confirmInt - 1];
                    HighlightConfirm.SetActive(true);
                }
                else
                {
                    //Same as above, but now the text field is in range
                    if (confirmInt > 3)
                    {
                        confirmInt = 1;
                    }
                    confirmCurrent = (ConfirmFinder)confirmInt;
                    //Sets highlight over appropriate pane, if the text field is not selected
                    if (confirmInt < 3)
                    {
                        HighlightConfirm.transform.position = ConfirmButtonLocations[confirmInt - 1];
                        HighlightConfirm.SetActive(true);
                        inputField.DeactivateInputField();
                    }
                    //If the text field is selected, allows for input
                    else
                    {
                        HighlightConfirm.SetActive(false);
                        inputField.ActivateInputField();
                    }

                }
                break;
        }
        #endregion
    }

    private void CycleBack()
    {
        //Cycles backwards through options in currently selected window
        #region Backward cycle
        int windowInt = (int)windowCurrent;
        switch (windowInt)
        {
            case 1:
                //Cycles through the enum of available main menu options
                int mainInt = (int)mainCurrent - 1;
                //Checks this isn't out of range
                if (mainInt < 1)
                {
                    mainInt = 4;
                }
                mainCurrent = (MainFinder)mainInt;
                HighlightTop.SetActive(true);
                HighlightTop.transform.position = MainLocations[mainInt - 1];
                break;
            case 2:
                paneCurrent--;
                if(paneCurrent < 1)
                {
                    paneCurrent = 3;
                }
                //Checks that the next pane isn't less than 1, or empty
                if (saveOrLoad == true)
                {
                    //Checks that the last option isn't out of range
                    if (((pageCurrent - 1) * 3) + (paneCurrent) > (listLength - 1))
                    {
                        //If it is, cycles to the last populated list entry if there's less than one full page
                        if (pageCount - 1 < 1)
                        {
                            paneCurrent = listLength - 1;
                        }
                        else
                        {
                            //or one higher than the remainder of the asset list divided by the number of full pages
                            paneCurrent = ((listLength - 1) % ((pageCurrent - 1) * 3));
                        }
                    }
                }
                else
                {
                    //Checks that the last option isn't out of range
                    if (((pageCurrent - 1) * 3) + (paneCurrent) > listLength)
                    {
                        //If it is, cycles to the last populated list entry if there's less than one full page
                        if (pageCount - 1 < 1)
                        {
                            paneCurrent = listLength;
                        }
                        else
                        {
                            //or one higher than the remainder of the asset list divided by the number of full pages
                            paneCurrent = ((listLength - 1) % ((pageCount - 1) * 3));
                        }
                    }
                    //Debug.Log(paneCurrent);
                }
                //Moves the highlight over the appropriate pane
                if (paneCurrent != 0)
                {
                    HighlightSave.SetActive(true);
                    HighlightSave.transform.position = SaveLoadLocations[paneCurrent - 1];
                }
                else
                {
                    //Or disables it, if there's no saves
                    HighlightSave.SetActive(false);
                }
                break;
            case 3:
                //Cycles through the enum of options on the options page
                int optionsInt = (int)optionsCurrent - 1;
                //checks this isn't out of range
                if (optionsInt < 1)
                {
                    optionsInt = 2;
                }
                //Sets highlight accordingly
                if (optionsInt == 1)
                {
                    HighlightOptionsMusic.SetActive(true);
                    HighlightOptionsSfx.SetActive(false);
                }
                else
                {
                    HighlightOptionsMusic.SetActive(false);
                    HighlightOptionsSfx.SetActive(true);
                }
                optionsCurrent = (OptionsFinder)optionsInt;
                break;
            case 4:
                //Cycles through options on the confirm page
                int confirmInt = (int)confirmCurrent - 1;
                //checks this isn't out of range
                if (confirmInt < 1)
                {
                    confirmInt = 2;
                }
                //Sets highlight over appropriate pane
                confirmCurrent = (ConfirmFinder)confirmInt;
                HighlightConfirm.transform.position = ConfirmButtonLocations[confirmInt - 1];
                HighlightConfirm.SetActive(true);
                break;
        }
        #endregion
    }

    #region button voids
    //Used by the save button
    public void SavePressed()
    {
        saveOrLoad = false;
        DisableMainMenu();
        MenuSetup();
    }

    //Used by the load button
    public void LoadPressed()
    {
        saveOrLoad = true;
        DisableMainMenu();
        MenuSetup();
    }

    //Used by the options button
    public void OptionsPressed()
    {
        DisableMainMenu();
        OptionsSetup();
    }

    //Used by the confirm save button
    public void ConfirmSave()
    {
        //if the game is set to save
        if(saveOrLoad==false)
        {
            if (!isSaving)
            {
                InputObject.SetActive(true);
                inputField.ActivateInputField();
                ChildText.text = "Save";
                isSaving = true;
                ConfirmText.text = "Name your room";
                confirmCurrent = ConfirmFinder.Text;
                HighlightConfirm.SetActive(false);
            }
            else
            {
                inputField = InputObject.GetComponent<TMP_InputField>();
                if (inputField.text != "")
                {
                    Debug.Log(saveFileSelected);
                    windowCurrent = (WindowFinder)1;
                    Saves.Save(inputField.text);
                    DisableSubmenus();
                    Confirm.SetActive(false);
                    Main.SetActive(true);
                    isSaving = false;
                }
            }
        }
        //if the game is set to load
        else
        {
            if (saveLoadIdentity != 0)
            {
                paneCurrent = saveLoadIdentity;
            }
                if (saveFileSelected < listLength)
                {
                    Debug.Log(paneCurrent);
                    Debug.Log(paneCurrent - 1 + (pageCurrent - 1) * 3);
                    windowCurrent = (WindowFinder)1;
                    Saves.Load(saveTexts[(paneCurrent - 1) + (pageCurrent - 1) * 3].text);
                    Debug.Log(saveTexts[(paneCurrent - 1) + (pageCurrent - 1) * 3].text);
                    lastSaved = (paneCurrent - 1) + (pageCurrent - 1) * 3;
                    DisableSubmenus();
                    Confirm.SetActive(false);
                    Main.SetActive(true);
                }
            saveLoadIdentity = 0;
        }
    }
    #endregion
}
