using System.Collections;
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
    [SerializeField] private float rotationSpeed = 10f; 
    [SerializeField] private LayerMask groundLayer; 
    [SerializeField] private float groundCheckDistance = 1f;
    [SerializeField] private float decelerationRate = 2f; // Скорость замедления

    private Vector3 _moveDirection;
    private CameraFollow _cameraFollow;
    private bool _isGrounded;
    private bool _isRunning;
    private Coroutine _decelerationCoroutine;

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
        _input.Gameplay.Jump.performed += JumpOnPerformed;
        _input.Gameplay.Run.performed += RunOnPerformed;
        _input.Gameplay.Run.canceled += RunOnCanceled;
    }

    private void OnDisable()
    {
        _input.Gameplay.Move.performed -= OnMovePerformed;
        _input.Gameplay.Move.canceled -= OnMoveCanceled;
        _input.Gameplay.Jump.performed -= JumpOnPerformed;
        _input.Gameplay.Run.performed -= RunOnPerformed;
        _input.Gameplay.Run.canceled -= RunOnCanceled;
        _input.Gameplay.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        Vector2 inputVector = context.ReadValue<Vector2>();
        _moveDirection = new Vector3(inputVector.x, 0, inputVector.y);

        // Остановка корутины замедления, если игрок снова двигается
        if (_decelerationCoroutine != null)
        {
            StopCoroutine(_decelerationCoroutine);
            _decelerationCoroutine = null;
        }
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        _moveDirection = Vector3.zero;

        // Запуск корутины замедления, если игрок отпустил управление
        if (_decelerationCoroutine == null)
        {
            _decelerationCoroutine = StartCoroutine(Decelerate());
        }
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

        if (_moveDirection.magnitude > 0.1f)
        {
            // Движение
            Vector3 force = globalMoveDirection * currentSpeed;
            _rb.AddForce(force, ForceMode.Acceleration);

            // Вращение
            Vector3 torque = new Vector3(globalMoveDirection.z, 0, -globalMoveDirection.x) * rotationSpeed;
            _rb.AddTorque(torque, ForceMode.Acceleration);
        }

        // Гравитация
        _rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);
    }

    private IEnumerator Decelerate()
    {
        while (_rb.linearVelocity.magnitude > 0.1f)
        {
            _rb.linearVelocity = Vector3.Lerp(_rb.linearVelocity, Vector3.zero, Time.fixedDeltaTime * decelerationRate);
            yield return new WaitForFixedUpdate();
        }

        _rb.linearVelocity = Vector3.zero; // Полная остановка
        _decelerationCoroutine = null;
    }

    private void CheckGround()
    {
        RaycastHit hit;
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance, groundLayer);

        Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, _isGrounded ? Color.green : Color.red);
    }
}
