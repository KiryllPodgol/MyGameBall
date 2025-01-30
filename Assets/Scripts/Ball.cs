using UnityEngine;
using UnityEngine.InputSystem;

public class Ball : MonoBehaviour
{
    private InputAsset _input;
    private Rigidbody _rb;
    [SerializeField] private float moveSpeed = 5f; 
    [SerializeField] private float runMultiplier = 2f;
    [SerializeField] private float gravity = -9.81f; // Сила гравитации
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private LayerMask groundLayer; 
    [SerializeField] private float groundCheckDistance = 1f;
    [SerializeField] private float decelerationRate = 2f; // Скорость замедления
    private float _originalFixedDeltaTime;
    private bool _isSlowMotion = false;
    private Vector3 _moveDirection;
    private CameraFollow _cameraFollow;
    private bool _isGrounded;
    private bool _isRunning;

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
        _originalFixedDeltaTime = Time.fixedDeltaTime;
    }

    private void OnEnable()
    {
        _input.Gameplay.Enable();
        _input.Gameplay.Move.performed += OnMovePerformed;
        _input.Gameplay.Move.canceled += OnMoveCanceled;
        _input.Gameplay.Jump.performed += JumpOnPerformed;
        _input.Gameplay.Run.performed += RunOnPerformed;
        _input.Gameplay.Run.canceled += RunOnCanceled;
        _input.Gameplay.SlowMotion.performed += SlowMotionOnperformed;
        _input.Gameplay.SlowMotion.canceled += SlowMotionOncanceled;
    }
    private void SlowMotionOnperformed(InputAction.CallbackContext obj)
    {
        _isSlowMotion = true;
        Time.timeScale = 0.2f;
        Time.fixedDeltaTime = _originalFixedDeltaTime * Time.timeScale;
    }

    private void SlowMotionOncanceled(InputAction.CallbackContext obj)
    {
        _isSlowMotion = false;
        Time.timeScale = 1f; 
        Time.fixedDeltaTime = _originalFixedDeltaTime; 
    }
    

    private void OnDisable()
    {
        _input.Gameplay.Move.performed -= OnMovePerformed;
        _input.Gameplay.Move.canceled -= OnMoveCanceled;
        _input.Gameplay.Jump.performed -= JumpOnPerformed;
        _input.Gameplay.Run.performed -= RunOnPerformed;
        _input.Gameplay.Run.canceled -= RunOnCanceled;
        _input.Gameplay.SlowMotion.performed -= SlowMotionOnperformed;
        _input.Gameplay.SlowMotion.canceled -= SlowMotionOncanceled;
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

    private void RunOnPerformed(InputAction.CallbackContext obj)
    {
        _isRunning = true;
    }

    private void RunOnCanceled(InputAction.CallbackContext obj)
    {
        _isRunning = false;
    }

    private void JumpOnPerformed(InputAction.CallbackContext obj)
    {
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

        float currentSpeed = _isRunning ? moveSpeed * runMultiplier : moveSpeed;

        if (_moveDirection.sqrMagnitude > 0.1f)
        {
            // Движение и Вращение
            Vector3 force = globalMoveDirection * currentSpeed;
            _rb.AddForceAtPosition(force, _rb.transform.position + Vector3.up * 1.75f, ForceMode.Acceleration);
        }
        else if (_rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            // Плавное замедление
            Vector3 newVelocity = Vector3.Lerp(_rb.linearVelocity, Vector3.zero,
                Time.fixedDeltaTime * decelerationRate);
            _rb.linearVelocity = new Vector3(newVelocity.x, _rb.linearVelocity.y, newVelocity.z);

            // Замедление вращения
            _rb.angularVelocity = Vector3.Lerp(_rb.angularVelocity, Vector3.zero,
                Time.fixedDeltaTime * decelerationRate);
        }
        else
        {
            // Полная остановка
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }

        // Гравитация
        _rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);
    }
    private void CheckGround()
    {
        RaycastHit hit;
        float sphereRadius = 0.5f; // Радиус сферы
        _isGrounded = Physics.SphereCast(transform.position, sphereRadius, Vector3.down, out hit, groundCheckDistance, groundLayer);


        Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, _isGrounded ? Color.green : Color.red);
    }
}
