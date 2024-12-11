using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;
    [Header("Game Settings")]
    [SerializeField] private float timeLimit = 20f;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private CameraFollow cameraFollow;
    [Header("Portal Settings")]
    [SerializeField] private Transform portalZone; 

    private float _timeRemaining;
    private bool _isGameActive;
    private GameObject _player;

    private void Start()
    {
        if (playerPrefab != null && spawnPoint != null)
        {
            _player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);

            if (cameraFollow != null)
            {
                cameraFollow.SetTarget(_player.transform);
            }
        }

      
        _timeRemaining = timeLimit;
        _isGameActive = true;
    }

    private void Update()
    {
        if (!_isGameActive) return;

        // Проверяем, достиг ли игрок зоны портала
        if (HasReachedPortal())
        {
            EndGame(true);
            return; // Выходим, чтобы остановить дальнейшие обновления
        }

        _timeRemaining -= Time.deltaTime;

        if (_timeRemaining <= 0)
        {
            _timeRemaining = 0;
            EndGame(false);
        }

        UpdateTimerUI();
    }

    private void EndGame(bool isWin)
    {
        _isGameActive = false;

        if (isWin)
        {
            timerText.text = "You Win!";
            timerText.color = Color.green; 
        }
        else
        {
            timerText.text = "Time's Up!";
            RestartLevel(); // Перезапускаем уровень
        }
    }

    private void UpdateTimerUI()
    {
        if (!_isGameActive) return; 

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

    private bool HasReachedPortal()
    {
        if (_player == null || portalZone == null) return false;

        // Рассчитываем дистанцию между игроком и зоной портала
        float distanceToPortal = Vector3.Distance(_player.transform.position, portalZone.position);

        // Проверяем, находится ли игрок в пределах радиуса зоны портала
        return distanceToPortal <= 1f; // Радиус зоны — 1 единица
    }
}