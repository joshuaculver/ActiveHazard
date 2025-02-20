using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccessibilityButton : MonoBehaviour
{
    Toggle toggle;

    public void Start()
    {
        int saved = PlayerPrefs.GetInt("mode");
        toggle = GetComponent<Toggle>();

        if(saved == 1)
        {
            toggle.isOn = true;
        }
        else
        {
            toggle.isOn = false;
        }

        toggle.onValueChanged.AddListener(delegate{
            ToggleValueChanged(toggle);
        });
    }

    void ToggleValueChanged(Toggle change)
    {
        int setting;
        if(toggle.isOn)
        {
            setting = 1;
        }
        else
        {
            setting = 0;
        }
        PlayerPrefs.SetInt("mode", setting);
        Debug.Log("Saved setting" + setting.ToString());
    }
}
