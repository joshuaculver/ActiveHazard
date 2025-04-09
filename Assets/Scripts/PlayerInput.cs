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

    public bool canMove;
    public bool moving = false;

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
            float deltaX = Input.GetAxis("Horizontal") * speed;
            float deltaZ = Input.GetAxis("Vertical") * speed;
            Vector3 movement = new Vector3(deltaX, 0, deltaZ);
            movement = Vector3.ClampMagnitude(movement, speed);

            movement.y = gravity;

            movement *= Time.deltaTime;
            movement = transform.TransformDirection(movement);
            _charController.Move(movement);
        }
    }

    public void ChangeSpeed(float spdMult)
    {
        speed = defSpeed * spdMult;
    }

    public void ResetSpeed()
    {
        speed = defSpeed;
    }
}
