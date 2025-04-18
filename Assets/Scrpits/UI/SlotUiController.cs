using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

        private InventoryUIController _inventoryUI => GetComponentInParent<InventoryUIController>();

        private void Start()
        {
            IsSelected = false;
            if (SlotItem.ItemID == 0)
            {
                EmptySlot();
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

                // 在底栏和背包中进行交换
                if (SlotType == SlotTypes.Bag && targetSlot.SlotType == SlotTypes.Bag)
                {
                    InventoryManager.Instance.SwapPlayerBagItem(SlotIndex, targetSlotIndex);
                }
            }
            else
            {
                if (SlotItem.CanDropped)
                {
                    var pos = Camera.main.ScreenToWorldPoint(
                    new Vector3(
                            Input.mousePosition.x,
                            Input.mousePosition.y,
                            -Camera.main.transform.position.z
                        )
                    );

                    EventHandler.CallInstantiateItemInScene(SlotItem.ItemID, pos);
                }
            }

            //TODO: 移动后的物体，依旧可以点按出现高亮 -> 数据没有清除
            _inventoryUI.UpdateSlotHighlight(-1);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _inventoryUI.DragtItemImage.transform.position = Input.mousePosition;//TODO: 使用input system
        }
    }
}

