using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("UI Elements")] [SerializeField]
    private Slider volumeSlider;
    [SerializeField] private GameObject pauseMenu;
    [Header("Results")] [SerializeField] private ResultsUI resultsUI;
    [Header("Settings")] [SerializeField] private bool Pausable = true;
    [SerializeField] AudioMixerGroup mixer;
    
    private InputAsset _input;
    private bool isPaused = false;
    private const string VolumePrefKey = "MusicVolume";
    
    private void Awake()
    {
        _input = new InputAsset();
    }

    private void Start()
    {
        Time.timeScale = 1f;
        float savedVolume = PlayerPrefs.GetFloat(VolumePrefKey, 0f);

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(UpdateVolume);
        }

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        UpdateVolume(savedVolume);
    }

    private void OnEnable()
    {
        _input.UI.Pause.performed += OnPausePressed;
        _input.UI.Enable();
    }

    private void OnDisable()
    {
        _input.UI.Pause.performed -= OnPausePressed;
        _input.UI.Disable();
    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        if (Pausable)
        {
            TogglePause();
        }
        else
        {
            // Debug.Log("Pause is disabled for this scene.");
        }
    }

    public void UpdateVolume(float volume)
    {
        float volumeDb = (volume > 0.0001f) ? Mathf.Log10(volume) * 20f : -80f;
        mixer.audioMixer.SetFloat("MasterVolume", volumeDb);
        PlayerPrefs.SetFloat(VolumePrefKey, volume);
        PlayerPrefs.Save();
        Debug.Log($"Volume updated to {volume}");
    }

    public void ExitApplication()
    {
        Application.Quit();
    }

    public void TogglePause()
    {
        isPaused = !isPaused; // Переключаем состояние паузы

        Time.timeScale = isPaused ? 0 : 1;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(isPaused);
        }

        Debug.Log($"TogglePause: isPaused = {isPaused}, Time.timeScale = {Time.timeScale}");
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) 
        {
            TogglePause();
        }
    }
    public void ResetStats()
    {
        GameStats.Instance.ResetStats();
    }

    public void LoadStats()
    {
        GameStats.Instance.LoadStats();
        if (resultsUI != null)
            resultsUI.UpdateResults();
    }
    public void LoadScene(int sceneIndex)
    {
        SceneLoader.LoadScene(sceneIndex);
    }
}
