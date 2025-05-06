using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


namespace Farm.Inventory
{
    public class TradeUIController : MonoBehaviour
    {
        [Header("UI Component")]
        public Image itemIcon;
        public TextMeshProUGUI itemName;
        public TMP_InputField tradeAmount;
        public Button submitBtn;
        public Button cancelBtn;

        private ItemDetails _currentItem;
        private bool _isSellTrade;

        private void Awake()
        {
            cancelBtn.onClick.AddListener(CancelTrade);
            submitBtn.onClick.AddListener(SubmitTradeItem);
        }

        public void SetupTradeUI(ItemDetails item, bool isSell)
        {
            _currentItem = item;
            itemIcon.sprite = item.ItemIcon;
            itemName.text = item.ItemName;
            _isSellTrade = isSell;
            tradeAmount.text = string.Empty;
        }

        private void SubmitTradeItem()
        {
            int amount = Convert.ToInt32(tradeAmount.text);
            InventoryManager.Instance.TradeItem(_currentItem, amount, _isSellTrade);
            CancelTrade();
        }

        private void CancelTrade()
        {
            gameObject.SetActive(false);
        }
    }
}
