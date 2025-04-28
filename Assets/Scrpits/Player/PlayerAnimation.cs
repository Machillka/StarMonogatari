using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Farm.Inventory;

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
        EventHandler.HarvestAtPlaterPositionEvent += OnHarvestAtPlaterPositionEvent;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelected;
        EventHandler.BeforeSceneLoadedEvent -= OnBeforeSceneLoadedEvent;
        EventHandler.MouseClickEvent -= OnMouseClickEvent;
        EventHandler.HarvestAtPlaterPositionEvent -= OnHarvestAtPlaterPositionEvent;
    }

    private void OnHarvestAtPlaterPositionEvent(int itemID)
    {
        Sprite itemSprite = InventoryManager.Instance.GetItemDetails(itemID).ItemOnWorldSprite;
        if (_holdItem.enabled == false)
        {
            // Debug.Log("start show Item");
            StartCoroutine(ShowItem(itemSprite));
        }
    }

    /// <summary>
    /// 播放动画并且实现效果
    /// </summary>
    /// <param name="mouseWorldPosition">鼠标的世界坐标</param>
    /// <param name="itemInformation">当前选中的 item 信息</param>
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
            // _mouseY = mouseWorldPosition.y - (transform.position.y + 0.85f);        // 位置到人物中间
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

    private IEnumerator ShowItem(Sprite itemSprite)
    {
        _holdItem.sprite = itemSprite;
        _holdItem.enabled = true;
        yield return new WaitForSeconds(0.35f);
        _holdItem.enabled = false;
    }

    private IEnumerator UseToolRoutine(Vector3 mouseWorldPosition, ItemDetails itemInformation)
    {
        // FIXME: 动作和取消输入之间有延迟
        _isUseTool = true;
        InputManager.Instance.IsDisabledInput = true;
        yield return null;

        foreach (var anim in _animators)
        {
            anim.SetTrigger("UseTool");
            anim.SetFloat("InputX", _mouseX);
            anim.SetFloat("InputY", _mouseY);
        }

        // 播放动画
        yield return new WaitForSeconds(0.35f);
        // 执行效果
        EventHandler.CallExcuteActionAfterAnimation(mouseWorldPosition, itemInformation);
        // 等待效果执行
        yield return new WaitForSeconds(0.25f);

        // 重新允许输入
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
            ItemType.CollectTool => PlayerHoldPartTypes.Collect,
            ItemType.WaterTool => PlayerHoldPartTypes.Water,
            ItemType.ChopTool => PlayerHoldPartTypes.Chop,
            ItemType.BreakTool => PlayerHoldPartTypes.Break,
            ItemType.ReapTool => PlayerHoldPartTypes.Reap,          //FIXME 镰刀有举起的动画
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
            else
            {
                _holdItem.enabled = false;
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
