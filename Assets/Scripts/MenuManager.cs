using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status {get; private set;}
    public bool pauseMenuOpen = false;
    public GameObject pausePanel;
    public Button resume;
    public Button mainMenu;

    void Awake()
    {
        pausePanel.SetActive(false);
    }
    void Update()
    {
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
}
