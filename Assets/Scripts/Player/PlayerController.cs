using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 6f;
    public float sprintSpeed = 10f;
    public float gravity = -20f;
    public float jumpForce = 8f;

    private CharacterController controller;
    private float verticalVelocity;
    private ThirdPersonCamera thirdPersonCamera;
    private Weapon weapon;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        thirdPersonCamera = Camera.main?.GetComponent<ThirdPersonCamera>();
        weapon = GetComponentInChildren<Weapon>();
    }

    private void Update()
    {
        HandleMovement();
    }

    

    private void HandleMovement()
    {
        

        float horizontal = 0f;
        float vertical = 0f;

       
        if (Input.GetKey(KeyCode.W)) vertical += 1f;
        if (Input.GetKey(KeyCode.A)) horizontal -= 1f;
        if (Input.GetKey(KeyCode.S)) vertical -= 1f;
        if (Input.GetKey(KeyCode.D)) horizontal += 1f;

        // Move relative to camera direction
        float camYaw = thirdPersonCamera != null ? thirdPersonCamera.HorizontalAngle : transform.eulerAngles.y;
        Quaternion camRotation = Quaternion.Euler(0f, camYaw, 0f);
        Vector3 move = camRotation * new Vector3(horizontal, 0f, vertical);
        if (move.sqrMagnitude > 1f) move.Normalize();

        // Rotate player to face movement direction, or camera direction when shooting
        bool isShooting = weapon != null && weapon.IsShooting;
        if (isShooting)
        {
            //float camYawAim = thirdPersonCamera != null ? thirdPersonCamera.HorizontalAngle : transform.eulerAngles.y;
            Quaternion aimRotation = Quaternion.Euler(0f, camYaw, 0f);
            transform.rotation = aimRotation;
        }
        
        else if (move.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }


        // Determine speed based on sprinting
        Vector3 velocity;
        if(Input.GetKey(KeyCode.LeftShift))
            velocity = move * sprintSpeed;

        else
            velocity = move * walkSpeed;

        // Handle jumping and gravity
        if (controller.isGrounded)
        {
            verticalVelocity = -2f;

            if (Input.GetKey(KeyCode.Space))
            {
                verticalVelocity = jumpForce;
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }
}
