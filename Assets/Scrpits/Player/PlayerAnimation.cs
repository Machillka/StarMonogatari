using System.Collections.Generic;
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

    private void OnMouseClickEvent(Vector3 position, ItemDetails itemInformation)
    {
        // TODO: 执行对应动画

        // 执行动画后的逻辑
        EventHandler.CallExcuteActionAfterAnimation(position, itemInformation);
    }

    private void OnBeforeSceneLoadedEvent()
    {
        _holdItem.enabled = false;
        SwitchAnimator(PlayerHoldPartTypes.None);

    }

    private void OnItemSelected(ItemDetails itemInformation, bool isSelected)
    {
        PlayerHoldPartTypes currentType = itemInformation.ItemType switch
        {
            ItemType.Seed => PlayerHoldPartTypes.Carry,
            ItemType.Commodity => PlayerHoldPartTypes.Carry,
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
