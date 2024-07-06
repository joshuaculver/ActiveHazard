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
    private float innerSpot = 15f;
    private float outerSpot = 19.5f;

    public char slideSet;
    public int slide;
    public char newSet;
    public int newSlide;

    public bool switchingSlide = false;
    private float switchSpd = 0.01f;
    public void Startup()
    {
        slideSet = 'A';
        slide = 0;
        slideUI.SetActive(false);
        slideLight.intensity = 0;
        shader = slideBack.GetComponent<UnscaledTimeRender>();
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
                    SetSlide(newSet, newSlide);
                    if(slideSet == 'A' && slide == 7)
                    {
                        slideLight.color = Color.red;
                        slideLight.spotAngle = 15.5f;
                        slideLight.innerSpotAngle = 14.5f;
                    }
                    else if(slideLight.color != Color.white)
                    {
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

    public void switchSlide(char set, int sli)
    {
        //If menu manager not transitioning
        newSet = set;
        newSlide = sli;

        switchingSlide = true;
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

    private void SetSlide(char setChar, int slideNum)
    {
        if(setChar == 'A')
        {
            slideImage.material = slidesA[slideNum];
        }
        //If special slide set back color
        slideSet = setChar;
        slide = slideNum;
        Managers.Music.check();
    }

    public void Temp1()
    {
        Debug.Log("Temp1");
        switchSlide('A', 0);
    }

    public void Temp2()
    {
        Debug.Log("Temp2");
        switchSlide('A', 1);
    }
}
