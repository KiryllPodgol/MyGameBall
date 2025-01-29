using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [Header("UI")] [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI levelTitleText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Game Settings")] [SerializeField]
    private float timeLimit = 20f;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private CameraFollow cameraFollow;

    [Header("Portal Settings")] [SerializeField]
    private float stopTimerDistance = 3f;
    [SerializeField] private Transform portalZone;
    
    [Header("Timer Settings")] [SerializeField]
    private float timerStartDelay = 5f;
    
    [Header("Death Settings")] [SerializeField]
    private GameObject deadBodyPrefab;
    [SerializeField] private Vector3 offset = new Vector3(32f, 0f, 0f);
    [SerializeField] private float invulnerabilityDuration = 5f; // Время неуязвимости после смерти

    private float _timeRemaining;
    private bool _isGameActive;
    private bool _isTimerActive;
    private GameObject _player;
    private int _score;
    private int _initialScore;
    private bool _isInvulnerable = false;

    private void Start()
    {
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;

        if (GameStats.Instance != null)
        {
            GameStats.Instance.StartLevel(currentLevelIndex); // Начинаем отслеживание времени уровня
        }

        SetLevelTitle();

        if (playerPrefab != null && spawnPoint != null)
        {
            SpawnPlayer(spawnPoint.position, spawnPoint.rotation);
        }

        _timeRemaining = timeLimit;
        _isGameActive = true;
        _isTimerActive = false;
        _initialScore = _score;
        UpdateScoreUI();
        StartCoroutine(StartTimerWithDelay());
        UpdateTimerUI();
        GameEvents.OnCollectibleCollected += OnCollectibleCollected;
    }

    private void OnDestroy()
    {
        GameEvents.OnCollectibleCollected -= OnCollectibleCollected;
    }

    private void OnCollectibleCollected()
    {
        AddScore(1);
    }

    private void Update()
    {
        if (!_isGameActive) return;

        if (HasReachedPortal())
        {
            EndGame(true);
            return;
        }

        if (HasPlayerNearPortal())
        {
            ToggleTimer(false);
        }

        if (HasPlayerFallen())
        {
            RestartLevel();
            return;
        }

        if (!_isTimerActive) return;

        _timeRemaining -= Time.deltaTime;
        if (_timeRemaining <= 0)
        {
            _timeRemaining = 0;
            EndGame(false);
        }

        UpdateTimerUI();
    }

    private void SetLevelTitle()
    {
        if (levelTitleText == null)
        {
            Debug.LogError("LevelTitleText reference is missing. Please assign it in the inspector.");
            return;
        }

        string sceneName = SceneManager.GetActiveScene().name;
        levelTitleText.text = $"Level: {sceneName}";
    }

    private bool HasPlayerFallen()
    {
        const float fallThreshold = -10f;
        const float raycastDistance = 1.5f;
        LayerMask groundMask = LayerMask.GetMask("Ground");

        if (_player != null)
        {
            if (_player.transform.position.y < fallThreshold)
            {
                if (!Physics.Raycast(_player.transform.position, Vector3.down, raycastDistance, groundMask))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void ToggleTimer(bool isActive)
    {
        _isTimerActive = isActive;
    }

    private IEnumerator StartTimerWithDelay()
    {
        yield return new WaitForSeconds(timerStartDelay);
        ToggleTimer(true);
    }

    private void EndGame(bool isWin)
    {
        _isGameActive = false;
        _isTimerActive = false;

        if (isWin)
        {
            _score += Mathf.FloorToInt(_timeRemaining) * 10;
            UpdateScoreUI();

            timerText.text = "You Win!";
            timerText.color = Color.green;
        }
        else
        {
            timerText.text = "Time's Up!";
            RestartLevel();
        }
    }

    private void UpdateTimerUI()
    {
        if (!_isGameActive) return;

        int minutes = Mathf.FloorToInt(_timeRemaining / 60);
        int seconds = Mathf.FloorToInt(_timeRemaining % 60);

        if (!_isTimerActive)
        {
            timerText.text = $"Starts in {Mathf.CeilToInt(timerStartDelay)}s";
        }
        else
        {
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        timerText.color = _isTimerActive && _timeRemaining <= 10 ? Color.red : Color.white;
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Монет: {_score}";
        }
    }

    public void RestartLevel()
    {
        if (GameStats.Instance != null)
        {
            int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
            GameStats.Instance.AddRestart(currentLevelIndex);
        }

        _score = _initialScore;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private bool HasPlayerNearPortal()
    {
        if (_player == null || portalZone == null) return false;

        float distanceToPortal = Vector3.Distance(_player.transform.position, portalZone.position);
        return distanceToPortal <= stopTimerDistance;
    }

    private bool HasReachedPortal()
    {
        if (_player == null || portalZone == null) return false;
        float distanceToPortal = Vector3.Distance(_player.transform.position, portalZone.position);
        return distanceToPortal <= 1f;
    }

    public void AddScore(int amount)
    {
        _score += amount;
        UpdateScoreUI();
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (_isInvulnerable) return;
        if (collision.gameObject.TryGetComponent<Death>(out Death deathComponent))
        {
            Death();
        }
    }
    public void Death()
    {
        if (GameStats.Instance != null)
        {
            int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
            GameStats.Instance.AddDeath(currentLevelIndex);
        }

        // Создаем тело игрока после смерти
        Vector3 deathPosition = _player.transform.position;
        if (deadBodyPrefab != null)
        {
            Instantiate(deadBodyPrefab, deathPosition, Quaternion.identity);
        }

        // Удаляем текущего игрока
        if (_player != null)
        {
            Destroy(_player);
        }

        // Запускаем респавн игрока с учетом смещения
        StartCoroutine(RespawnPlayer(deathPosition));
    }
    
    private IEnumerator RespawnPlayer(Vector3 deathPosition)
    {
        _isInvulnerable = true; // Включаем неуязвимость

        // Ждем 2 секунды перед респавном
        yield return new WaitForSeconds(2f);
        
        Vector3 respawnPosition = deathPosition + offset;

        // Респавним игрока
        SpawnPlayer(respawnPosition, Quaternion.identity);
        
        yield return new WaitForSeconds(invulnerabilityDuration);
        _isInvulnerable = false; // Выключаем неуязвимость
    }
    private void SpawnPlayer(Vector3 position, Quaternion rotation)
    {
        _player = Instantiate(playerPrefab, position, rotation);

        if (cameraFollow != null)
        {
            cameraFollow.SetTarget(_player.transform);
        }
    }
}