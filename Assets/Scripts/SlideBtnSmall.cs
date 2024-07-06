using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlideBtnSmall : MonoBehaviour
{
    private Button thisButton;
    public char set;
    public int slideNum;

    void Awake()
    {
        thisButton = GetComponent<Button>();

        thisButton.onClick.AddListener(ButtonClick);
    }

    public void ButtonClick()
    {
        Managers.Slides.switchSlide(set, slideNum);
    }
}
