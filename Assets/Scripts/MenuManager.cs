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
    public RawImage solid;
    public UnityEngine.UIElements.Button resume;
    public UnityEngine.UIElements.Button mainMenu;

    public Camera mainCam;
    public Camera slideCam;

    public bool playerDead = false;

    void Awake()
    {
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
        Managers.Player.Hold();
        pauseMenuOpen = true;
    }

    public void closePauseMenu()
    {
        Managers.Unpause();
        pausePanel.SetActive(false);
        Managers.Player.Release();
        pauseMenuOpen = false;
    }

    public void CoverRoutine(float time)
    {
        StartCoroutine(Cover(time));
    }

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
