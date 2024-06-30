using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleLight : MonoBehaviour
{
    public Light spot;
    public GameObject targ;
    public float timer;
    public float waitTime;
    float movePerSec;

    void Start()
    {
        targ = new GameObject();
        newTarg();
        waitTime = 1f;
        timer = 0f;
        movePerSec = 50.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > waitTime)
        {
            newTarg();
            timer = 0f;
            waitTime = 1f + Random.Range(-0.25f, 0.75f);
            movePerSec = 50f + Random.Range(-25f, 25f);
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targ.transform.rotation, movePerSec * Time.deltaTime);
        timer += Time.deltaTime;
    }

    void newTarg()
    {
        //-89 to 89 so rotation never tries to go around the opposite side
        targ.transform.rotation = Quaternion.Euler(Random.Range(-10f, 10f), Random.Range(-120f, 120f), 0);
    }
}
