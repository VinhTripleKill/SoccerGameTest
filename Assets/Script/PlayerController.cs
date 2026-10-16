using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator animator;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Boundary (World Position)")]
    [SerializeField] private float minX = -22.5f;
    [SerializeField] private float maxX = 22.5f;
    [SerializeField] private float minZ = -22.5f;
    [SerializeField] private float maxZ = 22.5f;

    private static readonly int IsRunningHash = Animator.StringToHash("isRunning");

    private Vector3 velocity;

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Transform body = controller.transform;

        Vector3 move = body.right * horizontal + body.forward * vertical;

        if (move.magnitude > 1f)
            move.Normalize();

        // Animation: đang có input di chuyển thì chạy, không thì idle
        bool isRunning = move.sqrMagnitude > 0.01f;
        if (animator != null) animator.SetBool(IsRunningHash, isRunning);

        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;

        Vector3 delta = move * moveSpeed * Time.deltaTime;
        delta.y = velocity.y * Time.deltaTime;

        Vector3 currentPos = body.position;
        float targetX = Mathf.Clamp(currentPos.x + delta.x, minX, maxX);
        float targetZ = Mathf.Clamp(currentPos.z + delta.z, minZ, maxZ);

        delta.x = targetX - currentPos.x;
        delta.z = targetZ - currentPos.z;

        controller.Move(delta);
    }
}