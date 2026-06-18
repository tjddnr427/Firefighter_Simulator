using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float timeLimit = 90f;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI scoreText;

    private float remainingTime;
    private int score;
    private bool isGameOver;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        remainingTime = timeLimit;
        score = 0;
        UpdateUI();
    }

    void Update()
    {
        if (isGameOver) return;

        remainingTime -= Time.deltaTime;
        UpdateUI();

        if (remainingTime <= 0f)
            GameOver();

        if (FindObjectsByType<FireZone>(FindObjectsSortMode.None).Length == 0)
            GameClear();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateUI();
    }

    public void GameOver()
    {
        isGameOver = true;
        Debug.Log("GAME OVER");
    }

    void GameClear()
    {
        isGameOver = true;
        Debug.Log("GAME CLEAR! Score: " + score);
    }

    void UpdateUI()
    {
        if (timeText != null) timeText.text = "Time: " + Mathf.CeilToInt(remainingTime) + "s";
        if (scoreText != null) scoreText.text = "Score: " + score;
    }
}
