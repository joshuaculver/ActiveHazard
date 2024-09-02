using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbSFXPauser : MonoBehaviour
{
    public AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.volume = Managers.Music.SFXVol;
    }

    // Update is called once per frame
    void Update()
    {
        if(Managers.isPaused)
        {
            if(source.isPlaying)
            {
                source.Pause();
            }
        }
        if(!Managers.isPaused)
        {
            if(!source.isPlaying)
            {
                source.UnPause();
            }
        }
    }
}
