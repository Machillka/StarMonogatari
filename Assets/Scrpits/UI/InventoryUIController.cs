using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

        private void Start()
        {
            for (int i = 0; i < PlayerSlots.Length; i++)
            {
                PlayerSlots[i].SlotIndex = i;
            }

            _isBagOpening = _bagUI.activeInHierarchy;
        }

        private void OnEnable()
        {
            EventHandler.UpdateInventoryUI += UpdateInventoryUI;
            EventHandler.BeforeSceneLoadedEvent += OnBeforeSceneLoadedEvent;
        }

        private void OnDisable()
        {
            EventHandler.UpdateInventoryUI -= UpdateInventoryUI;
            EventHandler.BeforeSceneLoadedEvent -= OnBeforeSceneLoadedEvent;
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
                    break;
            }
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

