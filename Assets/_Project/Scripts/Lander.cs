using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lander : MonoBehaviour
{
    public static Lander Instance { get; private set; }

    public float CurrentFuelAmount { get; private set; }

    public event EventHandler OnUpForce;
    public event EventHandler OnLeftForce;
    public event EventHandler OnRightForce;
    public event EventHandler OnBeforeForce;
    public event EventHandler OnCoinPickup;
    public event EventHandler<OnLandedEventArgs> OnLanded;
    public class OnLandedEventArgs : EventArgs
    {
        public LandingType landingType;
        public float dotVector;
        public float landingSpeed;
        public float scoreMultiplier;
        public int score;
    }

    public enum LandingType
    {
        Success,
        WrongLandingArea,
        TooSteepAngle,
        TooFastLanding,
    }

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private float _softLandingVelocityMagnitude = 3f;
    [SerializeField] private float _maxFuelAmount = 10f;

    private Rigidbody2D _rb;
    private GameInput _gameInput;
    private InputAction _moveAction;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _rb = GetComponent<Rigidbody2D>();

        _gameInput = new();
        _moveAction = _gameInput.Gameplay.Move;
        CurrentFuelAmount = _maxFuelAmount;
    }

    private void OnEnable()
    {
        _gameInput.Enable();
    }

    private void FixedUpdate()
    {
        OnBeforeForce?.Invoke(this, EventArgs.Empty);

        if (CurrentFuelAmount <= 0f)
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
            OnLanded?.Invoke(this, new OnLandedEventArgs
            {
                landingType = LandingType.WrongLandingArea,
                dotVector = 0f,
                landingSpeed = 0f,
                scoreMultiplier = 0f,
                score = 0,
            });
            return;
        }

        float relativeVelocityMagnitude = collision.relativeVelocity.magnitude;

        if (relativeVelocityMagnitude > _softLandingVelocityMagnitude)
        {
            // Crashed
            print("Landed too hard.");
            OnLanded?.Invoke(this, new OnLandedEventArgs
            {
                landingType = LandingType.TooFastLanding,
                dotVector = 0f,
                landingSpeed = relativeVelocityMagnitude,
                scoreMultiplier = 0f,
                score = 0,
            });
            return;
        }

        float dotVector = Vector2.Dot(Vector2.up, transform.up);
        float minDotVector = 0.9f;
        if (dotVector < minDotVector)
        {
            // Landed too steeply
            print("Landed on too steep of an angle.");
            OnLanded?.Invoke(this, new OnLandedEventArgs
            {
                landingType = LandingType.TooSteepAngle,
                dotVector = dotVector,
                landingSpeed = relativeVelocityMagnitude,
                scoreMultiplier = 0f,
                score = 0,
            });
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
        OnLanded?.Invoke(this, new OnLandedEventArgs
        {
            landingType = LandingType.Success,
            dotVector = dotVector,
            landingSpeed = relativeVelocityMagnitude,
            scoreMultiplier = landingPad.ScoreMultiplier,
            score = score,
        });
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out FuelCollectable fuelCollectable))
            CurrentFuelAmount = _maxFuelAmount;

        if (collision.gameObject.TryGetComponent(out CoinCollectable coinCollectable))
            OnCoinPickup?.Invoke(this, EventArgs.Empty);

        if (collision.gameObject.TryGetComponent(out IDestroySelf destroyableObject))
            destroyableObject.DestroySelf();
    }

    public float GetSpeedX()
    {
        return _rb.linearVelocityX;
    }

    public float GetSpeedY()
    {
        return _rb.linearVelocityY;
    }

    public float GetFuelAmountNormalized()
    {
        return CurrentFuelAmount / _maxFuelAmount;
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
        CurrentFuelAmount -= fuelConsumptionAmount * Time.deltaTime;
    }

    private bool AnyThrustInputPressed()
    {
        return Keyboard.current.wKey.isPressed || Keyboard.current.aKey.isPressed || Keyboard.current.dKey.isPressed;
    }
}