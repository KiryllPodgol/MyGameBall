using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI levelTitleText;
    [Header("Game Settings")]
    [SerializeField] private float timeLimit = 20f;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private CameraFollow cameraFollow;
    [Header("Portal Settings")]
    [SerializeField] private float stopTimerDistance = 3f; // Расстояние для остановки таймера
    [SerializeField] private Transform portalZone;
    [Header("Timer Settings")]
    [SerializeField] private float timerStartDelay = 5f; 
    private float _timeRemaining;
    private bool _isGameActive;
    private bool _isTimerActive;
    private GameObject _player;

    private void Start()
    {
        SetLevelTitle();
        
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
        _isTimerActive = false;
        StartCoroutine(StartTimerWithDelay());
        UpdateTimerUI();
    }


    private void Update()
    {
        if (!_isGameActive) return;

        // Проверка на достижение портала
        if (HasReachedPortal())
        {
            EndGame(true);
            return;
        }

        // Проверка на близость к порталу
        if (HasPlayerNearPortal())
        {
            ToggleTimer(false); // Останавливаем таймер, если игрок близко к порталу
        }
    
        // Проверка на падение в пропасть
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
        if (isActive)
        {
            if (!_isTimerActive)
            {
                _isTimerActive = true;
            }
        }
        else
        {
            if (_isTimerActive)
            {
                _isTimerActive = false;
            }
        }
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
    
        timerText.text = !_isTimerActive 
            ? $"Starts in {Mathf.CeilToInt(timerStartDelay)}s" 
            : $"Time Left: {minutes}m {seconds}s";

        timerText.color = _isTimerActive && _timeRemaining <= 10 ? Color.red : Color.white;
    }
    public void RestartLevel()
    { 
        
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
}
