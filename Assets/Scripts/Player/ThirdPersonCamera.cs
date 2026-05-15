using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform target;
    public Vector3 offset = new Vector3(0.5f, 2f, -3.5f);
    public float mouseSensitivity = 0.05f;
    public float minVerticalAngle = -30f;
    public float maxVerticalAngle = 60f;
    public float smoothSpeed = 10f;
    public float collisionRadius = 0.2f;
    public LayerMask collisionMask = ~0;

    private float verticalAngle = 10f;
    private float horizontalAngle = 0f;
    private Weapon weapon;

    private void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }
        if (target != null)
        {
            horizontalAngle = target.eulerAngles.y + 90;
            verticalAngle = target.eulerAngles.x;
            weapon = target.GetComponentInChildren<Weapon>();
        }
    }
    
    // LateUpdate is used to ensure the camera updates after all player movement and rotation has been processed
    private void LateUpdate()
    {

        bool gameActive = GameManager.Instance != null && GameManager.Instance.CurrentState == GameManager.GameState.Playing;

        // Only allow camera rotation when the game is active and the player is not aiming
        if (Mouse.current != null && gameActive)
        {
            horizontalAngle += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            verticalAngle -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            //lock the vertical angle to prevent looking up or down
            verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle);
        }

        // Calculate the desired camera position based on the target's position and the current rotation angles
        Quaternion rotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0f);
        Vector3 desiredPosition = target.position + rotation * offset;
        Vector3 lookTarget = target.position + Vector3.up * 1.5f;

        // Prevent camera from clipping through walls
        Vector3 directionFromTarget = desiredPosition - lookTarget;
        float desiredDistance = directionFromTarget.magnitude;

        // SphereCast to check for collisions along the path from the target to the desired camera position
        if (Physics.SphereCast(lookTarget, collisionRadius, directionFromTarget.normalized, out RaycastHit hit, desiredDistance, collisionMask))
        {
            desiredPosition = lookTarget + directionFromTarget.normalized * (hit.distance - collisionRadius);
        }

        transform.position = desiredPosition;
        transform.LookAt(lookTarget);
    }

    public float HorizontalAngle => horizontalAngle;
}
