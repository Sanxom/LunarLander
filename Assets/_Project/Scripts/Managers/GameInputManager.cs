using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputManager : MonoBehaviour
{
    public static GameInputManager Instance { get; private set; }

    public event EventHandler OnMenuButtonPressed;

    private GameInput _gameInput;
    private InputAction _landerUpAction;
    private InputAction _landerLeftAction;
    private InputAction _landerRightAction;
    private InputAction _moveJoystickAction;
    private InputAction _menuAction;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _gameInput = new();
        _landerUpAction = _gameInput.Gameplay.LanderUp;
        _landerLeftAction = _gameInput.Gameplay.LanderLeft;
        _landerRightAction = _gameInput .Gameplay.LanderRight;
        _moveJoystickAction = _gameInput.Gameplay.Move;
        _menuAction = _gameInput.Gameplay.Menu;
        _menuAction.performed += MenuAction_Performed;
        _gameInput.Enable();
    }

    private void OnDestroy()
    {
        _menuAction.performed -= MenuAction_Performed;
        _gameInput.Disable();
    }

    public Vector2 GetMoveInputVector() => _moveJoystickAction.ReadValue<Vector2>();
    public bool IsUpActionPressed() => _landerUpAction.IsPressed();
    public bool IsLeftActionPressed() => _landerLeftAction.IsPressed();
    public bool IsRightActionPressed() => _landerRightAction.IsPressed();
    private void MenuAction_Performed(InputAction.CallbackContext obj) => OnMenuButtonPressed?.Invoke(this, EventArgs.Empty);
}