using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(NameofManager))]
[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(AIManager))]
[RequireComponent(typeof(MusicManager))]
[RequireComponent(typeof(MenuManager))]
[RequireComponent(typeof(SlideManager))]


public class Managers : MonoBehaviour
{
    public static PlayerManager Player {get; private set;}
    public static AIManager AI {get; private set;}
    public static MusicManager Music {get; private set;}
    public static MenuManager Menu {get; private set;}
    public static SlideManager Slides {get; private set;}
    public static StateManager State {get; private set;}

    private List<IGameManager> _startSequence;

    public static bool isPaused = false;

    void Awake()
    {
        Player = GetComponent<PlayerManager>();
        AI = GetComponent<AIManager>();
        Music = GetComponent<MusicManager>();
        Menu = GetComponent<MenuManager>();
        Slides = GetComponent<SlideManager>();
        State = GetComponent<StateManager>();

        _startSequence = new List<IGameManager>();
        _startSequence.Add(Player);
        _startSequence.Add(AI);
        _startSequence.Add(Music);
        _startSequence.Add(Menu);
        _startSequence.Add(Slides);
        _startSequence.Add(State);

        StartCoroutine(StartupManagers());
    }

    private IEnumerator StartupManagers()
    {
        foreach (IGameManager manager in _startSequence)
        {
            manager.Startup();
        }

        yield return null;

        int numModules = _startSequence.Count;
        int numReady = 0;

        while (numReady < numModules)
        {
            int lastReady = numReady;
            numReady = 0;

            foreach (IGameManager manager in _startSequence)
            {
                if (manager.status == ManagerStatus.Started)
                {
                    numReady++;
                }
            }

            if (numReady > lastReady)
                Debug.Log("Progress: " + numReady + "/" + numModules);

            yield return null;
        }

        Debug.Log("All managers started");
    }
    public static void Pause()
    {
        Debug.Log("Pausing");
        isPaused = true;
        Managers.Player.Hold();
        Time.timeScale = 0f;
    }

    public static void Unpause()
    {
        Debug.Log("Unpausing");
        isPaused = false;
        Managers.Player.Release();
        Time.timeScale = 1f;
    }
}
