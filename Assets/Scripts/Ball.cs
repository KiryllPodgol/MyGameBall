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
        Debug.Log("Jump key pressed");
        if (_isGrounded)
        {
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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
    private void CheckGround()
    {
        RaycastHit hit;
        // Проверяем наличие земли под объектом с использованием Raycast
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance, groundLayer);

        // Для отладки можно добавить линию визуализации
        Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, _isGrounded ? Color.green : Color.red);
    }
}
