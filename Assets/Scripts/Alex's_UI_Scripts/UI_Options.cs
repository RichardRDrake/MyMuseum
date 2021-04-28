using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

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

    //Audio mixer
    [SerializeField] private AudioMixer bgmMixer;
    [SerializeField] private AudioMixer sfxMixer;

    // Start is called before the first frame update
    void Start()
    {
        MusicTextDisplay = MusicText.GetComponent<TextMeshProUGUI>();
        SfxTextDisplay = SfxText.GetComponent<TextMeshProUGUI>();
        //These need to read from wherever audio settings are being stored
        if (PlayerPrefs.HasKey("BGM"))
        {
            Debug.Log("PlayerPrefs loaded BGM");
        }
        else
        {
            PlayerPrefs.SetFloat("BGM", 0.8f);
        }
        if (PlayerPrefs.HasKey("SFX"))
        {
            Debug.Log("PlayerPrefs loaded SFX");
        }
        else
        {
            PlayerPrefs.SetFloat("SFX", 0.8f);
        }
        MusicTextDisplay.text = Mathf.RoundToInt(PlayerPrefs.GetFloat("BGM") * 100).ToString();
        SfxTextDisplay.text = Mathf.RoundToInt(PlayerPrefs.GetFloat("SFX") * 100).ToString();
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

    //These connect to the sliders for music and SFX, respectively
    public void Music(float sliderMusic)
    {
        musicVolume = sliderMusic;
        bgmMixer.SetFloat("bgmVol", Mathf.Log10(musicVolume) * 20);
    }

    public void Sfx(float sliderSfx)
    {
        sfxVolume = sliderSfx;
        sfxMixer.SetFloat("sfxVol", Mathf.Log10(sfxVolume) * 20);
    }
}
