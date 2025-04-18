using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;

    [Header("Components")]
    private Rigidbody2D _rb;

    // private SystemInputActions inputController;

    private float _inputX, _inputY;                 // TODO[x] 使用 input system
    private Vector2 _movementInput;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        // inputController = new SystemInputActions();
    }

    // private void OnEnable()
    // {
    //     inputController.Enable();
    // }

    // private void OnDisable()
    // {
    //     inputController.Disable();
    // }

    private void Update()
    {
        PlayerInput();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void PlayerInput()
    {
        // _inputX = Input.GetAxis("Horizontal");
        // _inputY = Input.GetAxis("vertical");

        // _movementInput = new Vector2(_inputX, _inputY).normalized;      // 归一化 放置斜方向速度太快
        // _movementInput = inputController.Player.Move.ReadValue<Vector2>().normalized;

        _movementInput = InputManager.Instance.MovementInput;
    }

    private void Movement()
    {
        _rb.MovePosition(_rb.position + _movementInput * speed * Time.deltaTime);
    }
}
