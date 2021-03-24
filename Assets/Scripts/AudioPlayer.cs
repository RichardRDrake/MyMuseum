using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    // Start is called before the first frame update

    public int backgroundVolumeFirstPerson;
    void Start()
    {
        AudioManager.Instance.PlayClip("background_first_person", true, backgroundVolumeFirstPerson, Source.Music);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
