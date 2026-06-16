using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private float timeLimit = 90f;
    [SerializeField] private Text timeText;
    [SerializeField] private Text scoreText;

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
        if (timeText != null) timeText.text = "시간: " + Mathf.CeilToInt(remainingTime) + "초";
        if (scoreText != null) scoreText.text = "점수: " + score;
    }
}
