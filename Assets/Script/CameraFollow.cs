using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public enum CamMode
    {
        TopDown,
        ThirdPerson
    }

    private enum FollowState
    {
        Following,  // Đang bám theo target hiện tại (player hoặc bóng)
        Holding,    // Bóng đã vào gôn, camera đứng yên chờ
        Returning   // Đang lerp về player
    }

    [Header("References")]
    [SerializeField] private Transform target; // Player (target mặc định)

    [Header("Mode")]
    [SerializeField] private CamMode currentMode = CamMode.TopDown;
    [SerializeField] private KeyCode switchKey = KeyCode.C;

    [Header("Third-Person Settings")]
    [SerializeField] private Vector3 thirdPersonOffset = new Vector3(0f, 5.6f, -3.5f);
    [SerializeField] private Vector3 thirdPersonRotation = new Vector3(60f, 0f, 0f);

    [Header("Top-Down Settings")]
    [SerializeField] private Vector3 topDownOffset = new Vector3(0f, 5f, 0f);
    [SerializeField] private Vector3 topDownRotation = new Vector3(90f, 0f, 0f);

    [Header("Ball Follow")]
    [SerializeField] private float holdDuration = 2f;   // Thời gian đứng yên sau khi bóng vào gôn
    [SerializeField] private float returnSmooth = 3f;   // Tốc độ lerp về player (càng lớn càng nhanh)

    private Transform currentTarget;
    private FollowState state = FollowState.Following;
    private Vector3 focusPoint;
    private float holdTimer;

    private void Start()
    {
        currentTarget = target;
        if (target != null) focusPoint = target.position;

        Debug.Log("Current Camera Mode: " + currentMode);
        ApplyCameraTransform();
    }

    private void Update()
    {
        if (Input.GetKeyDown(switchKey))
        {
            SwitchMode();
        }
    }

    private void LateUpdate()
    {
        UpdateFocusPoint();
        ApplyCameraTransform();
    }

    // Được BallSoccer gọi khi bấm kick: camera chuyển sang bám theo quả bóng
    public void FollowBall(Transform ball)
    {
        if (ball == null) return;

        currentTarget = ball;
        state = FollowState.Following;
    }

    // Được BallSoccer gọi khi bóng vào gôn và bị destroy
    public void OnBallScored(Transform ball, Vector3 lastPosition)
    {
        // Chỉ xử lý nếu đó là quả bóng camera đang theo
        if (currentTarget != ball) return;

        focusPoint = lastPosition;
        currentTarget = null;
        holdTimer = holdDuration;
        state = FollowState.Holding;
    }

    private void UpdateFocusPoint()
    {
        switch (state)
        {
            case FollowState.Following:
                if (currentTarget != null)
                {
                    focusPoint = currentTarget.position;
                }
                else
                {
                    // Target bị hủy bất ngờ: giữ nguyên vị trí rồi quay về player
                    holdTimer = holdDuration;
                    state = FollowState.Holding;
                }
                break;

            case FollowState.Holding:
                holdTimer -= Time.deltaTime;
                if (holdTimer <= 0f) state = FollowState.Returning;
                break;

            case FollowState.Returning:
                if (target == null) break;

                // Lerp không phụ thuộc framerate
                float t = 1f - Mathf.Exp(-returnSmooth * Time.deltaTime);
                focusPoint = Vector3.Lerp(focusPoint, target.position, t);

                if ((focusPoint - target.position).sqrMagnitude < 0.0025f)
                {
                    currentTarget = target;
                    state = FollowState.Following;
                }
                break;
        }
    }

    private void SwitchMode()
    {
        currentMode = (currentMode == CamMode.TopDown) ? CamMode.ThirdPerson : CamMode.TopDown;
        Debug.Log("Current Camera Mode: " + currentMode);
    }

    private void ApplyCameraTransform()
    {
        if (target == null && currentTarget == null && state == FollowState.Following) return;

        switch (currentMode)
        {
            case CamMode.TopDown:
                transform.position = focusPoint + topDownOffset;
                transform.rotation = Quaternion.Euler(topDownRotation);
                break;

            case CamMode.ThirdPerson:
                transform.position = focusPoint + thirdPersonOffset;
                transform.rotation = Quaternion.Euler(thirdPersonRotation);
                break;
        }
    }
}