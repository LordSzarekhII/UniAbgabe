using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private CharacterController controller;
    private Weapon weapon;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponentInParent<CharacterController>();
        weapon = GetComponentInParent<PlayerController>()?.GetComponentInChildren<Weapon>();
    }

    private void OnEnable()
    {
        PlayerHealth.OnPlayerDied += HandleDeath;
        Weapon.OnReloadStarted += HandleReload;
    }

    private void OnDisable()
    {
        PlayerHealth.OnPlayerDied -= HandleDeath;
        Weapon.OnReloadStarted -= HandleReload;
    }

    private void Update()
    {
        if (animator == null) return;

        Vector3 horizontalVelocity = controller.velocity;
        horizontalVelocity.y = 0f;
        float speed = horizontalVelocity.magnitude;

        var keyboard = UnityEngine.InputSystem.Keyboard.current;
        bool isSprinting = keyboard != null && keyboard.leftShiftKey.isPressed && speed > 0.1f;

        animator.SetFloat("Speed", speed);
        animator.SetBool("IsGrounded", controller.isGrounded);
        animator.SetBool("IsShooting", weapon != null && weapon.IsShooting);
        animator.SetBool("IsReloading", weapon != null && weapon.IsReloading);
        animator.SetBool("IsSprinting", isSprinting);
    }

    private void HandleDeath()
    {
        if (animator != null)
            animator.SetTrigger("Die");
    }

    private void HandleReload()
    {
        if (animator != null)
            animator.SetTrigger("Reload");
    }
}
