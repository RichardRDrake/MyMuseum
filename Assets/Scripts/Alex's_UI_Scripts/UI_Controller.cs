﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Controller : MonoBehaviour
{
    #region variables

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

    private TempListScript Resources;
    //Length of Resources
    int listLength;

    //Used to determine which list to read from
    public int switchLists = 0;

    //Signals whether a menu needs to be displayed
    public bool displayMenu = false;

    int pageCount;
    int pageCurrent = 1;
    int pageNumber;

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

    //This determines whether the script should read from the artefact or build objects repositories
    //true = artefact, false = build
    private bool isArtefact = false;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        #region initialise variables
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

        //Gets the camera controller
        camController = Camera.GetComponent<CamController>();

        //Gets the (hopefully) single assetplacer in the scene
        assetPlacer = FindObjectOfType<AssetPlacer>();
        if (assetPlacer == null)
        {
            Debug.LogError("no assetPlacer present in scene");
        }
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        //Moves the player back up a menu level from wherever they were.
        if(Input.GetKeyDown("escape") || Input.GetKeyDown("backspace"))
        {
            NavigateUp();  
        }

        //If the UI controller recieves a signal from any artefact category button
        if(displayMenu == true)
        {
            displayMenu = false;
            MenuSetup();
        }


        //Pertaining to use of arrow keys to change pages in catalogue
        #region Catalogue Scrolling
        if (Input.GetKeyDown("left") && windowCurrent == windowFinder.Catalogue)
        {
            DecrementPage();
        }
        
        if(Input.GetKeyDown("right") && windowCurrent == windowFinder.Catalogue)
        {
            IncrementPage();
        }
        #endregion

        //Determines what to do if the player hits tab or down
        if(Input.GetKeyDown("tab") || Input.GetKeyDown("down"))
        {
            CycleThroughCatalogue();
        }

        //Determines what to do if the player hits left
        if(Input.GetKeyDown("up"))
        {
            CycleBackCatalogue();
        }

        //Determines what to do if the player hits enter or space
        if(Input.GetKeyDown("space") || Input.GetKeyDown("enter") || Input.GetKeyDown("return"))
        {
            NavigateDown();
        }
    }

    private void MenuSetup()
    {
        //Loads details for the asset placement submenus
        #region Sets up catalogue display
        //Hides listing and detail panel while values are refreshed
        DetailPanel.SetActive(false);
        AssetRepository.SetActive(false);

        //Ensures the highlight is active (if navigated to with mouse)
        //Then moves it to the correct location
        HighlightMenuTop.SetActive(true);
        HighlightMenuTop.transform.position = menuLocationList[switchLists];

        //Determines that it's reading from the correct Resources.readFrom
        if (isArtefact == true)
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
        //Displays values in Resources.readFrom, in objectDisplay
        //Starts at the relevant point for each page, should scale indefinitely.
        #region Display catalogue per page
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

    #region Catalogue page voids
    public void IncrementPage()
    {
        //Cycles pages upward
        pageCurrent++;
        pageCurrent = Mathf.Clamp(pageCurrent, 1, pageCount);
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
        pageCurrent--;
        pageCurrent = Mathf.Clamp(pageCurrent, 1, pageCount);
        ObjectsHide.SetActive(false);
        DisplayPageDetails();
        ObjectsHide.SetActive(true);
        Debug.Log("Current page is: " + pageCurrent.ToString() + ". Max page is: " + pageCount.ToString() + ".");
        //Sets currently selected pane to 0
        HighlightCatalogue.SetActive(false);
        paneCurrent = 0;
    }
    #endregion

    public void HideAllArtefact()
    {
        //Hides existing highlights
        ResetHighlight();
        //Resets all relevant menus and variables associated with artefact placement.
        DetailPanel.SetActive(false);
        AssetRepository.SetActive(false);
        ArtefactCategories.SetActive(false);
        BuildCategories.SetActive(false);
        HudDefault.SetActive(true);
        windowCurrent = windowFinder.Menu_Top;
    }

    public void DefaultToArtefacts()
    {
        //Hides existing highlights
        ResetHighlight();
        //Enters the artefact placement menus.
        HudDefault.SetActive(false);
        ArtefactCategories.SetActive(true);
        windowCurrent = windowFinder.Menu_Sub;
        isArtefact = true;
    }

    public void DefaultToBuild()
    {
        //Hides existing highlights
        ResetHighlight();
        //Enters the build menus.
        HudDefault.SetActive(false);
        BuildCategories.SetActive(true);
        windowCurrent = windowFinder.Menu_Sub;
        isArtefact = false;
    }

    private void ResetHighlight()
    {
        //Disables and resets all highlights
        HighlightMainMenu.SetActive(false);
        HighlightMenuTop.SetActive(false);
        HighlightCatalogue.SetActive(false);
        HighlightDetail.SetActive(false);
        HighlightAccept.SetActive(false);
        topCurrent = topFinder.Null;
        subCurrent = subFinder.Null;
        paneCurrent = 0;
        detailCurrent = detailFinder.Null;
    }

    private void NavigateDown()
    {
        //Goes an additional level down the menu hierarchy
        #region Down Hierarchy
        int windowInt = (int)windowCurrent;
        switch (windowInt)
        {
            //If the player is on the top level menu
            case 1:
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
                        //This should refer to the camera controller
                        //camController.yourcodehere();
                        break;
                }
                break;
            //If the player is on a submenu
            case 2:
                //Sets MenuSetup to read from the selected submenu's contents
                //If a submenu is not selected, ignores this function
                int subInt = (int)subCurrent;
                switchLists = subInt - 1;
                if (switchLists >= 0)
                {
                    MenuSetup();
                }
                break;
            //If the player is in the catalogue
            case 3:
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
            case 4:
                //If the player is in the detail pane
                //Joe does something if detailCurrent  is 3, or "select"
                //Should redirect to a public void, so the button can be connected, too.
                //ResetBuildUI (resets but does not disable UI)
                int detailInt = (int)detailCurrent;
                switch(detailInt)
                {
                    case 1:
                        DetailCycle();
                        break;
                    case 2:
                        DetailBack();
                        break;
                    case 3:
                        SendToAssetPlacer();
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
        #endregion
    }
    public void NavigateUp()
    {
        //Moves the player back up the menu hierarchies
        #region Backing up
        int windowInt = (int) windowCurrent;
        if(windowInt >=2 && windowInt <= 4)
        {
            windowInt--;
            switch (windowInt)
            {
                case 1:
                    HideAllArtefact();
                    ResetHighlight();
                    windowCurrent = (windowFinder)windowInt;
                    break;
                case 2:
                    AssetRepository.SetActive(false);
                    paneCurrent = 0;
                    HighlightCatalogue.SetActive(false);
                    windowCurrent = (windowFinder)windowInt;
                    break;
                case 3:
                    DetailPanel.SetActive(false);
                    detailImageCounter = 0;
                    HighlightDetail.SetActive(false);
                    HighlightAccept.SetActive(false);
                    detailCurrent = detailFinder.Null;
                    windowCurrent = (windowFinder)windowInt;
                    break;
                default:
                    break;
            }
        }
        else if(windowInt == 1)
        {
            SwitchToMain();
        }
        #endregion
    }

    private void CycleThroughCatalogue()
    {
        //Cycles forward through options in the currently displayed window
        #region Forward Cycle
        int windowInt = (int)windowCurrent;
        switch (windowInt)
        {
            case 1:
                //Cycles through the enum of available menu options
                int topInt = (int)topCurrent + 1;
                //If it goes beyond the limit of this enum, resets to the start.
                if(topInt > 6)
                {
                    topInt = 1;
                }
                topCurrent = (topFinder)topInt;
                //If it's within the list but not the main menu, moves the highlight over the appropriate icon
                if(topInt < 5)
                {
                    HighlightFirst.SetActive(false);
                    HighlightMenuTop.SetActive(true);
                    HighlightMenuTop.transform.position = menuLocationList[topInt - 1];
                }
                //Otherwise, hides the generic highlight and puts a unique highlight over the main menu icon
                else if(topInt == 5)
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
            case 2:
                //Much the same as the high-level menu above, only without the unique highlight.
                int subInt = (int)subCurrent + 1;
                if(subInt > 4)
                {
                    subInt = 1;
                }
                subCurrent = (subFinder)subInt;
                HighlightMenuTop.SetActive(true);
                HighlightMenuTop.transform.position = menuLocationList[subInt - 1];
                break;
            case 3:
                paneCurrent++;
                //Checks that the next pane cycled to isn't greater than 6, or empty
                if(paneCurrent > 6 || ((pageCurrent - 1) * 6) + (paneCurrent - 1) > Resources.readFrom.Count)
                {
                    paneCurrent = 1;
                }
                //Moves the highlight over the appropriate pane
                HighlightCatalogue.SetActive(true);
                HighlightCatalogue.transform.position = panelLocationList[paneCurrent - 1];
                break;
            case 4:
                //Cycles through options in the detail menu
                //Similar to the top level menu.
                //Int 1 or 2 correlates to the rotation buttons
                //If it's 3, it correlates to the accept button
                int detailInt = (int)detailCurrent + 1;
                if(detailInt > 3)
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
                }
                break;
            default:
                break;
        }
        #endregion
    }

    private void CycleBackCatalogue()
    {
        //Cycles backward through options in the currently displayed window
        #region Backward Cycle
        int windowInt = (int)windowCurrent;
        switch (windowInt)
        {
            case 1:
                //Cycles through the enum of available menu options
                int topInt = (int)topCurrent - 1;
                //If it goes beyond the limit of this enum, resets to the end.
                if (topInt < 1)
                {
                    topInt = 6;
                }
                topCurrent = (topFinder)topInt;
                //If that's the first person button, highlights it appropriately
                if(topInt == 6)
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
            case 2:
                //Much the same as the high-level menu above, only without the unique highlight.
                int subInt = (int)subCurrent - 1;
                if (subInt < 1)
                {
                    subInt = 4;
                }
                subCurrent = (subFinder)subInt;
                HighlightMenuTop.SetActive(true);
                HighlightMenuTop.transform.position = menuLocationList[subInt - 1];
                break;
            case 3:
                paneCurrent--;
                //Checks that the next pane cycled to isn't less than 1
                if (paneCurrent < 1)
                {
                    paneCurrent = 6;
                    //Checks that the next pane cycled to isn't empty
                    if(((pageCurrent - 1) * 6) + (paneCurrent - 1) > Resources.readFrom.Count)
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
            case 4:
                //Cycles through options in the detail menu
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
                }
                break;
            default:
                break;
        }
        #endregion
    }

    public void CataloguePaneHighlight()
    {
        //Called by UI_Showdetail
        //Leaves the highlight in the appropriate place if it was selected by mouse
        #region Highlight
        HighlightCatalogue.SetActive(true);
        HighlightCatalogue.transform.position = panelLocationList[paneCurrent - 1];
        #endregion
    }

    public void ResetBuildUI()
    {
        //Resets HUD to default
        #region Resets UI
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
        #endregion
    }

    public void SwitchToMain()
    {
        //Sets the main menu active, then instructs the main menu to disable this menu
        #region Switch to main menu
        MainMenu.SetActive(true);
        UI_MenuController.Activate();
        camController.canHotkey = false;
        #endregion
    }

    public void SendToAssetPlacer()
    {
        //Take the gameObject "referenced" from Alex's lists
        //Debug.Log(assetPlacer);
        //Need to work out where we are in the panels
        AssetPlacerScriptableObject newArtefact = Resources.readFrom[((pageCurrent - 1) * 6) + (paneCurrent - 1)];
        // We're gonna do this in Assetplacer now// AssetReference newAsset = newArtefact.GetArtefact();
        Debug.Log(newArtefact.name);
        assetPlacer.ReceiveFromUI(newArtefact);

        ResetBuildUI();
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
        if(detailImageCounter < 0)
        {
            detailImageCounter = 3;
        }

        detailImage.sprite = Sprite.Create(Resources.readFrom[((pageCurrent - 1) * 6) + (paneCurrent - 1)].PreviewImages[detailImageCounter], new Rect(0.0f, 0.0f, Resources.readFrom[((pageCurrent - 1) * 6) + (paneCurrent - 1)].PreviewImages[detailImageCounter].width, Resources.readFrom[((pageCurrent - 1) * 6) + (paneCurrent - 1)].PreviewImages[detailImageCounter].height), new Vector2(0.0f, 0.0f));
        #endregion
    }
}
