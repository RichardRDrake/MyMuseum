using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;


public class UI_Controller : MonoBehaviour
{
    //Script controlling the build menu
    #region Initialising variables

    //Used for first-time initialisation
    bool RoomSetup = false;

    #region GameObjects and relevant components
    //Main menu
    [SerializeField] private GameObject MainMenu;
    private UI_MenuController UI_MenuController;

    //Window showing main menu options
    [SerializeField] private GameObject HudDefault;

    //Window showing asset categories
    [SerializeField] private GameObject ArtefactCategories;

    //Window showing build categories
    [SerializeField] private GameObject BuildCategories;

    //Window showing list of placeable objects
    [SerializeField] private GameObject AssetRepository;

    //Panel for detailed view/confirming selection
    [SerializeField] private GameObject DetailPanel;
    [SerializeField] private GameObject DetailImageField;
    private Image detailImage;
    private int detailImageCounter = 0;

    //Camera object
    [SerializeField] private GameObject Camera;
    private CamController camController;

    //Asset placer
    [SerializeField] private AssetPlacer assetPlacer = null;

    //Object containing 

    #region submenu objects
    //List of buttons relating to each category
    //(This form of sorting is for testing's sake and is definitely not considered final)
    [SerializeField] private GameObject FloorBased;
    [SerializeField] private GameObject FloorOrWall;
    [SerializeField] private GameObject Small;
    [SerializeField] private GameObject Planar;
    [SerializeField] private GameObject Room;
    [SerializeField] private GameObject Plinth;
    [SerializeField] private GameObject Stand;
    [SerializeField] private GameObject Frame;
    private List<Vector3> menuLocationList = new List<Vector3>();
    #endregion

    #region catalogue objects
    //The six objects, and a list in which to contain their locations
    private List<Image> objectDisplay = new List<Image>();
    [SerializeField] private GameObject Object1;
    [SerializeField] private GameObject Object2;
    [SerializeField] private GameObject Object3;
    [SerializeField] private GameObject Object4;
    [SerializeField] private GameObject Object5;
    [SerializeField] private GameObject Object6;
    private List<Vector3> panelLocationList = new List<Vector3>();
    #endregion

    #region detail objects
    //The detail rotate buttons, and a list in which to contain their locations
    [SerializeField] private GameObject RotateLeft;
    [SerializeField] private GameObject RotateRight;
    [SerializeField] private GameObject ConfirmButton;
    private List<Vector3> rotateLocationList = new List<Vector3>();
    #endregion

    //Visual representation of the objects themselves
    //So they can be hidden when loading a new page
    [SerializeField] private GameObject ObjectsHide;

    //Page number object and text
    [SerializeField] private GameObject PageCounter;
    private TextMeshProUGUI countText;

    //menu highlight icons
    [SerializeField] private GameObject HighlightMenuTop;
    [SerializeField] private GameObject HighlightMainMenu;
    [SerializeField] private GameObject HighlightCatalogue;
    [SerializeField] private GameObject HighlightDetail;
    [SerializeField] private GameObject HighlightAccept;
    [SerializeField] private GameObject HighlightFirst;

    //hover highlight icons
    //Note that these are here to be disabled, not utilised
    [SerializeField] private GameObject HighlightMenuTopHover;
    [SerializeField] private GameObject HighlightCatalogueHover;
    #endregion

    #region Script containing list of menu-specific placeable objects
    private TempListScript Resources;
    int listLength;

    //Used to determine which list to read from
    public int switchLists = 0;
    #endregion

    #region Menu management variables
    //Signals whether a menu needs to be displayed
    public bool displayMenu = false;

    //This determines whether the script should read from the artefact or build objects repositories
    //true = artefact, false = build
    private bool isArtefact = false;

    //For tracking position in object catalogues
    int pageCount;
    int pageCurrent = 1;
    int pageNumber;


    #endregion

    #region menu enums
    //Determines which UI page the user is on.
    //More will be added as appropriate
    public enum windowFinder { Menu_Top = 1, Menu_Sub = 2, Catalogue = 3, Detail = 4 };
    public windowFinder windowCurrent = windowFinder.Menu_Top;

    //Determines which main menu option the user is on.
    public enum topFinder { Null = 0, Artefact = 1, Build = 2, Move = 3, Destroy = 4, Main = 5, First = 6 };
    public topFinder topCurrent = topFinder.Null;

