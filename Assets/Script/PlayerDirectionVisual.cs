using UnityEngine;

public class PlayerDirectionVisual : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private GameObject visualModel;
    [SerializeField] private float visualRot = 10f;

    private Vector3 lastPosition;

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void LateUpdate()
    {
        // Quãng đường di chuyển trong frame này (bỏ trục Y để không bị ảnh hưởng bởi trọng lực)
        Vector3 delta = transform.position - lastPosition;
        delta.y = 0f;
        lastPosition = transform.position;

        if (visualModel == null) return;

        // Chỉ xoay khi player thực sự di chuyển
        if (delta.sqrMagnitude > 0.00001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(delta.normalized, Vector3.up);

            visualModel.transform.rotation = Quaternion.Slerp(
                visualModel.transform.rotation,
                targetRotation,
                visualRot * Time.deltaTime
            );
        }
    }
}