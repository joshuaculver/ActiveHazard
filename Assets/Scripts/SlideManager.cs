using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status {get; private set;}
    public GameObject slideUI;
    public MeshRenderer slideImage;
    //Default 91,91,91,255
    public MeshRenderer slideBack;
    public UnscaledTimeRender shader;
    public List<Material> slidesA;

    public Light slideLight;
    private float lightIntensity = 5f;
    private float innerSpot = 16f;
    private float outerSpot = 21.5f;

    public char slideSet;
    public int slide;
    public char newSet;
    public int newSlide;

    public bool switchingSlide = false;
    private float switchSpd = 0.01f;

    public bool haveSlide;

    //public List<SlideBtnBig> setButtons;
    public List<SlideBtnSmall> buttonsA;
    public List<SlideCollectible> collectiblesA;

    public void Startup()
    {
        Debug.Log("Slide manager starting...");

        slideSet = 'X';
        slide = -1;
        slideUI.SetActive(false);
        slideLight.intensity = 0;
        shader = slideBack.GetComponent<UnscaledTimeRender>();
        haveSlide = false;

        status = ManagerStatus.Started;
    }

    // Update is called once per frame
    void Update()
    {
        if(Managers.Menu.slideViewing)
        {
            if(switchingSlide)
            {
                if(slideLight.intensity > 0f)
                {
                    slideLight.intensity = Mathf.MoveTowards(slideLight.intensity, 0f, switchSpd);
                }
                else
                {
                    Managers.Player.icon.color = new Color(1f, 1f, 1f, 0f);

                    SetSlide(newSet, newSlide);
                    
                    if(slideSet == 'A' && slide == 7)
                    {
                        slideLight.color = Color.red;
                        slideLight.spotAngle = 17.5f;
                        slideLight.innerSpotAngle = 15.5f;
                        shader.spd = 25f;
                    }
                    else if(slideLight.color != Color.white)
                    {
                        shader.spd = 1f;
                        slideLight.color = Color.white;
                        slideLight.spotAngle = outerSpot;
                        slideLight.innerSpotAngle = innerSpot;
                    }
                    switchingSlide = false;
                }
            }
            else
            {
                if(slideLight.intensity < lightIntensity)
                {
                slideLight.intensity = Mathf.MoveTowards(slideLight.intensity, lightIntensity, switchSpd); 
                }
                else if(slideLight.intensity > lightIntensity)
                {
                    slideLight.intensity = lightIntensity;
                }
            }
        }
    }

    //Initiates change
    public void switchSlide(char set, int sli)
    {
        if(slideSet == set && slide == sli)
        {
            return;
        }
        //If menu manager not transitioning
        newSet = set;
        newSlide = sli;

        switchingSlide = true;
    }

    //Swaps material
    public void SetSlide(char setChar, int slideNum)
    {
        if(setChar == 'A')
        {
            slideImage.material = slidesA[slideNum];
        }
        //If special slide set back color
        slideSet = setChar;
        slide = slideNum;

        //Managers.State.CheckInventory(slideSet, slide);
        
        Managers.Music.check();
    }

    public void ShowSlideUI()
    {
        slideUI.SetActive(true);
        Managers.Music.check();
    }

    public void HideSlideUI()
    {
        slideUI.SetActive(false);
    }

    public void EnableSlideCollectible(char set, int id)
    {
        if(set == 'A')
        {
            for(int i = 0; i < collectiblesA.Count; i++)
            {
                if(collectiblesA[i].ID == id)
                {
                    collectiblesA[i].gameObject.SetActive(true);
                }
            }
        }
    }

    public void EnableNext()
    {
        for(int i = 0; i < collectiblesA.Count; i++)
        {
            if(!collectiblesA[i].isActiveAndEnabled)
            {
                if(collectiblesA[i].ID != 7)
                {
                    Debug.Log("Enablng: A" + i.ToString());
                    collectiblesA[i].gameObject.SetActive(true);
                    return;
                }
            }
        }

        Debug.Log("No slides except final remaining");
        Debug.Log("Enabling");

        collectiblesA[7].gameObject.SetActive(true);
    }

    public void CollectSlide(char set, int id)
    {
        Debug.Log("Collecting Slide");
        Managers.State.AddInventory(set, id);
        if(set == 'A')
        {
            for (int i = 0; i < buttonsA.Count; i++)
            {
                if(buttonsA[i].set == set && buttonsA[i].slideNum == id)
                {
                    haveSlide = true;
                    buttonsA[i].gameObject.SetActive(true);
                    Managers.Menu.openSlideMenu('A', i);
                    buttonsA[i].SetSelected();
                }
            }
        }

        if(id == 7)
        {
            Managers.State.playerProgress.AEnding = true;
        }
    }
}
