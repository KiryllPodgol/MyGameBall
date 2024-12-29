using UnityEngine;
using TMPro;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;
    [Header("Game Settings")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float timeLimit = 20f;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private CameraFollow cameraFollow;
    [Header("Portal Settings")]
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

            if (virtualCamera != null)
            {
                virtualCamera.Follow = _player.transform;
                virtualCamera.LookAt = _player.transform;
            }

            if (cameraFollow != null)
            {
                cameraFollow.SetTarget(_player.transform);
            }
        }
        _timeRemaining = timeLimit;
        _isGameActive = true;
        _isTimerActive = false;
        Invoke(nameof(StartTimer), timerStartDelay);
        UpdateTimerUI();
    }

    private void Update()
    {
        if (!_isGameActive || !_isTimerActive) return;
        if (HasReachedPortal())
        {
            EndGame(true);
            return;
        }

        _timeRemaining -= Time.deltaTime;

        if (_timeRemaining <= 0)
        {
            _timeRemaining = 0;
            EndGame(false);
        }

        UpdateTimerUI();
    }

    private void StartTimer()
    {
        _isTimerActive = true;
        Debug.Log("Timer started!");
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

    private bool HasReachedPortal()
    {
        if (_player == null || portalZone == null) return false;

        float distanceToPortal = Vector3.Distance(_player.transform.position, portalZone.position);
        return distanceToPortal <= 1f;
    }
}
