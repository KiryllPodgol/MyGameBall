using UnityEngine;
using UnityEngine.InputSystem;

public class Ball : MonoBehaviour
{
    private InputAsset _input;
    private Rigidbody _rb;
    [SerializeField] private float moveSpeed = 5f; // Скорость движения
    [SerializeField] private float gravity = -9.81f; // Сила гравитации
    [SerializeField] private float jumpForce = 5f; // Силла прыжка
    [SerializeField] private float rotationSpeed = 10f; // Скорость вращения шарика
    [SerializeField] private LayerMask groundLayer; // Слой для земли
    [SerializeField] private float groundCheckDistance = 1f; // Дистанция проверки земли
    private Vector3 _moveDirection;
    private CameraFollow _cameraFollow;
    private bool _isGrounded;


    private void Awake()
    {
        _input = new InputAsset();
        _rb = GetComponent<Rigidbody>();
        if (Camera.main != null)
        {
            _cameraFollow = Camera.main.GetComponent<CameraFollow>();
        }
        else
        {
            Debug.LogError("Main Camera not found! Please assign a camera with the CameraFollow script.");
            enabled = false;
        }

    }

    private void OnEnable()
    {
        _input.Gameplay.Enable();
        _input.Gameplay.Move.performed += OnMovePerformed;
        _input.Gameplay.Move.canceled += OnMoveCanceled;
        _input.Gameplay.Jump.performed += JumpOnperformed;
        _input.Gameplay.Jump.canceled += JumpOnperformed;
    }

    private void OnDisable()
    {
        _input.Gameplay.Move.performed -= OnMovePerformed;
        _input.Gameplay.Move.canceled -= OnMoveCanceled;
        _input.Gameplay.Jump.performed -= JumpOnperformed;
        _input.Gameplay.Jump.canceled -= JumpOnperformed;
        _input.Gameplay.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        Vector2 inputVector = context.ReadValue<Vector2>();
        _moveDirection = new Vector3(inputVector.x, 0, inputVector.y);
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        _moveDirection = Vector3.zero;
    }

    private void JumpOnperformed(InputAction.CallbackContext obj)
    {
        if (_isGrounded)
        {
            var jumpDirection = Vector3.up; // По умолчанию прыжок вверх
            if (_contactNormal != Vector3.zero)
            {
                jumpDirection = _contactNormal.normalized; 
            }

            _rb.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        CheckGround();
        Vector3 forward = _cameraFollow.GetCameraRotation() * Vector3.forward;
        Vector3 right = _cameraFollow.GetCameraRotation() * Vector3.right;

        Vector3 globalMoveDirection = (forward * _moveDirection.z + right * _moveDirection.x).normalized;

        // Движение
        Vector3 force = globalMoveDirection * moveSpeed;

        _rb.AddForce(force, ForceMode.Acceleration);

        // Гравитация
        _rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);

        // Вращение
        if (_moveDirection.magnitude > 0.1f)
        {
            Vector3 torque = new Vector3(globalMoveDirection.z, 0, -globalMoveDirection.x) * rotationSpeed;
            _rb.AddTorque(torque, ForceMode.Acceleration);
        }
    }

    private Vector3 _contactNormal;

    private void OnCollisionEnter(Collision other)
    {
        _contactNormal = other.contacts[0].normal;
    }

    private void CheckGround()
    {
        RaycastHit hit;
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance, groundLayer);

        if (_isGrounded)
        {
            _contactNormal = hit.normal;
        }
        else
        {
            _contactNormal = Vector3.zero;
        }

        // Отрисовка для отладки
        Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, _isGrounded ? Color.green : Color.red);
        if (_isGrounded)
        {
            Debug.DrawRay(hit.point, hit.normal, Color.blue);
        }
    }
    private void OnDrawGizmos()
    {
        if (_isGrounded && _contactNormal != Vector3.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + _contactNormal);
            Gizmos.DrawSphere(transform.position + _contactNormal, 0.1f);
        }
    }
}