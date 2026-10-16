using UnityEngine;

public class GoalDebugRay : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string goalTag = "Goal";
    [SerializeField] private Color rayColor = Color.red;

    private GameObject[] goals;

    private void Start()
    {
        goals = GameObject.FindGameObjectsWithTag(goalTag);
    }

    private void Update()
    {
        Transform nearest = FindNearestGoal();
        if (nearest == null) return;

        // Tia đỏ nối từ object gắn script tới gôn gần nhất, cập nhật mỗi frame
        Debug.DrawLine(transform.position, nearest.position, rayColor);
    }

    private Transform FindNearestGoal()
    {
        if (goals == null) return null;

        Transform nearest = null;
        float minDist = float.MaxValue;

        foreach (GameObject g in goals)
        {
            if (g == null) continue; // Gôn có thể đã bị hủy

            float dist = (g.transform.position - transform.position).sqrMagnitude;
            if (dist < minDist)
            {
                minDist = dist;
                nearest = g.transform;
            }
        }
        return nearest;
    }
}