using UnityEngine;
using System.Collections;

public class PlayerAudEmitter : MonoBehaviour
{

    public bool running = false;
    private float waitTime = 0.9f;
    private float defWaitTime = 0.9f;
    private float timer = 0f;
    private float flyWait = 0.15f;
    private float flyTimer = 0f;
    private bool waiting = false;

    public AudioSource audSrc;
    public AudioSource FXsrc;

    public AudioClip[] stepClip;
    public AudioClip[] metalStepClip;
    public AudioClip[] flyByClip;

    void Awake()
    {
        audSrc.volume = 0.6f;
        FXsrc.volume = 0.8f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Managers.isPaused)
        {
            return;
        }

        if(waiting)
        {
            flyTimer += Time.deltaTime;

            if(flyTimer > flyWait)
            {
                emitFlyBy();
                flyTimer = 0f;
                waiting = false;
            }
        }

        if(!running)
        {
            return;
        }
        else
        {
            timer += Time.deltaTime;

            if(timer > waitTime)
            {
                emitStep();
                timer = timer - waitTime;
                waitTime = 1f + Random.Range(-0.15f, 0.15f);
            }
        }
    }
    
    void emitStep()
    {   
        RaycastHit hit;

        if(Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground")))
        {
            AudioClip stepPick;
            if(hit.collider.CompareTag("MetalFloor"))
            {
                int pick = Random.Range(0, metalStepClip.Length);
                stepPick = metalStepClip[pick];
                Debug.Log("Metal");
            }
            else
            {
                int pick = Random.Range(0, stepClip.Length);
                stepPick = stepClip[pick];
                Debug.Log("Other: " + hit.distance);
            }
            audSrc.pitch = 1 + Random.Range(-0.25f, 0.25f);
            audSrc.clip = stepPick;
            audSrc.Play();
        }
    }
    
    public void emitFlyBy()
    {
        FXsrc.pitch = 1 + Random.Range(-0.1f, 0f);
        int pick = Random.Range(0, flyByClip.Length);
        FXsrc.clip = flyByClip[pick];

        FXsrc.Play();
    }

    public void queFly()
    {
        flyTimer = 0f;
        waiting = true;
    }

    public void changeStepWait(float waitMult)
    {
        waitTime = defWaitTime / waitMult;
    }

    public void resetStepWait()
    {
        waitTime = defWaitTime;
    }
}