    //Determines which submenu the user is on.
    public enum subFinder { Null = 0, FloorRoom = 1, MiscPlinth = 2, SmallFrames = 3, PlanarStands = 4};
    public subFinder subCurrent = subFinder.Null;

    //Determines which catalogue pane the user is on.
    public int paneCurrent;

    //Determines which detail options the user is on.
    public enum detailFinder { Null = 0, RotateC = 1, RotateAC = 2, Select = 3};
    public detailFinder detailCurrent = detailFinder.Null;
    #endregion

    #region Audio
    private AudioManager audioManager;

    [SerializeField] private AudioMixer bgmMixer;
    [SerializeField] private AudioMixer sfxMixer;
    #endregion

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        #region Set variables

        #region Finds instances of necessary GameObjects
        //Finds the main menu's associated controller
        UI_MenuController = MainMenu.GetComponent<UI_MenuController>();

        //Gets the six inventory panes, and a resource list
        objectDisplay.Add(Object1.GetComponent<Image>());
        objectDisplay.Add(Object2.GetComponent<Image>());
        objectDisplay.Add(Object3.GetComponent<Image>());
        objectDisplay.Add(Object4.GetComponent<Image>());
        objectDisplay.Add(Object5.GetComponent<Image>());
        objectDisplay.Add(Object6.GetComponent<Image>());
        countText = PageCounter.GetComponent<TextMeshProUGUI>();
        if(!countText)
        {
            Debug.Log("countText is broken (again)");
        }

        //This debug TempListScript is NECESSARY. 
        Resources = GetComponent<TempListScript>();
        if (!Resources)
        {
            Debug.Log("Resources - Something went wrong");
        }

        //Gets the camera controller
        camController = Camera.GetComponent<CamController>();

        //Gets the (hopefully) single assetplacer in the scene
        assetPlacer = FindObjectOfType<AssetPlacer>();
        if (assetPlacer == null)
        {
            Debug.LogError("no assetPlacer present in scene");
        }
        #endregion

        #region Sets possible highlight positions
        //Sets the locations the highlights can occupy
        menuLocationList.Add(FloorBased.transform.position);
        menuLocationList.Add(FloorOrWall.transform.position);
        menuLocationList.Add(Small.transform.position);
        menuLocationList.Add(Planar.transform.position);

        panelLocationList.Add(Object1.transform.position);
        panelLocationList.Add(Object2.transform.position);
        panelLocationList.Add(Object3.transform.position);
        panelLocationList.Add(Object4.transform.position);
        panelLocationList.Add(Object5.transform.position);
        panelLocationList.Add(Object6.transform.position);

        rotateLocationList.Add(RotateLeft.transform.position);
        rotateLocationList.Add(RotateRight.transform.position);
        #endregion

        #region First-time setup variables
        //Checks whether the currently loaded scene has a loaded room prefab
        if (PlayerPrefs.HasKey("RoomSetup"))
        {
            if(PlayerPrefs.GetInt("RoomSetup") == 1)
            {
                RoomSetup = true;
                RoomInitialisation();
            }
        }
        else
        {
            Debug.Log("RoomSetup not initialised when scene was loaded.");
        }
        #endregion

        #region Audio setup
        //Gets the AudioManager values for SFX and BGM volume
        //Then sets the slider starting values to either the value in PlayerPrefs or, if those do not exist, 0.8f
        audioManager = FindObjectOfType<AudioManager>();
        if (PlayerPrefs.HasKey("BGM"))
        {
            bgmMixer.SetFloat("bgmVol", Mathf.Log10(PlayerPrefs.GetFloat("BGM")) * 20);
            sfxMixer.SetFloat("sfxVol", Mathf.Log10(PlayerPrefs.GetFloat("SFX")) * 20);
            Debug.Log("Playerprefs found");
        }
        else
        {
            bgmMixer.SetFloat("bgmVol", Mathf.Log10(0.8f) * 20);
            sfxMixer.SetFloat("sfxVol", Mathf.Log10(0.8f) * 20);
            Debug.Log("Playerprefs not found");
        }
        audioManager.Play("Default_BGM");
      //  audioManager.Play("Title_BGM");
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

        //Determines what to do if the player hits enter or space
        if (Input.GetKeyDown("space") || Input.GetKeyDown("enter") || Input.GetKeyDown("return"))
        {
            NavigateDown();
        }

