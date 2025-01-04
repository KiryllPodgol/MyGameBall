using System.Collections;
using UnityEngine;

public class TopViewComponent : MonoBehaviour
{
    [SerializeField] private CameraSettings cameraSettings; // Поле для настроек камеры
    private CameraFollow _cameraFollow;
    private float _originalFollowHeight;
    private float _originalFollowDistance; 
    private float _originalMouseSensitivity; // Для хранения оригинальной чувствительности мыши
    private float _originalFollowSpeed; // Для хранения оригинальной скорости следования
    private LayerMask _originalObstacleLayers; // Для хранения оригинальных слоев препятствий
    private bool _isTopView = false;

    private void Awake()
    {
        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            _cameraFollow = mainCamera.GetComponent<CameraFollow>();

            if (_cameraFollow != null)
            {
                // Сохраняем оригинальные параметры из CameraFollow
                _originalFollowHeight = _cameraFollow.FollowHeight;
                _originalFollowDistance = _cameraFollow.FollowDistance;
                _originalMouseSensitivity = _cameraFollow.MouseSensitivity; // Сохраняем оригинальную чувствительность мыши
                _originalFollowSpeed = _cameraFollow.FollowSpeed; // Сохраняем оригинальную скорость следования
                _originalObstacleLayers = _cameraFollow.ObstacleLayers; // Сохраняем оригинальные слои препятствий
            }
            else
            {
                Debug.LogError("CameraFollow component not found on the main camera.");
            }
        }
        else
        {
            Debug.LogError("Main camera not found.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Ball ball))
        {
            StartTopView();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Ball ball))
        {
            EndTopView();
        }
    }

    private void StartTopView()
    {
        if (_cameraFollow != null && !_isTopView)
        {
            StartCoroutine(SmoothTransition(cameraSettings.followHeight, cameraSettings.followDistance, cameraSettings.followSpeed, 1f)); // 1 секунда для перехода
            _cameraFollow.MouseSensitivity = 0f; 
            _cameraFollow.ObstacleLayers = cameraSettings.obstacleLayers;

            Debug.Log("Mouse sensitivity set to 0 for Top View.");
        
            _isTopView = true;
        }
    }

    private void EndTopView()
    {
        if (_cameraFollow != null && _isTopView)
        {
            StartCoroutine(SmoothTransition(_originalFollowHeight, _originalFollowDistance, _originalFollowSpeed, 1f)); // 1 секунда для перехода
            _cameraFollow.MouseSensitivity = _originalMouseSensitivity; 
            _cameraFollow.ObstacleLayers = _originalObstacleLayers;

            Debug.Log($"Mouse sensitivity restored to {_originalMouseSensitivity}.");
        
            _isTopView = false;
        }
    }

    private IEnumerator SmoothTransition(float targetHeight, float targetDistance, float targetSpeed, float duration)
    {
        float elapsedTime = 0f;

        float startHeight = _cameraFollow.FollowHeight;
        float startDistance = _cameraFollow.FollowDistance;
        float startSpeed = _cameraFollow.FollowSpeed;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            // Интерполяция значений
            _cameraFollow.FollowHeight = Mathf.Lerp(startHeight, targetHeight, t);
            _cameraFollow.FollowDistance = Mathf.Lerp(startDistance, targetDistance, t);
            _cameraFollow.FollowSpeed = Mathf.Lerp(startSpeed, targetSpeed, t);

            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        // Убедитесь, что конечные значения установлены
        _cameraFollow.FollowHeight = targetHeight;
        _cameraFollow.FollowDistance = targetDistance;
        _cameraFollow.FollowSpeed = targetSpeed;
    }

}
