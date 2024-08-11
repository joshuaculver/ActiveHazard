using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBob : MonoBehaviour
{
    public bool enable;

    private float amplitude = 0.00055f;
    private float frequencey = 8.5f;

    float toggleSpd = 2.0f;
    Vector3 startPos;
    CharacterController controller;

    public Transform cam;
    public Transform camHolder;

    private void Awake()
    {
        controller = GetComponentInParent<CharacterController>();
        startPos = cam.localPosition;
        enable = Managers.headBob;
    }

    // Update is called once per frame
    void Update()
    {
        if(!Managers.Player.pMove.canMove)
        {
            return;
        }

        if(!enable)
        {
            return;
        }

        checkMotion();
        resetPos();
        cam.LookAt(focusTarg());
    }

    private Vector3 stepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * frequencey) * amplitude;
        pos.x += Mathf.Cos(Time.time * frequencey / 2) * amplitude * 2;
        return pos;
    }

    private void checkMotion()
    {
        float speed = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude;

        if (speed < toggleSpd)
        {
            return;
        }

        playMotion(stepMotion());
    }

    private void resetPos()
    {
        if (cam.localPosition == startPos)
        {
            return;
        }

        cam.localPosition = Vector3.Lerp(cam.localPosition, startPos, 1 * Time.deltaTime);
    }

    private Vector3 focusTarg()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + camHolder.localPosition.y, transform.position.z);
        pos += camHolder.forward * 15.0f;
        return pos;
    }

    private void playMotion(Vector3 motion)
    {
        cam.localPosition += motion;
    }
}
