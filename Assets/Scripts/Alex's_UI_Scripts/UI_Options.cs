using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Options : MonoBehaviour
{
    //Display text fields
    [SerializeField] private GameObject MusicText;
    private TextMeshProUGUI MusicTextDisplay;
    [SerializeField] private GameObject SfxText;
    private TextMeshProUGUI SfxTextDisplay;

    //Variables
    public float musicVolume;
    private float musicVolumeInternal;
    public float sfxVolume;
    private float sfxVolumeInternal;

    //Audio controller ref here



    // Start is called before the first frame update
    void Start()
    {
        MusicTextDisplay = MusicText.GetComponent<TextMeshProUGUI>();
        SfxTextDisplay = SfxText.GetComponent<TextMeshProUGUI>();
        //These need to read from wherever audio settings are being stored
        MusicTextDisplay.text = "80";
        SfxTextDisplay.text = "80";
    }

    // Update is called once per frame
    void Update()
    {
        //If the value of the slider has changed
        if (musicVolume != musicVolumeInternal)
        {
            //Rounds the current slider value to the closest integer
            //Then sets the display to that integer
            int musicTemp = Mathf.RoundToInt(musicVolume * 100);
            MusicTextDisplay.text = musicTemp.ToString();
            musicVolumeInternal = musicVolume;
            //Insert line here to write to audio controller
        }

        //As above, with SFX
        if (sfxVolume != sfxVolumeInternal)
        {
            int sfxTemp = Mathf.RoundToInt(sfxVolume * 100);
            SfxTextDisplay.text = sfxTemp.ToString();
            sfxVolumeInternal = sfxVolume;
            //Insert line here to write to audio controller
        }
    }

    //These connect to UI_MenuController and the sliders for music and SFX, respectively
    public void Music(float sliderMusic)
    {
        musicVolume = sliderMusic;
    }

    public void Sfx(float sliderSfx)
    {
        sfxVolume = sliderSfx;
    }
}
