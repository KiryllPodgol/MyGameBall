using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Vector2 pitchLimits = new Vector2(-45f, 45f);
    [SerializeField] private float followDistance = 10f;
    [SerializeField] private float followHeight = 5f;
    [SerializeField] private float followSpeed = 10f;

    private Transform _target;
    private float _yaw = 0f;
    private float _pitch = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        transform.LookAt(_target.position + Vector3.up * 1f);
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