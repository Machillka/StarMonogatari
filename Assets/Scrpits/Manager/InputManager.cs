using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// 用于监听按键输入
/// </summary>
public class InputManager : Singleton<InputManager>
{
    private SystemInputActions _inputController;
    public Vector2 MovementInput{ get; protected set; }
    public bool IsShiftTimeButtonPressed => _inputController.Player.TimeShift.WasPressedThisFrame();
    public bool IsShiftTimeButtonPressing => _inputController.Player.TimeShift.IsPressed();
    public bool IsSpaceButtonPressed => _inputController.Player.DialogEnter.WasPressedThisFrame();
    public bool IsExitShopButtonPressed => _inputController.Player.ExitShop.WasPressedThisFrame();
    public bool IsOpenBagButtonPressed => _inputController.Player.OpenBag.WasCompletedThisFrame();
    public bool IsLeftMouseButtonPressed;

    public UnityAction<Vector2> OnMoveInput;                                 // 移动事件

    public bool IsDisabledInput;

    private float _inputX, _inputY;

    protected override void Awake()
    {
        base.Awake();

        _inputController = new SystemInputActions();
    }

    private void OnEnable()
    {
        _inputController.Enable();
        _inputController.ActionBar.SelectSlot.performed += OnSelectSlotEvent;
        EventHandler.BeforeSceneLoadedEvent += OnBeforeSceneLoadedEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;
    }

    private void OnDisable()
    {
        _inputController.Disable();
        _inputController.ActionBar.SelectSlot.performed -= OnSelectSlotEvent;
        EventHandler.BeforeSceneLoadedEvent -= OnBeforeSceneLoadedEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;
    }

    private void OnUpdateGameStateEvent(GameStates gameState)
    {
        IsDisabledInput = gameState switch
        {
            GameStates.GamePlay => false,
            GameStates.Pause => true,
            _ => false
        };
    }

    private void OnSelectSlotEvent(InputAction.CallbackContext context)
    {
        // if (int.TryParse(context.control.name, out int index))
        // Debug.Log($"按下的按键为{index}");
        if (!IsDisabledInput)
            EventHandler.CallOnSelectSlotEvent(context);
    }

    private void OnAfterSceneLoadedEvent()
    {
        IsDisabledInput = false;
    }

    private void OnBeforeSceneLoadedEvent()
    {
        IsDisabledInput = true;
    }

    private void Update()
    {
        if (IsDisabledInput)
        {
            MovementInput = Vector2.zero;
            return;
        }

        MovementInput = _inputController.Player.Move.ReadValue<Vector2>();

        _inputX = MovementInput.x;
        _inputY = MovementInput.y;

        if (_inputX != 0 && _inputY != 0)
        {
            _inputX = _inputX * 0.6f;
            _inputY = _inputY * 0.6f;
        }

        MovementInput = new Vector2(_inputX, _inputY);
        IsLeftMouseButtonPressed = _inputController.Player.DropItem.WasPressedThisFrame();
    }
}
