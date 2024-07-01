using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnscaledTimeRender : MonoBehaviour
{
    Renderer ren;

    void Start()
    {
        ren = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        ren.material.SetFloat("_UnSinTime", Mathf.Sin(Time.unscaledTime));
        ren.material.SetFloat("_UnCosTime", Mathf.Cos(Time.unscaledTime));
    }
    
}
