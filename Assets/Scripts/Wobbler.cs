using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wobbler : MonoBehaviour
{
    private Transform origin;
    private GameObject target;

    //Always X, rotate only when rotate is true
    public bool rotate;

    public float spd;
    public float rotSpd;
    public float amount;

    void Awake()
    {
        origin = GetComponentInParent<Transform>();
        target = new GameObject();
        //spd = 0.05f;
        //rotSpd = 5f;
        NewTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, target.transform.position) <= 0.001f)
        {
            NewTarget();
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, spd * Time.unscaledDeltaTime);
        }

        if(rotate)
        {
            if(Vector3.Angle(transform.eulerAngles, target.transform.eulerAngles) <= 0.001f)
            {
                NewRotation();
            }
            else
            {
                Quaternion rot = Quaternion.RotateTowards(transform.rotation, target.transform.rotation, rotSpd * Time.unscaledDeltaTime);
                transform.localRotation = rot;
            }
        }
    }

    private void NewTarget()
    {
        float newX;
        newX = origin.transform.position.x + Random.Range(amount * -1,amount);
        target.transform.position = new Vector3(newX, origin.transform.position.y, origin.transform.position.z);
    }

    private void NewRotation()
    {
        float x = Random.Range(85f, 95f);
        float y = Random.Range(265f, 275f);
        float z = Random.Range(355f, 365f);
        target.transform.eulerAngles = new Vector3(x, y, z);
    }

}
