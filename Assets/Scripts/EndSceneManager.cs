using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndSceneManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    void Start()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + PlayerPrefs.GetInt("FinalScore", 0);
    }

    public void OnGoToTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
