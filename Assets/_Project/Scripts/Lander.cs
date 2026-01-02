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
    public event EventHandler OnFuelPickup;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }
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

    public enum State
    {
        WaitingToStart,
        Normal,
        GameOver,
    }

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private float _softLandingVelocityMagnitude = 3f;
    [SerializeField] private float _maxFuelAmount = 10f;

    private const float GRAVITY_NORMAL = 0.7f;

    private Rigidbody2D _rb;
    private State _state;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;

        CurrentFuelAmount = _maxFuelAmount;
        _state = State.WaitingToStart;
    }

    private void FixedUpdate()
    {
        OnBeforeForce?.Invoke(this, EventArgs.Empty);

        switch (_state)
        {
            case State.WaitingToStart:
                if (AnyThrustInputPressed())
                {
                    _rb.gravityScale = GRAVITY_NORMAL;
                    SetState(State.Normal);
                }
                break;
            case State.Normal:
                if (CurrentFuelAmount <= 0f)
                    return;

                if (AnyThrustInputPressed())
                    ConsumeFuel();

                float gamepadDeadzone = 0.2f;
                if (GameInputManager.Instance.IsUpActionPressed() || GameInputManager.Instance.GetMoveInputVector().y > gamepadDeadzone)
                {
                    _rb.AddForce(_moveSpeed * Time.deltaTime * transform.up);
                    OnUpForce?.Invoke(this, EventArgs.Empty);
                }
                if (GameInputManager.Instance.IsLeftActionPressed() || GameInputManager.Instance.GetMoveInputVector().x < -gamepadDeadzone)
                {
                    _rb.AddTorque(_rotateSpeed * Time.deltaTime);
                    OnLeftForce?.Invoke(this, EventArgs.Empty);
                }
                if (GameInputManager.Instance.IsRightActionPressed() || GameInputManager.Instance.GetMoveInputVector().x > gamepadDeadzone)
                {
                    _rb.AddTorque(-_rotateSpeed * Time.deltaTime);
                    OnRightForce?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GameOver:
                break;
            default:
                break;
        }
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
            SetState(State.GameOver);
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
            SetState(State.GameOver);
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
            SetState(State.GameOver);
            return;
        }

        print("Successful landing");

        float maxScoreAmountLandingAngle = 100f;
        float scoreDotVectorMultiplier = 10f;
        float landingAngleScore = maxScoreAmountLandingAngle - Mathf.Abs(dotVector - 1f) * scoreDotVectorMultiplier;
        float maxScoreAmountLandingSpeed = 100f;
        float landingSpeedScore = (_softLandingVelocityMagnitude - relativeVelocityMagnitude) * maxScoreAmountLandingSpeed;

        int score = Mathf.RoundToInt((landingAngleScore + landingSpeedScore) * landingPad.ScoreMultiplier);

        OnLanded?.Invoke(this, new OnLandedEventArgs
        {
            landingType = LandingType.Success,
            dotVector = dotVector,
            landingSpeed = relativeVelocityMagnitude,
            scoreMultiplier = landingPad.ScoreMultiplier,
            score = score,
        });
        SetState(State.GameOver);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out FuelCollectable fuelCollectable))
        {
            CurrentFuelAmount = _maxFuelAmount;
            OnFuelPickup?.Invoke(this, EventArgs.Empty);
        }

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

    private void ConsumeFuel()
    {
        float fuelConsumptionAmount = 1f;
        CurrentFuelAmount -= fuelConsumptionAmount * Time.deltaTime;
    }

    private bool AnyThrustInputPressed()
    {
        return GameInputManager.Instance.IsUpActionPressed() 
            || GameInputManager.Instance.IsLeftActionPressed() 
            || GameInputManager.Instance.IsRightActionPressed()
            || GameInputManager.Instance.GetMoveInputVector() != Vector2.zero;
    }

    private void SetState(State state)
    {
        _state = state;
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
        {
            state = _state
        });
    }
}