using UnityEngine;
using System.Collections;

public class PlayerAudEmitter : MonoBehaviour
{

    public bool running = false;
    private float waitTime = 0.9f;
    private float timer = 0f;
    private float flyWait = 0.15f;
    private float flyTimer = 0f;
    private bool waiting = false;

    public AudioSource audSrc;
    public AudioSource FXsrc;

    public AudioClip[] stepClip;
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
        int pick = Random.Range(0, stepClip.Length);
        audSrc.pitch = 1 + Random.Range(-0.25f, 0.25f);
        audSrc.clip = stepClip[pick];

        audSrc.Play();
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
}
