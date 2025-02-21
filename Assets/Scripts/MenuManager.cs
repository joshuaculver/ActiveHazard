using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class MenuManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status {get; private set;}
    public bool pauseMenuOpen = false;
    public GameObject pausePanel;
    public bool slideViewing = false;

    private SlideManager slides;

    public RawImage solid;
    private float fadeSpd = 1.75f;
    public UnityEngine.UIElements.Button resume;
    public UnityEngine.UIElements.Button mainMenu;

    private Camera mainCam;
    public Camera slideCam;
    private Camera targCam;

    private bool transitioning = false;

    public bool playerDead = false;

    public bool sceneChange;

    public void Startup()
    {
        mainCam = Managers.Player.cam;
        mainCam.enabled = true;
        slideCam.enabled = false;
        pausePanel.SetActive(false);
        slides = GetComponent<SlideManager>();
        status = ManagerStatus.Started;
    }

    void Update()
    {
        if(playerDead)
        {
            Debug.Log("Dead");
            if(solid.color.a < 1f)
            {
                Debug.Log("Fading");
                solid.color = new Color(0f, 0f, 0f, Mathf.MoveTowards(solid.color.a, 1f, 0.3f * Time.deltaTime));
            }
            else
            {
                SceneManager.LoadScene("MainMenu");
            }
            return;
        }
        if(transitioning && !sceneChange)
        {
            if(solid.color.a < 1f)
            {
                solid.color = new Color(0f, 0f, 0f, Mathf.MoveTowards(solid.color.a, 1f, fadeSpd * Time.unscaledDeltaTime));
            }
            else if(solid.color.a >  0.9f)
            {
                Debug.Log("Switching camera");
                //switch cameras
                if(targCam == mainCam)
                {
                    Managers.Slides.HideSlideUI();
                    mainCam.enabled = true;
                    slideCam.enabled = false;
                }
                else if(targCam == slideCam)
                {
                    mainCam.enabled = false;
                    slideCam.enabled = true;
                    openSlideMenu(Managers.Slides.slideSet, Managers.Slides.slide);
                    Managers.Slides.ShowSlideUI();
                    Debug.Log("Intro: " + Managers.State.playerProgress.inIntro);
                    if(Managers.State.playerProgress.inIntro)
                    {
                        Managers.State.OpenLobby();
                    }
                }
                //Switch to not transitioning to fade back in
                transitioning = false;
                Debug.Log("Main cam:" + mainCam.enabled + " - Slide Cam:" + slideCam.enabled);
            }
            else
            {
                Debug.Log("No transition case met");
                transitioning = false;
            }
        }
        else if(!transitioning && !sceneChange && solid.color.a > 0f)
        {
            solid.color = new Color(0f, 0f, 0f, Mathf.MoveTowards(solid.color.a, 0f, fadeSpd * Time.unscaledDeltaTime));
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(pauseMenuOpen)
            {
                closePauseMenu();
            }
            else if(!pauseMenuOpen)
            {
                openPauseMenu();
            }
        }
        //Right mouse toggles slide menu
        else if(Input.GetMouseButtonDown(1) && !slideViewing && !sceneChange)
        {
            if(Managers.Slides.haveSlide)
            {
                openSlideMenu(Managers.Slides.slideSet, Managers.Slides.slide);
            }
        }
        else if(Input.GetMouseButtonDown(1) && slideViewing)
        {
            closeSlideMenu();
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void openPauseMenu()
    {
        Managers.Pause();
        pausePanel.SetActive(true);
        pauseMenuOpen = true;
    }

    public void closePauseMenu()
    {
        Managers.Unpause();
        pausePanel.SetActive(false);
        pauseMenuOpen = false;
    }

    public void openSlideMenu(char initSet, int initSlide)
    {
        if(Managers.AI.spawned && Managers.AI.active.status == AIStatus.Chase || Managers.AI.spawned && Managers.AI.active.status == AIStatus.Pursue)
        {
            //TODO put a buzzer here or something
            return;
        }
        else
        {
            Managers.Slides.SetSlide(initSet, initSlide);
            Managers.Pause();
            slideViewing = true;
            Managers.Slides.switchSlide(initSet, initSlide);
            SwitchCamera(slideCam);
        }
    }

    public void closeSlideMenu()
    {
        Managers.Unpause();
        slideViewing = false;
        SwitchCamera(mainCam);
        Managers.Music.check();
    }

    public void SwitchCamera(Camera cam)
    {
        transitioning = true;
        targCam = cam;
    }

    public void CoverRoutine(float time)
    {
        StartCoroutine(Cover(time));
    }

    //Poorly named. Specifically the flash for being killed by hazard
    private IEnumerator Cover(float time)
    {
        solid.color = new Color(1f, 0.81f, 0.5f, 1f);
        float currTime = 0f;
        while(currTime < time)
        {
            currTime += Time.deltaTime;
            yield return null;
        }
        solid.color = new Color(0f, 0f, 0f, 0f);
        yield break;
    }
}
