using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status {get; private set;}

    public AudioSource[] srcs;
    //Index of current playing/active source
    public AudioSource atk;
    private static int flip = 0;
    private bool running;

    public AudioClip[] breath;
    public AudioClip sting;
    public AudioClip pursuit;
    public AudioClip search;
    public AudioClip[] aim;
    public AudioClip death;
    public AudioClip killMode;
    public AudioClip[] slidesA;
    private AudioClip playing;
    public AIStatus prevStatus;

    public AudioClip[] ambience;

    //Cooldown for "sting" playing
    private float CD;
    //Timer for fading ques over normal music
    private float atkFade = 0f;
    private float aimFade = 0.25f;
    //Volume music moves to when going to "normal" volume
    public float defaultVol = 1f;
    public float SFXVol = 0.3f;
    private bool stingCD = false;
    public int prevDanger;
    private bool MusLock;

    private float noMusTime = 0f;
    //Low for testing
    private float ambThreshold = 300f;
    private int ambInstances = 0;

    //Was the last track for a menu/timescale = 0f context
    private bool menuMus;

    // Start is called before the first frame update
    public void Startup()
    {
        Debug.Log("Music manager starting...");
        for (int i = 0; i < srcs.Length; i++)
        {
            srcs[i].spatialBlend = 0f;
            srcs[i].spatialize = false;
            srcs[i].playOnAwake = false;
            srcs[i].loop = true;
            srcs[i].volume = 0f;
        }

        atk.spatialBlend = 0f;
        atk.spatialize = false;
        atk.playOnAwake = false;
        atk.loop = true;
        atk.volume = 0f;
        atk.Pause();

        menuMus = false;

        MusLock = false;

        running = true;
        Debug.Log("Music manager started");

        check();

        atk.clip = aim[0];

        status = ManagerStatus.Started;
    }

    void Update()
    {
        if(MusLock)
        {
            return;
        }

        //TODO hacky special state just for game over with it's own lock. Probably need a function for special case music/sound
        if(Managers.Player.gameOver)
        {
            MusLock = true;
            StopAll();
            PlayOverride(death);
            srcs[flip].loop = false;
        }

        if(running)
        {
            if(CD > 0)
            {
                CD -= Time.unscaledDeltaTime;
            }

            if(Managers.AI.spawned)
            {
                if(Managers.AI.active.attacking && atk.volume < defaultVol)
                {
                    atkFade += Time.unscaledDeltaTime;

                    if(atkFade >= aimFade)
                    {
                        if(atk.volume < defaultVol)
                        {
                            if(!atk.isPlaying)
                            {
                                atk.Play();
                            }
                            atk.volume += 0.03f;
                            if(atk.volume > defaultVol)
                            {
                                atk.volume = defaultVol;
                            }
                            else
                            {
                                atk.volume += 0.03f;
                                srcs[flip].volume -= 0.03f;
                            }

                            Debug.Log("Attack volume ^:" + atk.volume);
                        }

                        atkFade = 0f;
                    }
                }

                if(!Managers.AI.active.attacking && atk.volume > 0f)
                {
                    atkFade += Time.unscaledDeltaTime;

                    if(atkFade >= aimFade)
                    {
                        if(atk.volume > 0)
                        {
                            atk.volume -= 0.015f;
                            if(srcs[flip].isPlaying)
                            {
                                srcs[flip].volume += 0.015f;
                            }
                            Debug.Log("Attack volume v:" + atk.volume);
                        }
                        else if(atk.volume <= 0f)
                        {
                            atk.Pause();
                            atk.volume = 0f;
                            if(srcs[flip].isPlaying)
                            {
                                srcs[flip].volume = defaultVol;
                            }
                        }
                        Debug.Log("Regular volume ^:" + srcs[flip].volume);
                        
                        atkFade = 0f;
                    }
                }
            }
        }

        if(!srcs[flip].isPlaying || srcs[flip].volume <= 0f)
        {
            noMusTime += Time.deltaTime;

            if(noMusTime >= ambThreshold)
            {
                if(ambInstances <= Random.Range(3,10))
                {
                    PlayMus(ambience[Random.Range(0,2)], defaultVol);
                }
                else
                {
                    PlayMus(ambience[3], defaultVol);
                }
            }
        }
    }

    //Uses two audio sources to crossfade
    private void PlayMus(AudioClip clip, float vol)
    {
        if(srcs[flip].clip == clip && srcs[flip].volume == vol)
        {
            Debug.Log("Already playing");
            return;
        }
        else if(Managers.Menu.slideViewing && srcs[flip].clip == clip && srcs[flip].volume != 0)
        {
            Debug.Log("Already playing");
            return;
        }
        AudioSource source = srcs[flip];

        float dur = 3f;

        //Fade out currently playing
        StartCoroutine(StartFade(source, dur, 0f));

        flip = 1 - flip;
        source = srcs[flip];

        //TODO make volume var. esp for options later
        source.clip = clip;
        source.loop = true;
        source.Play();
        playing = clip;
        if(!atk.isPlaying)
        {
            StartCoroutine(StartFade(source, dur, defaultVol));
        }

        CD = 10f;
        noMusTime = 0f;
    }

    private void PlayOverride(AudioClip clip)
    {
        AudioSource source = srcs[flip];
        source.Stop();

        source.clip = clip;
        source.volume = 0.5f;
        source.loop = false;
        source.Play();
    }

    public IEnumerator Sting()
    {
        if(stingCD == true)
        {
            Debug.Log("Sting on CD");
            yield break;
        }

        running = false;

        Debug.Log("Stinging");

        Debug.Log("Stopped");
        stingCD = true;

        flip = 1 - flip;
        AudioSource source = srcs[flip];

        StopAll();

        source.volume = 0.5f;
        source.clip = sting;
        source.loop = false;

        float timeTo = source.clip.length - 3f;
        float timing = 0f;
        source.Play();

        while(timing < timeTo)
        {
            timing += Time.deltaTime;
            yield return null;
        }

        running = true;
        check();
        yield break;
    }

    private void StopAll()
    {
        for (int i = 0; i < 2; i++)
        {
            AudioSource source = srcs[i];
            
            source.volume = 0f;
            source.Stop();
        }
        atk.volume = 0f;
        atk.Stop();
    }

    private void StartAll()
    {
        for (int i = 0; i < 2; i++)
        {
            AudioSource source = srcs[i];
            
            source.volume = defaultVol;
        }
        atk.volume = defaultVol;
    }

    //TODO replace coroutine with mathf.moveTowards()
    public static IEnumerator StartFade(AudioSource src, float duration, float targetVolume)
    {
        float currTime = 0f;
        float start = src.volume;
        while (currTime < duration)
        {
            currTime += Time.unscaledDeltaTime;
            src.volume = Mathf.Lerp(start, targetVolume, currTime / duration);
            yield return null;
        }
        yield break;
    }

    //Set manager to not running and wait until clip is finished to set running again
    private IEnumerator Wait(float seconds)
    {
        float time = 0f;
        running = false;

        while(time < seconds)
        {
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        running = true;
    }

    //Called by other scripts when states change/events occur
    public void check()
    {
        Debug.Log("Music check");
        if(!running || CD > 0 && !Managers.Menu.slideViewing)
        {
            Debug.Log("Not running");

            if(menuMus && !Managers.Menu.slideViewing)
            {
                Debug.Log("Fading menu music out");
                StartCoroutine(StartFade(srcs[flip], 3f, 0f));
                menuMus = false;
            }
            return;
        }

        if(Managers.Menu.slideViewing)
        {
            Debug.Log("Slide music");
            if(Managers.Slides.slideSet == 'A')
            {
                Debug.Log("Set A");
                float slideVol = 0.9f;
                if(Managers.Slides.slide == 7)
                {
                    Debug.Log("8");
                    PlayMus(slidesA[4], 1f);
                    menuMus = true;
                }
                else if(Managers.Slides.slide == 6)
                {
                    Debug.Log("7");
                    PlayMus(slidesA[3], slideVol);
                    menuMus = true;
                }
                else if(Managers.Slides.slide == 4 || Managers.Slides.slide == 5)
                {
                    Debug.Log("4-6");
                    PlayMus(slidesA[2], slideVol);
                    menuMus = true;
                }
                else if(Managers.Slides.slide == 2 || Managers.Slides.slide == 3)
                {
                    Debug.Log("2/3");
                    PlayMus(slidesA[1], slideVol);
                    menuMus = true;
                }
                else if(Managers.Slides.slide == 0 || Managers.Slides.slide == 1)
                {
                    Debug.Log("0/1");
                    PlayMus(slidesA[0], slideVol);
                    menuMus = true;
                }
            }
            return;
        }

        if(Managers.AI.spawned)
        {
            if(Managers.AI.active.status == prevStatus && Managers.AI.danger == prevDanger)
            {
                Debug.Log("Music check: status and danger unchanged");
                return;
            }
            else
            {
                Debug.Log("Change music");
                if(Managers.AI.active.status == AIStatus.Chase || Managers.AI.active.status == AIStatus.Pursue)
                {
                    Debug.Log("Playing pursuit/chase");
                    if(playing != pursuit)
                    {
                        PlayMus(pursuit, defaultVol);
                        prevStatus = Managers.AI.active.status;
                        prevDanger = Managers.AI.danger;
                    }
                }
                else if(Managers.AI.active.status == AIStatus.Hunt)
                {
                    if(playing != search)
                    {
                        Debug.Log("Playing search");
                        PlayMus(search, defaultVol);
                        prevStatus = Managers.AI.active.status;
                        prevDanger = Managers.AI.danger;
                    }
                }
                else if(Managers.AI.active.status == AIStatus.Avoid || Managers.AI.active.status == AIStatus.Wander)
                {
                    if(Managers.AI.danger >= 5)
                    {
                        if(playing != breath[1])
                        {
                            Debug.Log("Playing breath 1");
                            PlayMus(breath[1], defaultVol);
                            prevStatus = Managers.AI.active.status;
                            prevDanger = Managers.AI.danger;
                        }
                    }
                }
            }
        }
    }

    //Called to check warnings remaining and change attacking track
    public void atkUpdate(int select)
    {   
        float vol = atk.volume;
        atk.volume = 0f;

        if(select == 0 && atk.clip != aim[2])
        {
            atk.clip = aim[2];
            Debug.Log("Switching to: " + atk.clip);
        }
        else if(select == 1 && atk.clip != aim[1])
        {
            atk.clip = aim[1];
            Debug.Log("Switching to: " + atk.clip);
        }
        else if(select == 2 && atk.clip != aim[0])
        {
            atk.clip = aim[0];
            Debug.Log("Switching to: " + atk.clip);
        }

        atk.volume = 0.001f;
        if(!atk.isPlaying)
        {
            atk.Play();
        }
        StartCoroutine(StartFade(atk, 0.5f, vol));
    }

    public void Stop()
    {
        running = false;
        StopAll();
    }

    public void Start()
    {
        running = true;
        StartAll();
    }
}
