using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class PlayerManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status {get; private set;}
    public GameObject playerPrefab;
    public Transform playerSpawn;
    public GameObject player;
    public bool playerInput;
    public bool interacting;
    public float interactRange = 5f;
    public Queue<Transform> nearNodes;

    public Camera cam;
    public PlayerAudEmitter aud;
    public Animator playerAnimator; 
    public PlayerInput pMove;
    private MouseLook pCamX;
    private MouseLook pCamY;
    private DecalProjector projector;
    public Animation camAnim;
    public bool gameOver = false;

    public float normalFOV = 70;

    public RawImage icon;
    public Texture handsIcon;
    public Texture eyeIcon;

    private float iconFadeSpd = 2f;

    private HeadBob bob;

    public void Startup()
    {
        Debug.Log("Player manager starting...");
        SpawnPlayer();
        playerAnimator = player.GetComponentInChildren<Animator>();
        cam = player.GetComponentInChildren<Camera>();
        camAnim = cam.GetComponentInParent<Animation>();
        pMove = player.GetComponent<PlayerInput>();
        pCamX = player.GetComponent<MouseLook>();
        pCamY = cam.GetComponentInParent<MouseLook>();
        playerAnimator = playerAnimator.GetComponent<Animator>();
        projector = player.GetComponentInChildren<DecalProjector>();
        aud = player.GetComponentInChildren<PlayerAudEmitter>();
        bob = player.GetComponentInChildren<HeadBob>();
        playerInput = true;

        nearNodes = new Queue<Transform>();

        status = ManagerStatus.Started;
        Debug.Log("Setting bob: " + Managers.headBob.ToString());

        bob.enable = Managers.headBob;
    }

    public void Update()
    {
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
            
            if(playerInput)
            {
                Ray ray = cam.ViewportPointToRay(new Vector3(.5f, 0.5f, 0));
                RaycastHit hit;

                if(Physics.Raycast(ray, out hit, interactRange))
                {
                    Interactable interactable = hit.collider.GetComponent<Interactable>();
                    if(interactable != null)
                    {
                        if(icon.color.a < 1f)
                        {
                            icon.color = new Color(1f, 1f, 1f, Mathf.MoveTowards(icon.color.a, 1f, Time.unscaledDeltaTime * 2f));
                        }
                    }
                    else
                    {
                        if(icon.color.a > 0f)
                        {
                            icon.color = new Color(1f, 1f, 1f, Mathf.MoveTowards(icon.color.a, 0f, Time.unscaledDeltaTime * 2f));
                        }
                    }
                }
                else
                {
                    if(icon.color.a > 0f)
                    {
                        icon.color = new Color(1f, 1f, 1f, Mathf.MoveTowards(icon.color.a, 0f, Time.unscaledDeltaTime * 2f));
                    }
                }

                if(Input.GetMouseButtonDown(0))
                {
                    if(Physics.Raycast(ray, out hit, interactRange))
                    {
                        Interactable interactable = hit.collider.GetComponent<Interactable>();
                        Debug.Log(interactable);
                        if(interactable != null)
                        {
                            Debug.Log("Interacting");
                            interactable.Interact();
                        }
                    }
                }
            }
            else
            {
                icon.color = new Color(1f, 1f, 1f, Mathf.MoveTowards(icon.color.a, 0f, Time.unscaledDeltaTime * 2.75f));
            }
            /*
            if(Input.GetKeyDown(KeyCode.F))
            {
                Die();
            }
            if(Input.GetKeyDown(KeyCode.T))
            {
                Managers.AI.danger += 1;
                Managers.AI.DangerCheck();
                Debug.Log("DEBUG increasing danger - " + Managers.AI.danger.ToString());
            }
            if(Input.GetKeyDown(KeyCode.G))
            {
                Managers.AI.danger -= 1;
                Managers.AI.DangerCheck();
                Debug.Log("DEBUG lowering danger - " + Managers.AI.danger.ToString());
            }
            if(Input.GetKeyDown(KeyCode.H))
            {
                Managers.State.OpenExit();
            }
            */
    }

    public void SpawnPlayer()
    {
        Debug.Log("Spawning player");
        //Euler controls facing/orientation
        player = Instantiate(playerPrefab, playerSpawn.position, Quaternion.Euler(new Vector3(0, 180, 0)));
    }

    public void Hold()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        pMove.canMove = false;
        pCamX.canMove = false;
        pCamY.canMove = false;
        playerInput = false;
    }

    public void Release()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        pMove.canMove = true;
        pCamX.canMove = true;
        pCamY.canMove = true;
        playerInput = true;

        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }

    public void Die()
    {
        gameOver = true;
        pMove.moving = false;
        Hold();
        bob.enable = false;
        Animation anim = camAnim;
        Managers.Menu.CoverRoutine(.15f);
        projector.enabled = true;
        anim.Play();
        StartCoroutine(DeathWait(3f));
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
        Managers.Menu.playerDead = true;
        yield break;
    }

    public void Attacked()
    {
        aud.queFly();
    }

    public void canInteract(bool set)
    {
        if(set)
        {
            icon.texture = handsIcon;
        }
        else
        {
            icon.texture = eyeIcon;
        }
    }
    public void HeadBobToggle(bool set)
    {
        if(set == true)
        {
            if(Managers.headBob == true)
            {
                bob.enable = set;
            }
            else
            {
                bob.enable = false;
            }
        }
        else
        {
            bob.enable = set;
        }
    }
}
