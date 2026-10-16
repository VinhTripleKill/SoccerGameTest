using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GamePlayUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Buttons")]
    [SerializeField] private GameObject kickB;
    [SerializeField] private GameObject resetB;
    [SerializeField] private GameObject autoKickB;

    [Header("Score")]
    [SerializeField] private TextMeshProUGUI scoreT;

    private int score = 0;

    // Quả bóng mà player đang đứng trong vùng đá
    private BallSoccer currentBall;

    private void Awake()
    {
        SetButtonsActive(false);

        if (resetB != null)
        {
            Button resetButton = resetB.GetComponent<Button>();
            if (resetButton != null)
                resetButton.onClick.AddListener(ReloadScene);
        }

        if (kickB != null)
        {
            Button kickButton = kickB.GetComponent<Button>();
            if (kickButton != null)
                kickButton.onClick.AddListener(OnKickPressed);
        }

        if (autoKickB != null)
        {
            Button autoKickButton = autoKickB.GetComponent<Button>();
            if (autoKickButton != null)
                autoKickButton.onClick.AddListener(OnAutoKickPressed);
        }

        UpdateScoreUI();
    }

    // Bóng gọi khi player vào vùng đá
    public void Show(BallSoccer ball)
    {
        currentBall = ball;
        SetButtonsActive(true);
    }

    // Bóng gọi khi player ra khỏi vùng (hoặc bóng bị hủy)
    public void Hide(BallSoccer ball)
    {
        if (currentBall != ball) return;

        currentBall = null;
        SetButtonsActive(false);
    }

    private void OnKickPressed()
    {
        if (currentBall != null) currentBall.Kick();
    }

    // Auto kick: đá quả bóng xa player nhất về phía gôn gần nhất
    private void OnAutoKickPressed()
    {
        if (player == null)
        {
            Debug.LogWarning("GamePlayUI chưa được gán Player");
            return;
        }

        BallSoccer farthest = BallSoccer.GetFarthestFrom(player.position);
        if (farthest == null)
        {
            Debug.Log("Không còn quả bóng nào để auto kick");
            return;
        }

        farthest.Kick();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void UpdateScoreUI()
    {
        if (scoreT != null) scoreT.text = score.ToString();
    }

    // Chỉ kickB phụ thuộc vào việc player đứng trong vùng đá
    private void SetButtonsActive(bool isActive)
    {
        if (kickB != null) kickB.SetActive(isActive);
    }
}