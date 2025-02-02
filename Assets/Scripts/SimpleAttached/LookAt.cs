using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform target;

    public float speed;

    // Update is called once per frame
    void Update()
    {
        Vector3 targetDir = target.position - transform.position;

        float step = speed * Time.deltaTime;

        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, speed);

        transform.rotation = Quaternion.LookRotation(newDir);
    }
}
