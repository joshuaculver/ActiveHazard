using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[AddComponentMenu("Control Script/Player Input")]
public class PlayerInput : MonoBehaviour
{
    public float speed = 3f;
    //Default speed
    public float defSpeed = 3f;
    public float gravity;
    public float runSpeedMult = 2.15f;
    public float stamina = 600f; 
    public float staminaCap = 600f;

    public bool canMove;
    public bool moving = false;
    public bool sprinting = false;

    private CharacterController _charController;
    // Start is called before the first frame update
    void Start()
    {
        _charController = GetComponent<CharacterController>();
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(canMove)
        {
            if(Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
            {
                moving = true;
            }
            else
            {
                moving = false;
            }

            Vector3 movement;

            if(stamina > 5f && Input.GetKey(KeyCode.LeftShift))
            {
                float deltaX = Input.GetAxis("Horizontal") * speed * runSpeedMult;
                float deltaZ = Input.GetAxis("Vertical") * speed * runSpeedMult;
                movement = new Vector3(deltaX, 0, deltaZ);

                movement = Vector3.ClampMagnitude(movement, speed * runSpeedMult);

                stamina -= 1f;
                
                sprinting = true;
            }
            else
            {
                float deltaX = Input.GetAxis("Horizontal") * speed;
                float deltaZ = Input.GetAxis("Vertical") * speed;
                movement = new Vector3(deltaX, 0, deltaZ);

                movement = Vector3.ClampMagnitude(movement, speed);

                if(stamina < staminaCap)
                {
                    stamina += 0.5f;
                }
                sprinting = false;
            }

            movement.y = gravity;

            movement *= Time.deltaTime;
            movement = transform.TransformDirection(movement);
            _charController.Move(movement);
        }
        else
        {
           Vector3 movement = new Vector3(0, 0, 0);
            _charController.Move(movement);
        }
    }

    public void ChangeSpeed(float spdMult)
    {
        if(sprinting)
        {
            speed = defSpeed * spdMult * runSpeedMult;
        }
        else
        {
            speed = defSpeed * spdMult;
        }
    }

    public void ResetSpeed()
    {
        speed = defSpeed;
    }
}
