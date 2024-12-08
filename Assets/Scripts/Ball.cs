using UnityEngine;

public class Ball : MonoBehaviour
{
    private InputAsset _input;
    private Rigidbody _rb;

    [SerializeField] private float moveSpeed = 5f; // Скорость движения
    [SerializeField] private float gravity = -9.81f; // Сила гравитации
    [SerializeField] private float rotationSpeed = 10f; // Скорость вращения шарика
    private Vector3 _moveDirection;

    private void Awake()
    {
        _input = new InputAsset();
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _input.Gameplay.Enable();
        _input.Gameplay.Move.performed += OnMovePerformed;
        _input.Gameplay.Move.canceled += OnMoveCanceled;
    }

    private void OnDisable()
    {
        _input.Gameplay.Move.performed -= OnMovePerformed;
        _input.Gameplay.Move.canceled -= OnMoveCanceled;
        _input.Gameplay.Disable();
    }

    private void OnMovePerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Vector2 inputVector = context.ReadValue<Vector2>();
        _moveDirection = new Vector3(inputVector.x, 0, inputVector.y);
    }

    private void OnMoveCanceled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        _moveDirection = Vector3.zero;
    }

    private void FixedUpdate()
    {
        // Движение
        Vector3 force = _moveDirection * moveSpeed;
        _rb.AddForce(force, ForceMode.Acceleration);

        // Гравитация
        _rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);

        // Вращение — только при наличии движения
        if (_moveDirection.magnitude > 0.1f) // Проверка, что направление не нулевое
        {
            Vector3 torque = new Vector3(_moveDirection.z, 0, -_moveDirection.x) * rotationSpeed;
            _rb.AddTorque(torque, ForceMode.Acceleration);
        }
    }
}