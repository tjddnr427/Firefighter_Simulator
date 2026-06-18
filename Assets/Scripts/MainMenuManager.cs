using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject controlsPanel;
    public Slider bgmSlider;
    public Slider sfxSlider;

    void Start()
    {
        settingsPanel.SetActive(false);
        controlsPanel.SetActive(false);

        if (bgmSlider != null) bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);
        if (sfxSlider != null) sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    public void OnStartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void OnOpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void OnCloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void OnBGMVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("BGMVolume", value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void OnOpenControls()
    {
        controlsPanel.SetActive(true);
    }

    public void OnCloseControls()
    {
        controlsPanel.SetActive(false);
    }

    public void OnQuitGame()
    {
        Application.Quit();
    }
}
