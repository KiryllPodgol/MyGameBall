using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText; // Текстовый элемент для отображения таймера

    [Header("Game Settings")]
    [SerializeField] private float timeLimit = 20f; // Ограничение по времени в секундах
    [SerializeField] private GameObject playerPrefab; // Префаб персонажа
    [SerializeField] private Transform spawnPoint; // Точка спавна персонажа

    private float _timeRemaining;
    private bool _isGameActive;

    private void Start()
    {
        // Спавн персонажа
        if (playerPrefab != null && spawnPoint != null)
        {
            Debug.Log("Спавним персонажа...");
            Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            Debug.LogError("Префаб персонажа или точка спавна не назначены!");
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
            EndGame(false); // Проигрыш, если время закончилось
        }

        UpdateTimerUI();
    }

    public void FinishReached()
    {
        if (_isGameActive)
        {
            EndGame(true); // Победа, если достиг финиша
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
        // Обновляем текст таймера
        int seconds = Mathf.FloorToInt(_timeRemaining % 60);
        timerText.text = $"Time Left: {seconds}s";
    }

    public void RestartLevel()
    {
        // Перезапуск текущего уровня
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}