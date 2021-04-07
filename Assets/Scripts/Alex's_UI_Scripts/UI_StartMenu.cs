using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StartMenu : MonoBehaviour
{
    //Script controlling inputs on the start menu
    //Should be very familiar to anyone who's seen UI_Controller or UI_MenuController, as the latter has very similar functionality
    #region Menus
    [SerializeField] private GameObject MenuMain;
    [SerializeField] private GameObject MenuOptions;
    [SerializeField] private GameObject LoadBuild;
    [SerializeField] private GameObject LoadExplore;
    #endregion

    #region Options Contents
    [SerializeField] private GameObject MusicSliderObject;
    private Slider musicSlider;
    [SerializeField] private GameObject SfxSliderObject;
    private Slider sfxSlider;
    #endregion

    #region highlights

    #endregion

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
    public enum OptionsFinder { Null = 0, Music = 1, SFX = 2 }
    public OptionsFinder optionsCurrent = OptionsFinder.Null;

    //Tracks which object in the confirm window the user is on
    public enum ConfirmFinder { Null = 0, Confirm = 1, Cancel = 2 }
    public ConfirmFinder confirmCurrent = ConfirmFinder.Null;
    #endregion

    private MainMenu sceneNavigation;

    // Start is called before the first frame update
    void Start()
    {
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
        #endregion
    }

    public void NavigateDown()
    {
        #region Move down the hierarchy
        #endregion
    }
}
