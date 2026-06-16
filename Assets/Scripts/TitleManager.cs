using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public void OnStartGame()
    {
        SceneManager.LoadScene("PlayScene");
    }

    public void OnQuitGame()
    {
        Application.Quit();
    }
}
