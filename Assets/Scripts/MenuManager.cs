using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("UI Elements")] 
    [SerializeField]
    private Slider volumeSlider;
    [FormerlySerializedAs("Sensativity")] [SerializeField]
    private Slider sensativity;
    [SerializeField] private GameObject pauseMenu;
    [Header("Results")] [SerializeField] private ResultsUI resultsUI;
    [FormerlySerializedAs("Pausable")] [Header("Settings")] [SerializeField] private bool pausable = true;
    [SerializeField] AudioMixerGroup mixer;
    private InputAsset _input;
    private bool _isPaused = false;
    private const string VolumePrefKey = "MusicVolume";
    private const string SensitivityPrefKey = "MouseSensitivity"; 
    
    private void Awake()
    {
        _input = new InputAsset();
    }
    private void Start()
    {
        Time.timeScale = 1f;

        float savedVolume = PlayerPrefs.GetFloat(VolumePrefKey, 0f);
        float savedSensitivity = PlayerPrefs.GetFloat(SensitivityPrefKey, 100f);

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(UpdateVolume);
        }
        
        if (sensativity != null)
        {
            sensativity.value = savedSensitivity;
            sensativity.onValueChanged.AddListener(UpdateSensitivity);
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
        if (pausable)
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
        // Debug.Log($"Volume updated to {volume}");
    }
    public void UpdateSensitivity(float value)
    {
        GameEvents.ChangeSensitivity(value);
        PlayerPrefs.SetFloat(SensitivityPrefKey, value);
        PlayerPrefs.Save();
        // Debug.Log($"Sensitivity updated to: {value}");
    }

    public void ExitApplication()
    {
        Application.Quit();
    }

    public void TogglePause()
    {
        _isPaused = !_isPaused;

        Time.timeScale = _isPaused ? 0 : 1;
        Cursor.lockState = _isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = _isPaused;

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(_isPaused);
        }

        // Debug.Log($"TogglePause: isPaused = {_isPaused}, Time.timeScale = {Time.timeScale}");
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && pausable && !_isPaused)
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
