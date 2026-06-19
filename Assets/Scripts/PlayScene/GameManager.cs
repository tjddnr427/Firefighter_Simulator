using UnityEngine;
using UnityEngine.SceneManagement;
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
        PlayerPrefs.SetString("EndResult", "GAME OVER");
        PlayerPrefs.SetInt("FinalScore", score);
        SceneManager.LoadScene("EndScene");
    }

    void GameClear()
    {
        isGameOver = true;
        PlayerPrefs.SetString("EndResult", "GAME CLEAR!");
        PlayerPrefs.SetInt("FinalScore", score);
        SceneManager.LoadScene("EndScene");
    }

    void UpdateUI()
    {
        if (timeText != null) timeText.text = "Time: " + Mathf.CeilToInt(remainingTime) + "s";
        if (scoreText != null) scoreText.text = "Score: " + score;
    }
}
