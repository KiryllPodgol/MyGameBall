using System.Collections;
using UnityEngine;
using TMPro;
using Unity.Cinemachine;
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
        StartCoroutine(StartTimerWithDelay()); // вот тут ошибка!!!
        UpdateTimerUI();
    }

    private void Update()
    {
        if (!_isGameActive) return;

        // Проверка на достижение портала (если игрок уже в 1м от портала)
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

    private bool HasPlayerFallen()
    {
        const float fallThreshold = -10f; 

        if (_player != null && _player.transform.position.y < fallThreshold)
        {
            Debug.Log("Player has fallen!");
            return true;
        }
        return false;
    }
    private void ToggleTimer(bool isActive)
    {
        if (isActive)
        {
            if (!_isTimerActive) // Проверяем, если таймер еще не активирован
            {
                _isTimerActive = true;
                Debug.Log("Timer started!");
            }
        }
        else
        {
            if (_isTimerActive) // Проверяем, если таймер уже активирован
            {
                _isTimerActive = false;
                Debug.Log("Timer stopped because player is near the portal!");
            }
        }
    }
    private IEnumerator StartTimerWithDelay()
    {
        yield return new WaitForSeconds(timerStartDelay);  // Ожидаем время задержки
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

        int seconds = Mathf.FloorToInt(_timeRemaining % 60);
        timerText.text = !_isTimerActive 
            ? $"Timer starts in {Mathf.CeilToInt(timerStartDelay)}s" 
            : $"Time Left: {seconds}s";

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
        Debug.Log("Distance to portal: " + distanceToPortal);
        return distanceToPortal <= stopTimerDistance; 
    }
    private bool HasReachedPortal()
    {
        if (_player == null || portalZone == null) return false;

        float distanceToPortal = Vector3.Distance(_player.transform.position, portalZone.position);
        return distanceToPortal <= 1f;
    }
}
