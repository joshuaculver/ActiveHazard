using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status {get; private set;}
    public GameObject playerPrefab;
    public Transform playerSpawn;
    public GameObject player;
    //public GameObject cam;
    public Camera cam;
    public PlayerAudEmitter aud;
    public Animator playerAnimator; 
    private PlayerInput pMove;
    private MouseLook pCamX;
    private MouseLook pCamY;
    //private Vector3 neutral = new Vector3(0, 0, 0);
    private DecalProjector projector;
    public RawImage black;
    public bool gameOver = false;
    public bool dead = false;
    public bool titleMode;

    public float normalFOV = 60;

    public void Startup()
    {
        Debug.Log("Player manager starting...");
        SpawnPlayer();
        playerAnimator = player.GetComponentInChildren<Animator>();
        cam = player.GetComponentInChildren<Camera>();
        pMove = player.GetComponent<PlayerInput>();
        pCamX = player.GetComponent<MouseLook>();
        pCamY = cam.GetComponent<MouseLook>();
        playerAnimator = playerAnimator.GetComponent<Animator>();
        projector = player.GetComponentInChildren<DecalProjector>();
        aud = player.GetComponentInChildren<PlayerAudEmitter>();

        status = ManagerStatus.Started;
    }

    public void Update()
    {
        if(dead)
        {
            Debug.Log("Dead");
            if(black.color.a < 1f)
            {
                Debug.Log("Fading");
                black.color = new Color(0f, 0f, 0f, Mathf.MoveTowards(black.color.a, 1f, 0.3f * Time.deltaTime));
            }
            else
            {
                SceneManager.LoadScene("MainMenu");
            }
            return;
        }
        else
        {
            /*
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                SceneManager.LoadScene("MainMenu");
            }
            */
            if(pMove.moving)
                {
                    playerAnimator.SetBool("IsWalking", true);
                    aud.running = true;
                }
            else if(!pMove.moving)
                {
                    playerAnimator.SetBool("IsWalking", false);
                    aud.running = false;
                }
            
            if(Input.GetKeyDown(KeyCode.F))
            {
                Die();
            }
            if(Input.GetKeyDown(KeyCode.G))
            {
                Attacked();
            }
        }
    }

    public void SpawnPlayer()
    {
        Debug.Log("Spawning player");
        player = Instantiate(playerPrefab, playerSpawn.position, Quaternion.Euler(new Vector3(0, 90, 0)));
        if(titleMode)
        {
            Hold();
        }
    }

    public void Hold()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        pMove.canMove = false;
        pCamX.canMove = false;
        pCamY.canMove = false;

    }

    public void Release()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        pMove.canMove = true;
        pCamX.canMove = true;
        pCamY.canMove = true;
    }

    public void Die()
    {
        gameOver = true;
        Hold();
        Animation anim = cam.GetComponent<Animation>();
        StartCoroutine(Cover(.15f));
        projector.enabled = true;
        anim.Play();
        StartCoroutine(DeathWait(3f));
    }

    public IEnumerator Cover(float time)
    {
        black.color = new Color(1f, 0.81f, 0.5f, 1f);
        float currTime = 0f;
        while(currTime < time)
        {
            currTime += Time.deltaTime;
            yield return null;
        }
        black.color = new Color(0f, 0f, 0f, 0f);
        yield break;
    }

    public IEnumerator DeathWait(float seconds)
    {
        Debug.Log("Death wait...");
        float currTime = 0f;
        while(currTime < seconds)
        {
            currTime += Time.deltaTime;
            yield return null;
        }
        Debug.Log("Death wait done");
        dead = true;
        yield break;
    }

    public void Attacked()
    {
        aud.queFly();
    }
}
