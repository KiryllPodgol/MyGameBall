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
            // Устанавливаем параметры для вида сверху из cameraSettings
            _cameraFollow.FollowHeight = cameraSettings.followHeight;
            _cameraFollow.FollowDistance = cameraSettings.followDistance;

            // Отключаем чувствительность мыши и устанавливаем скорость следования
            _cameraFollow.MouseSensitivity = 0f; 
            _cameraFollow.FollowSpeed = cameraSettings.followSpeed; // Установите скорость следования из настроек камеры
            
            // Устанавливаем слои препятствий из cameraSettings (например, "Nothing")
            _cameraFollow.ObstacleLayers = cameraSettings.obstacleLayers;

            Debug.Log("Mouse sensitivity set to 0 for Top View.");
            Debug.Log($"Follow speed set to {cameraSettings.followSpeed} for Top View.");
            
            _isTopView = true;
        }
    }

    private void EndTopView()
    {
        if (_cameraFollow != null && _isTopView)
        {
            // Восстанавливаем оригинальные параметры
            _cameraFollow.FollowHeight = _originalFollowHeight;
            _cameraFollow.FollowDistance = _originalFollowDistance;

            // Восстанавливаем оригинальную чувствительность мыши и скорость следования
            _cameraFollow.MouseSensitivity = _originalMouseSensitivity; 
            _cameraFollow.FollowSpeed = _originalFollowSpeed; 

            // Восстанавливаем оригинальные слои препятствий
            _cameraFollow.ObstacleLayers = _originalObstacleLayers;

            Debug.Log($"Mouse sensitivity restored to {_originalMouseSensitivity}.");
            Debug.Log($"Follow speed restored to {_originalFollowSpeed}.");
            
            _isTopView = false;
        }
    }
}
