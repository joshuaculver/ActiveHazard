using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideAudEmitter : MonoBehaviour
{
    public AudioSource audSrc;

    public List<AudioClip> clips;

    public float timer;
    private float waitTime;
    private float minWait = 120f;
    private float maxWait = 360f;
    public bool active = false;

    public void Awake()
    {
        timer = 0f;
        waitTime = Random.Range(minWait, maxWait);
    }

    public void Update()
    {
        if(active)
        {
            timer += Time.deltaTime;
            if(timer > waitTime)
            {
                audSrc.clip = clips[Random.Range(0, clips.Count)];
                audSrc.Play();
                waitTime = Random.Range(minWait, maxWait);
                timer = 0f;
            }
        }
    }
}
