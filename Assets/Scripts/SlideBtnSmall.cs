using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlideBtnSmall : MonoBehaviour
{
    private Button thisButton;
    public char set;
    public int slideNum;
    public bool unlocked = false;

    void Awake()
    {
        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(ButtonClick);
        if(set == 'A' && slideNum == 0)
        {
            thisButton.gameObject.SetActive(true);
        }
        else
        {
            thisButton.gameObject.SetActive(unlocked);
        }
    }

    public void ButtonClick()
    {
        Managers.Slides.switchSlide(set, slideNum);
    }

    public void SetSelected()
    {
        thisButton = GetComponent<Button>();
        thisButton.Select();
    }
}
