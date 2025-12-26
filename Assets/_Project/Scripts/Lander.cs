using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lander : MonoBehaviour
{
    public event EventHandler OnUpForce;
    public event EventHandler OnLeftForce;
    public event EventHandler OnRightForce;
    public event EventHandler OnBeforeForce;

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private float _softLandingVelocityMagnitude = 3f;
    [SerializeField] private float _maxFuelAmount = 10f;

    private Rigidbody2D _rb;
    private GameInput _gameInput;
    private InputAction _moveAction;
    private float _currentFuelAmount;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        _gameInput = new();
        _moveAction = _gameInput.Gameplay.Move;
        _currentFuelAmount = _maxFuelAmount;
    }

    private void OnEnable()
    {
        _gameInput.Enable();
    }

    private void FixedUpdate()
    {
        OnBeforeForce?.Invoke(this, EventArgs.Empty);

        if (_currentFuelAmount <= 0f)
            return;

        if (AnyThrustInputPressed())
            ConsumeFuel();

        if (Keyboard.current.wKey.isPressed)
        {
            _rb.AddForce(_moveSpeed * Time.deltaTime * transform.up);
            OnUpForce?.Invoke(this, EventArgs.Empty);
        }
        if (Keyboard.current.aKey.isPressed)
        {
            _rb.AddTorque(_rotateSpeed * Time.deltaTime);
            OnLeftForce?.Invoke(this, EventArgs.Empty);
        }
        if (Keyboard.current.dKey.isPressed)
        {
            _rb.AddTorque(-_rotateSpeed * Time.deltaTime);
            OnRightForce?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnDisable()
    {
        _gameInput.Disable();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.TryGetComponent(out LandingPad landingPad))
        {
            print("Crashed on Terrain");
            return;
        }

        float relativeVelocityMagnitude = collision.relativeVelocity.magnitude;

        if (relativeVelocityMagnitude > _softLandingVelocityMagnitude)
        {
            // Crashed
            print("Landed too fast.");
            return;
        }

        float dotVector = Vector2.Dot(Vector2.up, transform.up);
        float minDotVector = 0.9f;
        if (dotVector < minDotVector)
        {
            // Landed too steeply
            print("Landed at a bad angle.");
            return;
        }

        print("Successful landing");

        float maxScoreAmountLandingAngle = 100f;
        float scoreDotVectorMultiplier = 10f;
        float landingAngleScore = maxScoreAmountLandingAngle - Mathf.Abs(dotVector - 1f) * scoreDotVectorMultiplier;
        float maxScoreAmountLandingSpeed = 100f;
        float landingSpeedScore = (_softLandingVelocityMagnitude - relativeVelocityMagnitude) * maxScoreAmountLandingSpeed;

        print($"Landing Angle Score: {landingAngleScore}, Landing Speed Score: {landingSpeedScore}");

        int score = Mathf.RoundToInt((landingAngleScore + landingSpeedScore) * landingPad.ScoreMultiplier);

        print($"Score: {score}");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out FuelCollectable fuelCollectable))
        {
            _currentFuelAmount = _maxFuelAmount;
            fuelCollectable.DestroySelf();
        }
    }

    private void SetupInput()
    {
    }

    private void InputUnsubscribe()
    {
    }

    private void ConsumeFuel()
    {
        float fuelConsumptionAmount = 1f;
        _currentFuelAmount -= fuelConsumptionAmount * Time.deltaTime;
    }

    private bool AnyThrustInputPressed()
    {
        return Keyboard.current.wKey.isPressed || Keyboard.current.aKey.isPressed || Keyboard.current.dKey.isPressed;
    }
}