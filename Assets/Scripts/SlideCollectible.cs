using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideCollectible : Interactable
{
    public char set;
    public int ID;

    private int showState;
    private float timer = 0f;
    private float timerWait;

    public Light spot;
    public MeshRenderer beam;

    public SlideAudEmitter emitter;

    public void Awake()
    {
        //spot = GetComponentInChildren<Light>();
        //emitter = GetComponentInChildren<SlideAudEmitter>();

        if(set == 'A' && ID == 0)
        {
            showState = 3;
        }
        else
        {
            showState = 0;
        }

        timerWait = ID * 20f;
    }

    void Update()
    {
        if(showState < 3)
        {
            timer += Time.deltaTime;

            if(timer > timerWait)
            {
                showState += 1;
                CheckShowState();
                timer = 0f;
            }
        }
    }

    public override void Interact()
    {
        if(Managers.AI.spawned)
        {
            if(Managers.AI.active.status == AIStatus.Chase || Managers.AI.active.status == AIStatus.Pursue)
            {
                //TODO add unable to pick up collectible audio feedback
            }
            else
            {
                Managers.Slides.CollectSlide(set, ID);
            }
        }
        else
        {
            Managers.Slides.CollectSlide(set, ID);
        }
    }

    public string GetID()
    {
        string partA = set.ToString();
        string partB = ID.ToString();
        string objectID = partA + partB;

        return objectID;
    }

    public void CheckShowState()
    {
        if(showState > 3)
        {
            showState = 3;
        }
        else if(showState < 0)
        {
            showState = 0;
        }

        if(showState == 3)
        {
            spot.gameObject.SetActive(true);
            beam.gameObject.SetActive(true);
            emitter.gameObject.SetActive(true);
            emitter.active = true;
        }
        else if(showState == 2)
        {
            beam.gameObject.SetActive(true);
            emitter.gameObject.SetActive(true);
            emitter.active = true;

            spot.gameObject.SetActive(false);        
        }
        else if(showState == 1)
        {
            emitter.gameObject.SetActive(true);
            emitter.active = true;

            spot.gameObject.SetActive(false);
            beam.gameObject.SetActive(false);
        }
        else if(showState == 0)
        {
            spot.gameObject.SetActive(false);
            beam.gameObject.SetActive(false);
            emitter.gameObject.SetActive(false);
            emitter.active = false;
        }
    }
}
