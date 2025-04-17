using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.Inventory
{
    public class SlotUiController : MonoBehaviour
    {
        [Header("Component")]
        [SerializeField] private Image _slotImage;
        [SerializeField] private TextMeshProUGUI _amountText;
        [SerializeField] private Image _slotHighlightImage;
        [SerializeField] private Button _button;

        public SlotTypes SlotType;
        public bool IsSelected;

        public ItemDetails SlotItem;
        public int ItemAmount;
        public int ItemIndex;

        private void Start()
        {
            IsSelected = false;
            if (SlotItem.ItemID == 0)
            {
                EmptySlot();
            }
        }

        /// <summary>
        /// 清空格子
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
        }

        /// <summary>
        ///更新背包格子数据信息
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
    }
}

