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

    public void Update()
    {

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
}
