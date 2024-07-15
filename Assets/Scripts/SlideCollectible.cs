using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideCollectible : Interactable
{
    public char set;
    public int ID;

    private int showState;

    private Light spot;
    public MeshRenderer beam;

    private SlideAudEmitter emitter;

    public void Awake()
    {
        spot = GetComponentInChildren<Light>();
        emitter = GetComponentInChildren<SlideAudEmitter>();

        if(set == 'A' && ID == 0)
        {
            showState = 3;
        }
        else
        {
            showState = 0;
        }
    }

    public override void Interact()
    {
        Managers.Slides.CollectSlide(set, ID);
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
        }
        else if(showState == 2)
        {
            beam.gameObject.SetActive(true);
            emitter.gameObject.SetActive(true);

            spot.gameObject.SetActive(false);        
        }
        else if(showState == 1)
        {
            emitter.gameObject.SetActive(true);

            spot.gameObject.SetActive(false);
            beam.gameObject.SetActive(false);
        }
        else if(showState == 0)
        {
            spot.gameObject.SetActive(false);
            beam.gameObject.SetActive(false);
            emitter.gameObject.SetActive(false);
        }
    }
}
