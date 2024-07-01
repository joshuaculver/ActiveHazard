using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status {get; private set;}
    public bool pauseMenuOpen = false;
    public GameObject pausePanel;
    public bool slideViewing = false;
    public RawImage solid;
    public UnityEngine.UIElements.Button resume;
    public UnityEngine.UIElements.Button mainMenu;
    public int slideSet;
    public int slide;

    private Camera mainCam;
    public Camera slideCam;
    private Camera targCam;

    private bool transitioning = false;

    public bool playerDead = false;

    //Slide image
    //Slide background

    void Awake()
    {
        mainCam = Managers.Player.cam;
        mainCam.enabled = true;
        slideCam.enabled = false;
        pausePanel.SetActive(false);
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
        if(transitioning)
        {
            if(solid.color.a < 1f)
            {
                Debug.Log("Fading to black");
                solid.color = new Color(0f, 0f, 0f, Mathf.MoveTowards(solid.color.a, 1f, 2f * Time.unscaledDeltaTime));
            }
            else if(solid.color.a >  0.9f)
            {
                Debug.Log("Switching camera");
                //switch cameras
                if(targCam == mainCam)
                {
                    mainCam.enabled = true;
                    slideCam.enabled = false;
                }
                else if(targCam == slideCam)
                {
                    mainCam.enabled = false;
                    slideCam.enabled = true;
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
        else if(!transitioning && solid.color.a > 0f)
        {
            Debug.Log("Fading from black");
            solid.color = new Color(0f, 0f, 0f, Mathf.MoveTowards(solid.color.a, 0f, 2f * Time.unscaledDeltaTime));
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
        else if(Input.GetKeyDown(KeyCode.N))
        {
            openSlideMenu(0,0);
        }
        else if(Input.GetKeyDown(KeyCode.M))
        {
            closeSlideMenu();
        }
    }

    public void Startup()
    {
        status = ManagerStatus.Started;
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

    public void openSlideMenu(int initSet, int initSlide)
    {
        if(Managers.AI.spawned && Managers.AI.active.status == AIStatus.Chase || Managers.AI.spawned && Managers.AI.active.status == AIStatus.Pursue)
        {
            //TODO put a buzzer here or something
            return;
        }
        else
        {
            Managers.Pause();
            //Setup or switch slides here
            SwitchCamera(slideCam);
            slideViewing = true;
            slideSet = initSet;
            slide = initSlide;
            Managers.Music.check();
        }
    }

    public void closeSlideMenu()
    {
        Managers.Unpause();
        //Setup or switch slides here
        SwitchCamera(mainCam);
        slideViewing = false;
        Managers.Music.check();
    }

    public void switchSlide(int newSet, int newSlide)
    {
        slideSet = newSet;
        slide = newSlide;
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
