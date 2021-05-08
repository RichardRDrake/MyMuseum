using UnityEngine.Audio;
using System;
using UnityEngine;
public enum Source { Music }

public class AudioManager : MonoBehaviour
{
    //new audiomanager
    public Sound[] sounds;
    public static AudioManager instance;

    public float bgmCurrent;
    public float sfxCurrent;

    [SerializeField] private AudioMixerGroup bgmMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
   //     DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

    }

    void Start()
    {
        //
    }

    public void Play(string name)
    {
        //Debug.Log("Gets here");
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound:" + name + " not found!");
            return;
        }


        if (name.Contains("_BGM"))
        {
            s.source.outputAudioMixerGroup = bgmMixerGroup;
        }
        else if (name.Contains("_SFX"))
        {
            s.source.outputAudioMixerGroup = sfxMixerGroup;
        }
        else
        {
            Debug.Log("Audio not tagged as _BGM or _SFX");
        }
        s.source.Play();
    }




}

