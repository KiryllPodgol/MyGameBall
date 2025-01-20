using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Vector2 pitchLimits = new Vector2(-45f, 45f);
    [SerializeField] private float followDistance = 10f;
    [SerializeField] private float followHeight = 5f;
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private LayerMask obstacleLayers; 
    [SerializeField] private float cameraRadius = 0.5f;

    private Transform _target;
    private float _yaw = 0f;
    private float _pitch = 0f;

    private void Start()
    {
       
        UpdateCursorState();
    }
    private void UpdateCursorState()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (currentSceneIndex == 5)
        {
            Cursor.lockState = CursorLockMode.None; // Разблокировать курсор
            Cursor.visible = true; // Сделать курсор видимым
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; // Заблокировать курсор
            Cursor.visible = false; // Скрыть курсор
        }
    }
    private void FixedUpdate()
    {
        if (_target == null) return;
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        _yaw += mouseX;
        _pitch -= mouseY;
        _pitch = Mathf.Clamp(_pitch, pitchLimits.x, pitchLimits.y);
        Vector3 desiredPosition = _target.position - Quaternion.Euler(0f, _yaw, 0f) * Vector3.forward * followDistance;
        desiredPosition.y = _target.position.y + followHeight;
        
        if (!CheckObstacles(desiredPosition))
        {
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        }
        
        transform.LookAt(_target.position + Vector3.up * 1f);
    }

    private bool CheckObstacles(Vector3 desiredPosition)
    {
        Vector3 directionToCamera = desiredPosition - _target.position;
        
        if (Physics.SphereCast(_target.position, cameraRadius, directionToCamera.normalized, out RaycastHit hit, directionToCamera.magnitude, obstacleLayers))
        {
            return true; 
        }
        
        return false; 
    }
    public float MouseSensitivity
    {
        get { return mouseSensitivity; }
        set { mouseSensitivity = value; } 
    }

    public float FollowSpeed
    {
        get { return followSpeed; }
        set { followSpeed = value; } 
    }
    public float FollowDistance
    {
        get { return followDistance; }
        set { followDistance = value; }
    }

    public float FollowHeight
    {
        get { return followHeight; }
        set { followHeight = value; }
    }
    public LayerMask ObstacleLayers
    {
        get { return obstacleLayers; }
        set { obstacleLayers = value; }
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public Quaternion GetCameraRotation()
    {
        return Quaternion.Euler(0, _yaw, 0);
    }
}
