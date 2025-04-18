using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator[] _animators;
    private bool _isMoving;

    private void Awake()
    {
        _animators = GetComponentsInChildren<Animator>();
    }

    private void Update()
    {
        _isMoving = InputManager.Instance.MovementInput != Vector2.zero;
        SwitchAnimation();
    }

    private void SwitchAnimation()
    {
        foreach (var anim in _animators)
        {
            anim.SetBool("IsMoving", _isMoving);
            if (_isMoving)
            {
                anim.SetFloat("InputX", InputManager.Instance.MovementInput.x);
                anim.SetFloat("InputY", InputManager.Instance.MovementInput.y);
            }
        }
    }
}
