using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;

    [Header("Components")]
    private Rigidbody2D _rb;

    private float _inputX, _inputY;                 // TODO[x] 使用 input system
    private Vector2 _movementInput;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        EventHandler.MoveToPosition += OnMoveToPosition;
    }

    private void OnDisable()
    {
        EventHandler.MoveToPosition -= OnMoveToPosition;
    }

    private void OnMoveToPosition(Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }

    private void Update()
    {
        if (InputManager.Instance.IsDisabledInput)
            _movementInput = Vector2.zero;
        else
            PlayerInput();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void PlayerInput()
    {
        _movementInput = InputManager.Instance.MovementInput;
    }

    private void Movement()
    {
        _rb.MovePosition(_rb.position + _movementInput * speed * Time.deltaTime);
    }
}
