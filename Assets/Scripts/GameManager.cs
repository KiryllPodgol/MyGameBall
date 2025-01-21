using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI levelTitleText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [Header("Game Settings")]
    [SerializeField] private float timeLimit = 20f;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private CameraFollow cameraFollow;
    [Header("Portal Settings")]
    [SerializeField] private float stopTimerDistance = 3f;
    [SerializeField] private Transform portalZone;
    [Header("Timer Settings")]
    [SerializeField] private float timerStartDelay = 5f;
    [Header("Death Settings")]
    [SerializeField] private GameObject deadBodyPrefab; // Префаб объекта, оставляемого после смерти
    [SerializeField] private float invulnerabilityDuration = 5f; // Время неуязвимости после смерти

    private float _timeRemaining;
    private bool _isGameActive;
    private bool _isTimerActive;
    private GameObject _player;
    private int _score;
    private int _initialScore;
    private bool _isInvulnerable = false; // Флаг неуязвимости

    private void Start()
    {
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
        Collectible.OnCollected += OnCollectibleCollected;
    }

    private void OnDestroy()
    {
        Collectible.OnCollected -= OnCollectibleCollected;
    }

    private void OnCollectibleCollected()
    {
        AddScore(100);
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
            timerText.text = string.Format("Time Left: {0:00}:{1:00}", minutes, seconds);
        }

        timerText.color = _isTimerActive && _timeRemaining <= 10 ? Color.red : Color.white;
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Очков: {_score}";
        }
    }

    public void RestartLevel()
    {
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

    private void OnCollisionEnter(Collision collision)
    {
        if (_isInvulnerable) return;
        if (collision.gameObject.TryGetComponent<Death>(out Death deathComponent))
        {
            Death(); 
        }
    }

    public void Death()
    {
        if (_player != null)
        {
            Instantiate(deadBodyPrefab, _player.transform.position, _player.transform.rotation);
            Vector3 deathPosition = _player.transform.position;
            Destroy(_player);
            StartCoroutine(RespawnPlayer(deathPosition));
        }
    }

    private void SpawnPlayer(Vector3 position, Quaternion rotation)
    {
        _player = Instantiate(playerPrefab, position, rotation);

        if (cameraFollow != null)
        {
            cameraFollow.SetTarget(_player.transform);
        }
    }

    private IEnumerator RespawnPlayer(Vector3 position)
    {
        _isInvulnerable = true;
        yield return new WaitForSeconds(2f); 
        SpawnPlayer(position, Quaternion.identity);
        yield return new WaitForSeconds(invulnerabilityDuration);
        _isInvulnerable = false;
    }
} 