using UnityEngine.Audio;
using System;
using UnityEngine;
public enum Source { Music }

public class AudioManager : MonoBehaviour
{
    //new audiomanager
    public Sound[] sounds;  

    void Awake()
    {
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }

    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }

    


}

