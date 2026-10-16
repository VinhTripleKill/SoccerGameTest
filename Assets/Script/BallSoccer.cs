using UnityEngine;
using System.Collections.Generic;
public class BallSoccer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GamePlayUI gamePlayUI;
    [SerializeField] private GoalVFX goalVFX;
    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] private GameObject soccer;

    [Header("Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string goalTag = "Goal";
    [SerializeField] private float logInterval = 1f;
    [SerializeField] private int score = 100;

    [Header("Kick")]
    [SerializeField] private float kickForce = 20f;

    private float stayTimer;
    private bool scored;
    // Danh sách tất cả quả bóng đang tồn tại trong scene
    private static readonly List<BallSoccer> AllBalls = new List<BallSoccer>();

    private Transform BallTransform => soccer != null ? soccer.transform : transform;

    private void OnEnable()
    {
        AllBalls.Add(this);
    }

    private void OnDisable()
    {
        AllBalls.Remove(this);
    }

    // Trả về quả bóng xa vị trí 'from' nhất (bỏ qua bóng đã ghi bàn)
    public static BallSoccer GetFarthestFrom(Vector3 from)
    {
        BallSoccer farthest = null;
        float maxDist = -1f;

        foreach (BallSoccer ball in AllBalls)
        {
            if (ball == null || ball.scored) continue;

            float dist = (ball.BallTransform.position - from).sqrMagnitude;
            if (dist > maxDist)
            {
                maxDist = dist;
                farthest = ball;
            }
        }
        return farthest;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayer(other)) return;

        stayTimer = 0f;
        Debug.Log("enter zone can kick ball");

        if (gamePlayUI != null) gamePlayUI.Show(this);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsPlayer(other)) return;

        stayTimer += Time.deltaTime;
        if (stayTimer >= logInterval)
        {
            stayTimer = 0f;
            Debug.Log("enter zone can kick ball");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsPlayer(other)) return;

        stayTimer = 0f;
        Debug.Log("exit zone can kick ball");

        if (gamePlayUI != null) gamePlayUI.Hide(this);
    }

    // Được GamePlayUI gọi khi bấm nút kickB
    public void Kick()
    {
        GameObject target = soccer != null ? soccer : gameObject;
        Rigidbody rb = target.GetComponent<Rigidbody>();
        if (rb == null) return;

        Transform goal = FindNearestGoal(target.transform.position);
        if (goal == null)
        {
            Debug.LogWarning("Không tìm thấy object nào có tag " + goalTag);
            return;
        }

        Vector3 direction = goal.position - target.transform.position;
        direction.y = 0f;
        direction.Normalize();

        rb.AddForce(direction * kickForce, ForceMode.Impulse);
        Debug.Log("Kick ball to " + goal.name);

        // Camera chuyển sang bám theo quả bóng vừa đá
        if (cameraFollow != null) cameraFollow.FollowBall(target.transform);
    }

    private Transform FindNearestGoal(Vector3 from)
    {
        GameObject[] goals = GameObject.FindGameObjectsWithTag(goalTag);
        Transform nearest = null;
        float minDist = float.MaxValue;

        foreach (GameObject g in goals)
        {
            float dist = (g.transform.position - from).sqrMagnitude;
            if (dist < minDist)
            {
                minDist = dist;
                nearest = g.transform;
            }
        }
        return nearest;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (scored) return;
        if (!collision.collider.CompareTag(goalTag)) return;

        scored = true;

        if (gamePlayUI != null)
        {
            gamePlayUI.AddScore(score);
            gamePlayUI.Hide(this);
        }

        Debug.Log("Goal! +" + score);

        GameObject target = soccer != null ? soccer : gameObject;

        if (goalVFX != null) goalVFX.Play(target.transform.position);

        // Camera đứng yên 2 giây tại điểm bóng vào gôn rồi mới lerp về player
        if (cameraFollow != null)
            cameraFollow.OnBallScored(target.transform, target.transform.position);

        Destroy(target);
    }

    private bool IsPlayer(Collider other)
    {
        Transform t = other.transform;
        while (t != null)
        {
            if (t.CompareTag(playerTag)) return true;
            t = t.parent;
        }
        return false;
    }
}