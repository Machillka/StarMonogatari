using UnityEngine;
using System;
using UnityEngine.Events;

/// <summary>
/// 用于监听按键输入
/// </summary>
public class InputManager : Singleton<InputManager>
{
    private SystemInputActions _inputController;
    public Vector2 MovementInput{ get; protected set; }
    public bool IsShiftTimeButtonPressed;
    public bool IsShiftTimeButtonPressing;

    public UnityAction<Vector2> OnMoveInput;                                 // 移动事件

    private float _inputX, _inputY;

    protected override void Awake()
    {
        base.Awake();

        _inputController = new SystemInputActions();
    }

    private void OnEnable()
    {
        _inputController.Enable();
    }

    private void OnDisable()
    {
        _inputController.Disable();
    }

    private void Update()
    {
        MovementInput = _inputController.Player.Move.ReadValue<Vector2>();

        _inputX = MovementInput.x;
        _inputY = MovementInput.y;

        if (_inputX != 0 && _inputY != 0)
        {
            _inputX = _inputX * 0.6f;
            _inputY = _inputY * 0.6f;
        }

        MovementInput = new Vector2(_inputX, _inputY);

        IsShiftTimeButtonPressed = _inputController.Player.TimeShift.WasPressedThisFrame();
        IsShiftTimeButtonPressing = _inputController.Player.TimeShift.IsPressed();
    }
}
