using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _holdItem;

    private Animator[] _animators;
    private bool _isMoving;

    [Header("各部分动画列表")]
    public List<AnimatorType> animatorTypes;

    private Dictionary<string, Animator> _animatorDict = new Dictionary<string, Animator>();

    [Header("工具动画相关")]
    private float _mouseX, _mouseY;
    private bool _isUseTool;

    private void Awake()
    {
        _animators = GetComponentsInChildren<Animator>();

        foreach (var anim in _animators)
        {
            _animatorDict.Add(anim.name, anim);
        }
    }

    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelected;
        EventHandler.BeforeSceneLoadedEvent += OnBeforeSceneLoadedEvent;
        EventHandler.MouseClickEvent += OnMouseClickEvent;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelected;
        EventHandler.BeforeSceneLoadedEvent -= OnBeforeSceneLoadedEvent;
        EventHandler.MouseClickEvent -= OnMouseClickEvent;
    }

    private void OnMouseClickEvent(Vector3 mouseWorldPosition, ItemDetails itemInformation)
    {
        if (_isUseTool)
        {
            return;
        }
        // TODO[x]:执行对应动画
        if (itemInformation.ItemType != ItemType.Seed
            && itemInformation.ItemType != ItemType.Commodity
            && itemInformation.ItemType != ItemType.Furniture)
        {
            _mouseX = mouseWorldPosition.x - transform.position.x;
            _mouseY = mouseWorldPosition.y - transform.position.y;

            if (Mathf.Abs(_mouseX) > Mathf.Abs(_mouseY))
            {
                _mouseY = 0f;
            }
            else
            {
                _mouseX = 0f;
            }

            StartCoroutine(UseToolRoutine(mouseWorldPosition, itemInformation));
        }

        else
        {
            EventHandler.CallExcuteActionAfterAnimation(mouseWorldPosition, itemInformation);
        }
    }

    private void OnBeforeSceneLoadedEvent()
    {
        _holdItem.enabled = false;
        SwitchAnimator(PlayerHoldPartTypes.None);

    }

    private IEnumerator UseToolRoutine(Vector3 mouseWorldPosition, ItemDetails itemInformation)
    {
        _isUseTool = true;
        InputManager.Instance.IsDisabledInput = true;
        yield return null;

        foreach (var anim in _animators)
        {
            anim.SetTrigger("UseTool");
            anim.SetFloat("InputX", _mouseX);
            anim.SetFloat("InputY", _mouseY);
        }

        yield return new WaitForSeconds(0.45f);
        EventHandler.CallExcuteActionAfterAnimation(mouseWorldPosition, itemInformation);
        yield return new WaitForSeconds(0.25f);

        _isUseTool = false;
        InputManager.Instance.IsDisabledInput = false;
    }

    private void OnItemSelected(ItemDetails itemInformation, bool isSelected)
    {
        //WORKFLOW 设置所不同工具返回不同的动画
        PlayerHoldPartTypes currentType = itemInformation.ItemType switch
        {
            ItemType.Seed => PlayerHoldPartTypes.Carry,
            ItemType.Commodity => PlayerHoldPartTypes.Carry,
            ItemType.HoeTool => PlayerHoldPartTypes.Hoe,
            _ => PlayerHoldPartTypes.None
        };
        if (isSelected == false)
        {
            currentType = PlayerHoldPartTypes.None;
            _holdItem.enabled = false;
        }
        else
        {
            if (currentType == PlayerHoldPartTypes.Carry)
            {
                _holdItem.sprite = itemInformation.ItemOnWorldSprite;
                _holdItem.enabled = true;
            }
        }
        SwitchAnimator(currentType);
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
            anim.SetFloat("MouseX", _mouseX);
            anim.SetFloat("MouseY", _mouseY);
            if (_isMoving)
            {
                anim.SetFloat("InputX", InputManager.Instance.MovementInput.x);
                anim.SetFloat("InputY", InputManager.Instance.MovementInput.y);
            }
        }
    }

    private void SwitchAnimator(PlayerHoldPartTypes holdType)
    {
        foreach (var item in animatorTypes)
        {
            if (item.holdType == holdType)
            {
                _animatorDict[item.bodyPart.ToString()].runtimeAnimatorController = item.overrideController;
            }
        }
    }
}
