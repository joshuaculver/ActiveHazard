using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendEye : MonoBehaviour
{
    private Transform eye;
    private Vector3 originScale;
    private Vector3 targetSize;

    // Start is called before the first frame update
    void Start()
    {
        eye = GetComponent<Transform>();
        //originScale = new Vector3(originScale.x, originScale.y, originScale.z);
        originScale = new Vector3(0.05f, 0.05f, 0.05f);
        Debug.Log(eye);
        Debug.Log(originScale);
    }

    void FixedUpdate()
    {
        Look();
    }

    void Look()
    {
        if(Managers.Player.player != null)
        {
            eye.LookAt(Managers.Player.player.transform);
        }
    }
}
