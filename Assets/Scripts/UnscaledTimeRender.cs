using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnscaledTimeRender : MonoBehaviour
{
    Renderer ren;
    public Color color;

    public float spd = 1f;

    void Start()
    {
        ren = GetComponent<Renderer>();
        color = new Color(0.356f, 0.356f, 0.356f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        ren.material.SetFloat("_UnSinTime", Mathf.Sin(Time.unscaledTime * spd));
        ren.material.SetFloat("_UnCosTime", Mathf.Cos(Time.unscaledTime * spd));
    }
}
