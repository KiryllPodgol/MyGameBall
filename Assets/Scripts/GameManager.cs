using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Game Settings")]
    [SerializeField] private float timeLimit = 20f;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private CameraFollow cameraFollow; // Ссылка на скрипт камеры

    private float _timeRemaining;
    private bool _isGameActive;

    private void Start()
    {
        // Спавн персонажа
        if (playerPrefab != null && spawnPoint != null)
        {
            GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);

            // Передача ссылки камеры на персонажа
            if (cameraFollow != null)
            {
                cameraFollow.SetTarget(player.transform);
            }
        }

        // Инициализация таймера
        _timeRemaining = timeLimit;
        _isGameActive = true;
    }

    private void Update()
    {
        if (!_isGameActive) return;

        _timeRemaining -= Time.deltaTime;

        if (_timeRemaining <= 0)
        {
            _timeRemaining = 0;
            EndGame(false);
        }

        UpdateTimerUI();
    }

    public void FinishReached()
    {
        if (_isGameActive)
        {
            EndGame(true);
        }
    }

    private void EndGame(bool isWin)
    {
        _isGameActive = false;

        if (isWin)
        {
            timerText.text = "You Win!";
        }
        else
        {
            timerText.text = "Time's Up!";
        }
    }

    private void UpdateTimerUI()
    {
        int seconds = Mathf.FloorToInt(_timeRemaining % 60);
        timerText.text = $"Time Left: {seconds}s";

        if (_timeRemaining <= 10)
        {
            timerText.color = Color.red;
        }
        else
        {
            timerText.color = Color.white;
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
