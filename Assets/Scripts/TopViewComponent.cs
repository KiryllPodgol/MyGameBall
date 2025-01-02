using UnityEngine;

public class TopViewComponent : MonoBehaviour
{
    private CameraFollow _cameraFollow;
    private float _originalFollowHeight;
    private float _originalFollowDistance; 
    private float _topViewHeight = 40f; 
    private float _topViewDistance = 0f; 
    private bool _isTopView = false;

    private void Awake()
    {
        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            _cameraFollow = mainCamera.GetComponent<CameraFollow>();

            if (_cameraFollow != null)
            {
             
                _originalFollowHeight = _cameraFollow.FollowHeight;
                _originalFollowDistance = _cameraFollow.FollowDistance;
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
           
            _cameraFollow.FollowHeight = _topViewHeight;
            _cameraFollow.FollowDistance = _topViewDistance;
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
            _isTopView = false;
        }
    }
}
