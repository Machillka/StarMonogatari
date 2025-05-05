using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Farm.Inventory
{
    public class SlotUiController : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image _slotImage;
        [SerializeField] private TextMeshProUGUI _amountText;
        [SerializeField] public Image SlotHighlightImage;
        [SerializeField] private Button _button;

        public SlotTypes SlotType;
        public bool IsSelected;

        public ItemDetails SlotItem;
        public int ItemAmount;
        public int SlotIndex;

        // public InventoryUIController InventoryUI => _inventoryUI;
        private InventoryUIController _inventoryUI => GetComponentInParent<InventoryUIController>();

        private void Start()
        {
            IsSelected = false;
            if (SlotItem.ItemID == 0)
            {
                EmptySlot();
            }
        }

        private void OnEnable()
        {
            EventHandler.SelectSlotEvent += OnSelectSlotEvent;
        }

        private void OnDisable()
        {
            EventHandler.SelectSlotEvent -= OnSelectSlotEvent;
        }

        private void OnSelectSlotEvent(InputAction.CallbackContext context)
        {
            string keyPressed = context.control.name;

            if (int.TryParse(keyPressed, out int index))
            {
                // slot -> 0, 1, 2, 3, ..., 8, 9
                //index -> 1, 2, 3, 4, ..., 9, 0
                if (index != (SlotIndex + 1) % 10 || ItemAmount == 0)
                    return;

                IsSelected = !IsSelected;

                if (IsSelected)
                    _inventoryUI.UpdateSlotHighlight(SlotIndex);
                else
                    _inventoryUI.UpdateSlotHighlight(-1);

                if (SlotType == SlotTypes.Bag)
                    EventHandler.CallItemSelectedEvent(SlotItem, IsSelected);
            }
        }

        /// <summary>
        /// 清空格子数据
        /// </summary>
        public void EmptySlot()
        {
            if (IsSelected)
            {
                IsSelected = false;
                _inventoryUI.UpdateSlotHighlight(-1);
                EventHandler.CallItemSelectedEvent(SlotItem, IsSelected);
            }

            _slotImage.enabled = false;
            _amountText.text = string.Empty;
            _button.interactable = false;
            SlotItem = new ItemDetails();
            ItemAmount = 0;
        }

        /// <summary>
        ///更新背包格子数据信息同时启用图片
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        public void UpdateSlot(ItemDetails item, int amount)
        {
            SlotItem = item;
            ItemAmount = amount;

            _slotImage.sprite = item.ItemIcon;
            _amountText.text = amount.ToString();

            _button.interactable = true;
            _slotImage.enabled = true;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (ItemAmount == 0)
                return;

            IsSelected = !IsSelected;
            _inventoryUI.UpdateSlotHighlight(SlotIndex);

            if (SlotType == SlotTypes.Bag)
            {
                EventHandler.CallItemSelectedEvent(SlotItem, IsSelected);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (ItemAmount != 0)
            {
                _inventoryUI.DragtItemImage.enabled = true;
                _inventoryUI.DragtItemImage.sprite = _slotImage.sprite;
                _inventoryUI.DragtItemImage.SetNativeSize();
                IsSelected = true;

                _inventoryUI.UpdateSlotHighlight(SlotIndex);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _inventoryUI.DragtItemImage.enabled = false;

            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                if (eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUiController>() == null)
                    return;

                var targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUiController>();
                int targetSlotIndex = targetSlot.SlotIndex;
                // Debug.Log($"Current Type: {SlotType} Target Type: {targetSlot.SlotType}");
                // 在底栏和背包中进行交换
                if (SlotType == SlotTypes.Bag && targetSlot.SlotType == SlotTypes.Bag)
                {
                    InventoryManager.Instance.SwapPlayerBagItem(SlotIndex, targetSlotIndex);
                }
                else if (SlotType == SlotTypes.Shop && targetSlot.SlotType == SlotTypes.Bag)        // 买东西
                {
                    EventHandler.CallShowTradeUIEvent(SlotItem, false);
                }
                else if (SlotType == SlotTypes.Bag && targetSlot.SlotType == SlotTypes.Shop)        // 卖
                {
                    EventHandler.CallShowTradeUIEvent(SlotItem, true);
                }

                // 清空高亮
                _inventoryUI.UpdateSlotHighlight(-1);
            }
            // else
            // {
            //     if (SlotItem.CanDropped)
            //     {
            //         var pos = Camera.main.ScreenToWorldPoint(
            //         new Vector3(
            //                 Input.mousePosition.x,
            //                 Input.mousePosition.y,
            //                 -Camera.main.transform.position.z
            //             )
            //         );

            //         EventHandler.CallInstantiateItemInScene(SlotItem.ItemID, pos);
            //     }
            // }

            //TODO[x]: 移动后的物体，依旧可以点按出现高亮 -> 数据没有清除
            _inventoryUI.UpdateSlotHighlight(-1);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _inventoryUI.DragtItemImage.transform.position = Input.mousePosition;//TODO: 使用input system
        }
    }
}

