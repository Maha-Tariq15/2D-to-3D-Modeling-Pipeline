using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationController : MonoBehaviour
{
    Animator animator;

    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float rotationSpeed = 720f;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;
    }

    void Update()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current != null)
        {
            input.x = (Keyboard.current.aKey.isPressed ? -1 : 0)
                    + (Keyboard.current.dKey.isPressed ? 1 : 0);

            input.y = (Keyboard.current.sKey.isPressed ? -1 : 0)
                    + (Keyboard.current.wKey.isPressed ? 1 : 0);
        }

        bool isMoving = input.sqrMagnitude > 0.01f;
        bool isRunning = isMoving && Keyboard.current.leftShiftKey.isPressed;

        animator.SetBool("isWalking", isMoving && !isRunning);
        animator.SetBool("isRunning", isRunning);

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (isRunning)
                animator.SetTrigger("RunJump");
            else
                animator.SetTrigger("IdleJump");
        }

        if (isMoving)
        {
            float speed = isRunning ? runSpeed : walkSpeed;
            Vector3 moveDir = new Vector3(input.x, 0, input.y).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            transform.position += moveDir * speed * Time.deltaTime;
        }
    }
}
