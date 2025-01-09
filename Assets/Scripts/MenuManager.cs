using UnityEngine; 
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private GameObject pauseMenu;
    private InputAsset _input;
    private bool isPaused = false;

    private void Awake()
    {
        _input = new InputAsset();
        if (musicSource != null && volumeSlider != null)
        {
            volumeSlider.value = musicSource.volume;
            volumeSlider.onValueChanged.AddListener(MusicVolume);
        }

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
    }

    private void OnEnable()
    {
        _input.UI.Pause.performed += OnPausePressed;
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.UI.Pause.performed -= OnPausePressed;
        _input.UI.Disable();
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(MusicVolume);
        }
    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        TogglePause();
    }

    public void MusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
    }

    public void ExitApplication()
    {
        Application.Quit();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;

        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        Debug.Log($"TogglePause: isPaused = {isPaused}, Time.timeScale = {Time.timeScale}");

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(isPaused);
            Debug.Log($"Pause menu is now {(isPaused ? "active" : "inactive")}.");
        }
    }

    public void LoadScene(int sceneIndex)
    {
        if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            // Используем метод для переключения через сцену-подложку
            SceneTransition.SwitchSceneWithLoading(sceneIndex);
        }
        else
        {
            Debug.LogError("Scene index out of range!");
        }
    }
}