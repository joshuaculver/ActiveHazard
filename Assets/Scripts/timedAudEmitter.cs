using UnityEngine;
using System.Collections;

public class timedAudEmitter : MonoBehaviour
{

    public bool running = false;
    private float waitTime = 1f;
    private float timer = 0f;

    public AudioSource audSrc;
    public AudioSource FXsrc;

    public AudioClip[] stepClip;
    public AudioClip[] runClip;
    public AudioClip[] bangClip;

    public bool titleMode;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(!running || Managers.isPaused)
        {
            return;
        }
        else
        {
            if(!titleMode && Managers.AI.active.status == AIStatus.Chase)
            {
                waitTime = 0.6f;
                timer += Time.deltaTime;
            }
            else
            {
                waitTime = 1.2f;
                if(!titleMode)
                {
                    timer += Time.deltaTime * 0.2f + (Managers.AI.active.agent.speed * 0.0006f);
                }
                else
                {
                    timer += Time.deltaTime * 0.2f + (Random.Range(0.5f,2f) * 0.0006f);
                }
            }

            if(timer > waitTime)
            {
                emitStep();
                timer = timer - waitTime;
            }
        }
    }

    void emitStep()
    {
        if(!titleMode && Managers.AI.active.status == AIStatus.Chase)
        {
            audSrc.volume = 1f;
            audSrc.pitch = Random.Range(0.6f, 1.3f);
            int pick = Random.Range(0, runClip.Length);
            audSrc.clip = runClip[pick];

            audSrc.Play();
        }
        else
        {
            if(titleMode)
            {
                audSrc.volume = 0.2f;
            }
            else
            {
                audSrc.volume = 0.7f;
            }
            int pick = Random.Range(0, stepClip.Length);
            audSrc.clip = stepClip[pick];

            audSrc.Play();
        }
    }

    public void emitBang()
    {
        FXsrc.pitch = 1 + Random.Range(-0.1f, 0f);
        int pick = Random.Range(0, bangClip.Length);
        FXsrc.clip = bangClip[pick];

        FXsrc.Play();
    }
    
}
