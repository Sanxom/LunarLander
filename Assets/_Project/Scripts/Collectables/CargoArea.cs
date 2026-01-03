using System;
using UnityEngine;

public class CargoArea : MonoBehaviour
{
    public event EventHandler OnInteractTimerChanged;
    public event EventHandler OnPickedUp;
    public event EventHandler OnDroppedOff;

    public enum InteractType
    {
        PickUp,
        Drop,
    }

    [SerializeField] private InteractType _interactType;
    [SerializeField] private float _interactTimerMax = 2f;

    private float _interactTimer;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Lander lander))
        {
            _interactTimer += Time.deltaTime;
            OnInteractTimerChanged?.Invoke(this, EventArgs.Empty);
            if (_interactTimer > _interactTimerMax)
            {
                switch (_interactType)
                {
                    case InteractType.PickUp:
                        lander.PickUpCargo();
                        OnPickedUp?.Invoke(this, EventArgs.Empty);
                        break;
                    case InteractType.Drop:
                        lander.DropCargo();
                        OnDroppedOff?.Invoke(this, EventArgs.Empty);
                        break;
                    default:
                        break;
                }
                DestroySelf();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _interactTimer = 0f;
        OnInteractTimerChanged?.Invoke(this, EventArgs.Empty);
    }

    public float GetInteractTimerNormalized()
    {
        return _interactTimer / _interactTimerMax;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}