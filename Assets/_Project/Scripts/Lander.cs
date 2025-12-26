using UnityEngine;
using UnityEngine.InputSystem;

public class Lander : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private float _softLandingVelocityMagnitude = 3f;

    private Rigidbody2D _rb;
    private GameInput _gameInput;
    private InputAction _moveAction;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        _gameInput = new();
        _moveAction = _gameInput.Gameplay.Move;
    }

    private void OnEnable()
    {
        _gameInput.Enable();
    }

    private void FixedUpdate()
    {
        if (Keyboard.current.wKey.isPressed)
            _rb.AddForce(_moveSpeed * Time.deltaTime * transform.up);
        if (Keyboard.current.aKey.isPressed)
            _rb.AddTorque(_rotateSpeed * Time.deltaTime);
        if (Keyboard.current.dKey.isPressed)
            _rb.AddTorque(-_rotateSpeed * Time.deltaTime);
    }

    private void OnDisable()
    {
        _gameInput.Disable();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude > _softLandingVelocityMagnitude)
        {
            // Crashed
            return;
        }

        float dotVector = Vector2.Dot(Vector2.up, transform.up);
        float minDotVector = 0.9f;
        if (dotVector < minDotVector)
        {
            // Landed too steeply
            return;
        }
    }

    private void SetupInput()
    {
    }

    private void InputUnsubscribe()
    {
    }
}