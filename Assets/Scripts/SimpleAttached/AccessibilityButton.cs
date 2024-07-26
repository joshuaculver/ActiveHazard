using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccessibilityButton : MonoBehaviour
{
    Toggle toggle;

    public void Start()
    {
        toggle = GetComponent<Toggle>();

        toggle.onValueChanged.AddListener(delegate {
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
