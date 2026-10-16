using UnityEngine;

public class GoalVFX : MonoBehaviour
{
    [Header("VFX")]
    [SerializeField] private ParticleSystem goalVFXprefab;

    // Spawn VFX tại vị trí truyền vào rồi tự hủy khi chạy xong
    public void Play(Vector3 position)
    {
        if (goalVFXprefab == null) return;

        ParticleSystem vfx = Instantiate(goalVFXprefab, position, Quaternion.identity);
        vfx.Play();

        // Hủy sau khi particle chạy xong (thời gian + tuổi thọ lớn nhất của hạt)
        var main = vfx.main;
        float lifeTime = main.duration + main.startLifetime.constantMax;
        Destroy(vfx.gameObject, lifeTime);
    }
}