using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using JetBrains.Rider.Unity.Editor;

namespace Farm.Inventory
{
    // FIXME: 通过单例运行 Inventory
    public class InventoryUIController : MonoBehaviour
    {
        public ItemToolTipContoller ItemToolTip;
        [SerializeField] private GameObject _bagUI;
        //TODO: 不通过拖拽赋值的方式
        [SerializeField] private SlotUiController[] PlayerSlots;

        public Image DragtItemImage;

        private bool _isBagOpening;

        [Header("通用背包")]
        [SerializeField] private GameObject baseBag;
        public GameObject shopSlotPrefab;
        [SerializeField] private List<SlotUiController> baseBagSlots;
        public GameObject boxSlotPrefab;

        [Header("交易UI")]
        public TradeUIController tradeUI;
        public TextMeshProUGUI playerCurrencyText;

        private void Start()
        {
            for (int i = 0; i < PlayerSlots.Length; i++)
            {
                PlayerSlots[i].SlotIndex = i;
            }

            _isBagOpening = _bagUI.activeInHierarchy;
            playerCurrencyText.text = InventoryManager.Instance.playerCurrency.ToString();
        }

        private void OnEnable()
        {
            EventHandler.UpdateInventoryUI += UpdateInventoryUI;
            EventHandler.BeforeSceneLoadedEvent += OnBeforeSceneLoadedEvent;
            EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;
            EventHandler.BaseBagCloseEvent += OnBaseBagCloseEvent;
            EventHandler.ShowTradeUIEvent += OnShowTradeUIEvent;
        }

        private void OnDisable()
        {
            EventHandler.UpdateInventoryUI -= UpdateInventoryUI;
            EventHandler.BeforeSceneLoadedEvent -= OnBeforeSceneLoadedEvent;
            EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;
            EventHandler.BaseBagCloseEvent -= OnBaseBagCloseEvent;
            EventHandler.ShowTradeUIEvent += OnShowTradeUIEvent;
        }

        private void OnBaseBagOpenEvent(SlotTypes slotType, InventoryBagSO bagData)
        {
            //TODO 添加箱子内容
            GameObject prefab = slotType switch
            {
                SlotTypes.Shop => shopSlotPrefab,
                SlotTypes.Box => boxSlotPrefab,
                _ => null
            };

            // 生成内容
            baseBag.SetActive(true);
            // Debug.Log(baseBag);
            baseBagSlots = new List<SlotUiController>();
            for (int i = 0; i < bagData.InventoryItemList.Count; i++)
            {
                var slot = Instantiate(prefab, baseBag.transform.GetChild(0)).GetComponent<SlotUiController>();
                slot.SlotIndex = i;
                baseBagSlots.Add(slot);
            }

            if (slotType == SlotTypes.Shop)
            {
                // 显示玩家背包
                _bagUI.GetComponent<RectTransform>().pivot = new Vector2(-1, 0.5f);
                _bagUI.SetActive(true);
                _isBagOpening = true;
            }

            // 更新UI
            LayoutRebuilder.ForceRebuildLayoutImmediate(baseBag.GetComponent<RectTransform>());
            UpdateInventoryUI(InventoryLocation.Box, bagData.InventoryItemList);
        }

        private void OnBaseBagCloseEvent(SlotTypes slotType, InventoryBagSO bagData)
        {
            baseBag.SetActive(false);
            ItemToolTip.gameObject.SetActive(false);
            UpdateSlotHighlight(-1);

            foreach (var slot in baseBagSlots)
            {
                Destroy(slot.gameObject);
            }
            baseBagSlots.Clear();

            if (slotType == SlotTypes.Shop)
            {
                // 关闭玩家背包
                _bagUI.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                _bagUI.SetActive(false);
                _isBagOpening = false;
            }
        }

        private void OnShowTradeUIEvent(ItemDetails item, bool isSell)
        {
            tradeUI.gameObject.SetActive(true);
            tradeUI.SetupTradeUI(item, isSell);
        }

        private void OnBeforeSceneLoadedEvent()
        {
            UpdateSlotHighlight(-1);
        }

        public void UpdateInventoryUI(InventoryLocation location, List<InventoryItem> items)
        {
            switch (location)
            {
                case InventoryLocation.Player:
                    for (int i = 0; i < PlayerSlots.Length; i++)
                    {
                        if (items[i].ItemAmount > 0)
                        {
                            var item = InventoryManager.Instance.GetItemDetails(items[i].ItemID);
                            PlayerSlots[i].UpdateSlot(item, items[i].ItemAmount);
                        }
                        else
                        {
                            PlayerSlots[i].EmptySlot();
                        }
                    }
                    break;
                case InventoryLocation.Box:
                    for (int i = 0; i < baseBagSlots.Count; i++)
                    {
                        if (items[i].ItemAmount > 0)
                        {
                            var item = InventoryManager.Instance.GetItemDetails(items[i].ItemID);
                            baseBagSlots[i].UpdateSlot(item, items[i].ItemAmount);
                        }
                        else
                        {
                            baseBagSlots[i].EmptySlot();
                        }
                    }
                    break;
            }
            playerCurrencyText.text = InventoryManager.Instance.playerCurrency.ToString();
        }

        /// <summary>
        /// 打开或关闭背包UI面板
        /// </summary>
        public void OpenBagUI()
        {
            _isBagOpening = !_isBagOpening;
            _bagUI.SetActive(_isBagOpening);
        }

        /// <summary>
        /// 更新高亮UI显示
        /// </summary>
        /// <param name="index"></param>
        public void UpdateSlotHighlight(int index)
        {
            foreach (var slot in PlayerSlots)
            {
                if (slot.SlotIndex == index && slot.IsSelected)
                {
                    slot.SlotHighlightImage.gameObject.SetActive(true);
                }
                else
                {
                    slot.IsSelected = false;
                    slot.SlotHighlightImage.gameObject.SetActive(false);
                }
            }
        }
    }
}

