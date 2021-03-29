using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Source { Music }

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] AudioSource firstAudioSource; //Create music source variable

    // create clip variables

    [SerializeField] AudioClip backgroundMusicFirst;
    [SerializeField] AudioClip backgroundMusicThird;

    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>(); //add all clips to dictionary

    void Awake()
    {
        Instance = this;
        _audioClips.Add("background_1st_person", backgroundMusicFirst);
      //  _audioClips.Add("background_3d_person", backgroundMusicThird);

    }

    public IEnumerator PlayClip(string clip, bool loop, float volume, Source source, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayClip(clip, loop, volume, source); //play clip
        yield return null;
    }

    public void PlayClip(string clip, bool loop, float volume, Source source)
    {
        AudioSource targetSource = null;
        if (source == Source.Music)
        {
            targetSource = firstAudioSource;
        }

        targetSource.volume = volume;
        targetSource.clip = _audioClips[clip];
        targetSource.loop = loop;
        targetSource.Play();
    }

    // You can use playoneshot to play multiple sounds simultaneously
    public void PlayOneShot(string clip, float volume)
    {
        firstAudioSource.PlayOneShot(_audioClips[clip], volume);
    }


}

