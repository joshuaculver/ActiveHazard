using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionButton : MonoBehaviour
{
    Toggle toggle;
    public string setting;
    //mode, headBob

    public void Start()
    {
        int saved = PlayerPrefs.GetInt(setting);
        toggle = GetComponent<Toggle>();

        if(saved == 1)
        {
            toggle.isOn = true;
        }
        else
        {
            toggle.isOn = false;
        }

        toggle.onValueChanged.AddListener(delegate {
            ToggleValueChanged(toggle);
        });
    }

    void ToggleValueChanged(Toggle change)
    {
        int set;
        if(toggle.isOn)
        {
            set = 1;
        }
        else
        {
            set = 0;
        }
        PlayerPrefs.SetInt(setting, set);
        Debug.Log("Saved setting " + setting + " - " + set.ToString());
    }
}