        //If the UI controller recieves a signal from any artefact category button
        if (displayMenu == true)
        {
            displayMenu = false;
            MenuSetup();
        }

        //Determines what to do if the player hits tab or down
        if (Input.GetKeyDown("tab") || Input.GetKeyDown("down"))
        {
            CycleThrough();
        }

        //Determines what to do if the player hits left
        if (Input.GetKeyDown("up"))
        {
            CycleBack();
        }
        #endregion

        #region catalogue scrolling
        //Pertaining to use of arrow keys to change pages in catalogue
        if (Input.GetKeyDown("left"))
        {
            if (windowCurrent == windowFinder.Catalogue)
            {
                DecrementPage();
            }
            else if (windowCurrent == windowFinder.Detail)
            {
                DetailBack();
            }
        }
        
        if(Input.GetKeyDown("right"))
        {
            if (windowCurrent == windowFinder.Catalogue)
            {
                IncrementPage();
            }
            else if (windowCurrent == windowFinder.Detail)
            {
                DetailCycle();
            }
        }
        #endregion
    }

    #region Menu navigation
    public void NavigateDown()
    {
        #region Move down the Hierarchy
        int windowInt = (int)windowCurrent;
        switch (windowInt)
        {
            case 1:
                #region If the player is on the top level menu
                //Switches based on the option selected
                int topInt = (int)topCurrent;
                switch (topInt)
                {
                    case 1:
                        DefaultToArtefacts();
                        break;
                    case 2:
                        DefaultToBuild();
                        break;
                    case 3:
                        //This should link to the move object function in future
                        break;
                    case 4:
                        //This should link to the delete object function in future
                        break;
                    case 5:
                        SwitchToMain();
                        break;
                    case 6:
                        camController.switchToFirstPerson();
                        HighlightFirst.SetActive(false);
                        break;
                }
                break;
                #endregion
            case 2:
                #region If the player is on a submenu
                //Sets MenuSetup to read from the selected submenu's contents
                //If a submenu is not selected, ignores this function
                int subInt = (int)subCurrent;
                switchLists = subInt - 1;
                if (switchLists >= 0)
                {
                    MenuSetup();
                }
                break;
                #endregion
            case 3:
                #region If the player is in the catalogue
                if (paneCurrent > 0)
                {
                    DetailPanel.SetActive(true);
                    detailImage = DetailImageField.GetComponent<Image>();
                    //Displays "Download" if the player presses on an icon beyond the length of the asset list
                    if (((pageCurrent - 1) * 6) + paneCurrent > Resources.readFrom.Count)
                    {
                        detailImage.sprite = null;
                    }
                    else
                    {
                        detailImage.sprite = Sprite.Create(Resources.readFrom[((pageCurrent - 1) * 6) + (paneCurrent - 1)].PreviewImages[0], new Rect(0.0f, 0.0f, Resources.readFrom[((pageCurrent - 1) * 6) + (paneCurrent - 1)].PreviewImages[0].width, Resources.readFrom[((pageCurrent - 1) * 6) + (paneCurrent - 1)].PreviewImages[0].height), new Vector2(0.0f, 0.0f));
                    }
                    windowCurrent = windowFinder.Detail;
                }
                break;
                #endregion
            case 4:
                #region If the player is in the detail pane
                int detailInt = (int)detailCurrent;
                switch (detailInt)
                {
                    case 1:
                        DetailCycle();
                        break;
                    case 2:
                        DetailBack();
                        break;
                    case 3:
                        if (RoomSetup == false)
                        {
                            SendToAssetPlacer();
                        }
                        else
                        {
                            //Places the new room
                            RoomSetup = false;
                        }
                        break;
                    default:
                        break;
                }
                break;
                #endregion
            default:
                break;
        }
        #endregion
    }

    public void NavigateUp()
    {
        #region Move up the Hierarchy
        //determine which menu layer the player is on
        int windowInt = (int)windowCurrent;
        if (windowInt >= 2 && windowInt <= 4)
        {
            windowInt--;
            switch (windowInt)
            {
                case 1:
                    #region Moves the player up to the top layer
                    HideAllArtefact();
                    ResetHighlight();
                    windowCurrent = (windowFinder)windowInt;
                    break;
                    #endregion
                case 2:
                    #region Moves the player up from the catalogue to the submenu
                    if (RoomSetup == false)
                    {
                        AssetRepository.SetActive(false);
                        paneCurrent = 0;
                        HighlightCatalogue.SetActive(false);
                        windowCurrent = (windowFinder)windowInt;
                    }
                    break;
                    #endregion
                case 3:
                    #region Moves the player up from the detail pane to the catalogue
                    DetailPanel.SetActive(false);
                    detailImageCounter = 0;
                    HighlightDetail.SetActive(false);
                    HighlightAccept.SetActive(false);
                    detailCurrent = detailFinder.Null;
                    windowCurrent = (windowFinder)windowInt;
                    break;
                    #endregion
                default:
                    break;
            }
        }
        #region Switches menu type from the build interface to the pause menu
        else if (windowInt == 1)
        {
            SwitchToMain();
        }
        #endregion
        #endregion
    }

    private void CycleThrough()
    {
        #region Cycles forward through options in the currently displayed window
        int windowInt = (int)windowCurrent;
        switch (windowInt)
        {
            case 1:
                #region Cycles through the enum of available menu options
                int topInt = (int)topCurrent + 1;
                //If it goes beyond the limit of this enum, resets to the start.
                if (topInt > 6)
                {
                    topInt = 1;
                }
                topCurrent = (topFinder)topInt;
                //If it's within the list but not the main menu, moves the highlight over the appropriate icon
                if (topInt < 5)
                {
                    HighlightFirst.SetActive(false);
                    HighlightMenuTop.SetActive(true);
                    HighlightMenuTop.transform.position = menuLocationList[topInt - 1];
                }
                //Otherwise, hides the generic highlight and puts a unique highlight over the main menu icon
                else if (topInt == 5)
                {
                    HighlightMenuTop.SetActive(false);
                    HighlightMainMenu.SetActive(true);
                }
                //The last option is the first person camera
                else
                {
                    HighlightMainMenu.SetActive(false);
                    HighlightFirst.SetActive(true);
                }
                break;
                #endregion
            case 2:
                #region Much the same as the high-level menu above, only without the unique highlight.
                int subInt = (int)subCurrent + 1;
                if (subInt > 4)
                {
                    subInt = 1;
                }
                subCurrent = (subFinder)subInt;
                HighlightMenuTop.SetActive(true);
                HighlightMenuTop.transform.position = menuLocationList[subInt - 1];
                break;
                #endregion
            case 3:
                #region Tabs backwards through panes in the current catalogue page
                paneCurrent++;
                //Checks that the next pane cycled to isn't greater than 6, or empty
                if (paneCurrent > 6 || ((pageCurrent - 1) * 6) + (paneCurrent - 1) > Resources.readFrom.Count)
                {
                    paneCurrent = 1;
                }
                //Moves the highlight over the appropriate pane
                HighlightCatalogue.SetActive(true);
                HighlightCatalogue.transform.position = panelLocationList[paneCurrent - 1];
                break;
                #endregion
            case 4:
                #region Cycles through options in the detail menu
                //Similar to the top level menu.
                //Int 1 or 2 correlates to the rotation buttons
                //If it's 3, it correlates to the accept button
                int detailInt = (int)detailCurrent + 1;
                if (detailInt > 3)
                {
                    detailInt = 1;
                }
                detailCurrent = (detailFinder)detailInt;
                if (detailInt < 3)
                {
                    HighlightAccept.SetActive(false);
                    HighlightDetail.SetActive(true);
                    HighlightDetail.transform.position = rotateLocationList[detailInt - 1];
                }
                else
                {
                    HighlightDetail.SetActive(false);
                    HighlightAccept.SetActive(true);
                    HighlightAccept.transform.position = ConfirmButton.transform.position;
                }
                break;
                #endregion
            default:
                break;
        }
        #endregion
    }

    private void CycleBack()
    {
        #region Cycles backward through options in the currently displayed window
        int windowInt = (int)windowCurrent;
        switch (windowInt)
        {
            case 1:
                #region Cycles through the enum of available menu options
                int topInt = (int)topCurrent - 1;
                //If it goes beyond the limit of this enum, resets to the end.
                if (topInt < 1)
                {
                    topInt = 6;
                }
                topCurrent = (topFinder)topInt;
                //If that's the first person button, highlights it appropriately
                if (topInt == 6)
                {
                    HighlightMenuTop.SetActive(false);
                    HighlightFirst.SetActive(true);
                }
                //If it's within the list but not the main menu, moves the highlight over the appropriate icon
                else if (topInt < 5)
                {
                    HighlightMainMenu.SetActive(false);
                    HighlightMenuTop.SetActive(true);
                    HighlightMenuTop.transform.position = menuLocationList[topInt - 1];
                }
                //Otherwise, hides the generic highlight and puts a unique highlight over the main menu icon
                else
                {
                    HighlightFirst.SetActive(false);
                    HighlightMainMenu.SetActive(true);
                }
                break;
                #endregion
            case 2:
                #region Much the same as the high-level menu above, only without the unique highlights.
                int subInt = (int)subCurrent - 1;
                if (subInt < 1)
                {
                    subInt = 4;
                }
                subCurrent = (subFinder)subInt;
                HighlightMenuTop.SetActive(true);
                HighlightMenuTop.transform.position = menuLocationList[subInt - 1];
                break;
                #endregion
            case 3:
                #region Tabs through panes in the current catalogue page
                paneCurrent--;
                //Checks that the next pane cycled to isn't less than 1
                if (paneCurrent < 1)
                {
                    paneCurrent = 6;
                    //Checks that the next pane cycled to isn't empty
                    if (((pageCurrent - 1) * 6) + (paneCurrent - 1) > Resources.readFrom.Count)
                    {
                        //If it is, cycles to the pane one higher than the remainder of the asset list divided by the number of full pages
                        Debug.Log(pageCurrent);
                        paneCurrent = (Resources.readFrom.Count % ((pageCurrent - 1) * 6)) + 1;
                    }
                }
                //Moves the highlight over the appropriate pane
                HighlightCatalogue.SetActive(true);
                HighlightCatalogue.transform.position = panelLocationList[paneCurrent - 1];
                break;
                #endregion
            case 4:
                #region Cycles through options in the detail menu
                //Similar to the top level menu.
                //Int 1 or 2 correlates to the rotation buttons
                //If it's 3, it correlates to the accept button
                int detailInt = (int)detailCurrent - 1;
                if (detailInt < 1)
                {
                    detailInt = 3;
                }
                detailCurrent = (detailFinder)detailInt;
                if (detailInt < 3)
                {
                    HighlightAccept.SetActive(false);
                    HighlightDetail.SetActive(true);
                    HighlightDetail.transform.position = rotateLocationList[detailInt - 1];
                }
                else
                {
                    HighlightDetail.SetActive(false);
                    HighlightAccept.SetActive(true);
                    HighlightAccept.transform.position = ConfirmButton.transform.position;
                }
                break;
                #endregion
            default:
                break;
        }
        #endregion
    }

    public void DetailCycle()
    {
        #region Cycles through list of images of the currently selected artefact
        detailImageCounter++;
        if (detailImageCounter > 3)
        {
            detailImageCounter = 0;
        }

        detailImage.sprite = Sprite.Create(Resources.readFrom[((pageCurrent - 1) * 6) + (paneCurrent - 1)].PreviewImages[detailImageCounter], new Rect(0.0f, 0.0f, Resources.readFrom[((pageCurrent - 1) * 6) + (paneCurrent - 1)].PreviewImages[detailImageCounter].width, Resources.readFrom[((pageCurrent - 1) * 6) + (paneCurrent - 1)].PreviewImages[detailImageCounter].height), new Vector2(0.0f, 0.0f));
        #endregion
    }

    public void DetailBack()
    {
        #region Cycles backwards through list of images of the currently selected artefact
        detailImageCounter--;
        if (detailImageCounter < 0)
        {
            detailImageCounter = 3;
        }

        detailImage.sprite = Sprite.Create(Resources.readFrom[((pageCurrent - 1) * 6) + (paneCurrent - 1)].PreviewImages[detailImageCounter], new Rect(0.0f, 0.0f, Resources.readFrom[((pageCurrent - 1) * 6) + (paneCurrent - 1)].PreviewImages[detailImageCounter].width, Resources.readFrom[((pageCurrent - 1) * 6) + (paneCurrent - 1)].PreviewImages[detailImageCounter].height), new Vector2(0.0f, 0.0f));
        #endregion
    }
    #endregion

    #region Catalogue Scrolling
    public void IncrementPage()
    {
        //Cycles pages upward
        pageCurrent = Mathf.Clamp(pageCurrent++, 1, pageCount);
        ObjectsHide.SetActive(false);
        DisplayPageDetails();
        ObjectsHide.SetActive(true);
        Debug.Log("Current page is: " + pageCurrent.ToString() + ". Max page is: " + pageCount.ToString() + ".");
        //Sets currently selected pane to 0
        HighlightCatalogue.SetActive(false);
        paneCurrent = 0;
    }

    public void DecrementPage()
    {
        //Cycles pages downward
        pageCurrent = Mathf.Clamp(pageCurrent--, 1, pageCount);
        ObjectsHide.SetActive(false);
        DisplayPageDetails();
        ObjectsHide.SetActive(true);
        Debug.Log("Current page is: " + pageCurrent.ToString() + ". Max page is: " + pageCount.ToString() + ".");
        //Sets currently selected pane to 0
        HighlightCatalogue.SetActive(false);
        paneCurrent = 0;
    }
    #endregion

    #region Catalogue setup
    private void MenuSetup()
    {
        #region Loads details for the asset placement submenus
        //Hides listing and detail panel while values are refreshed
        DetailPanel.SetActive(false);
        AssetRepository.SetActive(false);

        //Ensures the highlight is active (if navigated to with mouse)
        //Then moves it to the correct location
        HighlightMenuTop.SetActive(true);
        HighlightMenuTop.transform.position = menuLocationList[switchLists];

        #region Determines that it's reading from the correct Resources.readFrom
        //Note for Dorset - We'd rather have used one single record of all available scriptable objects
        //Then created a list of relevant assets from that record based on the catalogue category selected
        if (RoomSetup == true)
        {
            //Resources = Wherever the room prefabs are stored.GetComponent<TempListScript>();
        }
        else if (isArtefact == true)
        {
            switch (switchLists)
            {
                case 0:
                    Resources = FloorBased.GetComponent<TempListScript>();
                    break;
                case 1:
                    Resources = FloorOrWall.GetComponent<TempListScript>();
                    break;
                case 2:
                    Resources = Small.GetComponent<TempListScript>();
                    break;
                case 3:
                    Resources = Planar.GetComponent<TempListScript>();
                    break;
            }
        }
        else
        {
            switch (switchLists)
            {
                case 0:
                    Resources = Room.GetComponent<TempListScript>();
                    break;
                case 1:
                    Resources = Plinth.GetComponent<TempListScript>();
                    break;
                case 2:
                    Resources = Stand.GetComponent<TempListScript>();
                    break;
                case 3:
                    Resources = Frame.GetComponent<TempListScript>();
                    break;
            }
        }
        #endregion

        //Creates a total page count based on number of objects in Resources.readFrom
        //Includes all full pages, plus a page for the remainder
        listLength = Resources.readFrom.Count + 1;
        Debug.Log(listLength);
        pageCount = listLength / 6;
        if(listLength % 6 > 0)
        {
            pageCount++;
        }

        //Makes sure there is always one page
        if(listLength == 0)
        {
            pageCount = 1;
        }

        pageCurrent = 1;
        DisplayPageDetails();
        
        //Displays the AssetsRepository
        //(And 6 assets panes, if they were independently set inactive for any reason)
        AssetRepository.SetActive(true);
        ObjectsHide.SetActive(true);

        //Updates the player's current window. Permits use of arrow keys to scroll menus
        windowCurrent = windowFinder.Catalogue;
        #endregion
    }

    private void DisplayPageDetails()
    {
        #region Displays values in Resources.readFrom, in objectDisplay
        //Starts at the relevant point for each page, should scale indefinitely.
        countText.text = pageCurrent.ToString() + " / " + pageCount.ToString();
        for (int i = 0; i <= 5; i++)
        {
            Debug.Log(pageCurrent);
            pageNumber = ((pageCurrent - 1) * 6) + i;
            if (pageNumber == (listLength - 1))
            {
                //objectDisplay[i].sprite = null;
            }
            else if (pageNumber > (Resources.readFrom.Count - 1))
            {
                //objectDisplay[i].sprite = null;
            }
            else
            {
                if (Resources.readFrom[pageNumber] == null)
                {
                    continue;
                }
                Debug.Log(Resources.readFrom[pageNumber].ArtefactName);
                objectDisplay[i].sprite = Sprite.Create(Resources.readFrom[pageNumber].PreviewImages[0], new Rect(0.0f, 0.0f, Resources.readFrom[pageNumber].PreviewImages[0].width, Resources.readFrom[pageNumber].PreviewImages[0].height), new Vector2(0.0f, 0.0f));
            }
        }
        paneCurrent = 0;
        detailCurrent = detailFinder.Null;
        HighlightCatalogue.SetActive(false);
        #endregion
    }

    public void CataloguePaneHighlight()
    {
        #region Leaves the highlight in the appropriate place if it was selected by mouse
        //Called by UI_Showdetail
        HighlightCatalogue.SetActive(true);
        HighlightCatalogue.transform.position = panelLocationList[paneCurrent - 1];
        #endregion
    }
    #endregion

    #region Voids performing tasks associated with specific UI buttons
    public void DefaultToArtefacts()
    {
        #region Moves the player from the top-level menu to artefact selection
        //Hides existing highlights
        ResetHighlight();
        //Enters the artefact placement menus.
        HudDefault.SetActive(false);
        ArtefactCategories.SetActive(true);
        windowCurrent = windowFinder.Menu_Sub;
        isArtefact = true;
        #endregion
    }

    public void DefaultToBuild()
    {
        #region Moves the player from the top-level menu to build mode
        //Hides existing highlights
        ResetHighlight();
        //Enters the build menus.
        HudDefault.SetActive(false);
        BuildCategories.SetActive(true);
        windowCurrent = windowFinder.Menu_Sub;
        isArtefact = false;
        #endregion
    }

    public void SwitchToMain()
    {
        #region Sets the main menu active, then instructs the main menu to disable this menu
        MainMenu.SetActive(true);
        UI_MenuController.Activate();
        camController.canHotkey = false;
        #endregion
    }

    public void SendToAssetPlacer()
    {
        #region Instantiates asset, and begins placement
        //Take the gameObject "referenced" from Alex's lists
        //Debug.Log(assetPlacer);
        //Need to work out where we are in the panels
        AssetPlacerScriptableObject newArtefact = Resources.readFrom[((pageCurrent - 1) * 6) + (paneCurrent - 1)];
        // We're gonna do this in Assetplacer now// AssetReference newAsset = newArtefact.GetArtefact();
        Debug.Log(newArtefact.name);
        assetPlacer.ReceiveFromUI(newArtefact);

        ResetBuildUI();
        audioManager.Play("Select_Artifact_SFX");
        #endregion
    }
    #endregion

    #region UI reset voids
    public void HideAllArtefact()
    {
        #region Navigates back to top-level menu
        ResetHighlight();
        //Resets all relevant menus and variables associated with artefact placement.
        DetailPanel.SetActive(false);
        AssetRepository.SetActive(false);
        ArtefactCategories.SetActive(false);
        BuildCategories.SetActive(false);
        HudDefault.SetActive(true);
        windowCurrent = windowFinder.Menu_Top;
        #endregion
    }

    private void ResetHighlight()
    {
        #region Disables and resets all highlights
        HighlightMainMenu.SetActive(false);
        HighlightMenuTop.SetActive(false);
        HighlightCatalogue.SetActive(false);
        HighlightDetail.SetActive(false);
        HighlightAccept.SetActive(false);
        topCurrent = topFinder.Null;
        subCurrent = subFinder.Null;
        paneCurrent = 0;
        detailCurrent = detailFinder.Null;
        #endregion
    }

    public void ResetBuildUI()
    {
        #region Resets HUD to default
        HudDefault.SetActive(true);
        ArtefactCategories.SetActive(false);
        BuildCategories.SetActive(false);
        AssetRepository.SetActive(false);
        DetailPanel.SetActive(false);
        HighlightMainMenu.SetActive(false);
        HighlightMenuTop.SetActive(false);
        HighlightCatalogue.SetActive(false);
        HighlightMenuTopHover.SetActive(false);
        HighlightCatalogueHover.SetActive(false);
        HighlightAccept.SetActive(false);
        HighlightDetail.SetActive(false);
        windowCurrent = windowFinder.Menu_Top;
        topCurrent = topFinder.Null;
        MainMenu.SetActive(false);
        RoomSetup = false;
        #endregion
    }
    #endregion

    private void RoomInitialisation()
    {
        #region first-time room setup
        //Room initialisation needs to read the object containing serialised room prefabs.
        //These must be serialised in the same list structure as artefacts
        //And include Texture2D details in similar form to artefacts

        //MenuSetup();
        #endregion
    }
}
