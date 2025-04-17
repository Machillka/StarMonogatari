using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Farm.Inventory
{
    public class InventoryUIController : MonoBehaviour
    {
        [SerializeField] private GameObject _bagUI;
        //TODO: 不通过拖拽赋值的方式
        [SerializeField] private SlotUiController[] PlayerSlots;

        private bool _isBagOpening;

        private void Start()
        {
            for (int i = 0; i < PlayerSlots.Length; i++)
            {
                PlayerSlots[i].ItemIndex = i;
            }

            _isBagOpening = _bagUI.activeInHierarchy;
        }

        private void OnEnable()
        {
            EventHandler.UpdateInventoryUI += UpdateInventoryUI;
        }

        private void OnDisable()
        {
            EventHandler.UpdateInventoryUI -= UpdateInventoryUI;
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

        public void OpenBagUI()
        {
            _isBagOpening = !_isBagOpening;
            _bagUI.SetActive(_isBagOpening);
        }
    }
}

