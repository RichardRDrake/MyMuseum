using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StartMenu : MonoBehaviour
{
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
        
    }
}
