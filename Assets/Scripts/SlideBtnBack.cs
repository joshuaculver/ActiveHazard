using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlideBtnBack : MonoBehaviour
{
    private Button backButton;
    // Start is called before the first frame update
    void Awake()
    {
        backButton = GetComponent<Button>();

        backButton.onClick.AddListener(ButtonClick);
    }

    public void ButtonClick()
    {
        Managers.Menu.closeSlideMenu();
    }
}
