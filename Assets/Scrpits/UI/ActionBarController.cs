// using UnityEngine;

// namespace Farm.Inventory
// {
//     [RequireComponent(typeof(SlotUiController))]
//     public class ActionBarController : MonoBehaviour
//     {
//         private SlotUiController _slotUi;

//         private void Awake()
//         {
//             _slotUi = GetComponent<SlotUiController>();
//         }

//         private void Update()
//         {
//             if (Input.GetKeyDown(KeyCode.Alpha1))
//             {
//                 if (_slotUi.SlotItem != null)
//                 {
//                     _slotUi.IsSelected = !_slotUi.IsSelected;
//                     if (_slotUi.IsSelected)
//                     {
//                         _slotUi.InventoryUI.UpdateSlotHighlight(_slotUi.SlotIndex);
//                     }
//                     else
//                     {
//                         _slotUi.InventoryUI.UpdateSlotHighlight(-1);
//                     }

//                     EventHandler.CallItemSelectedEvent(_slotUi.SlotItem, _slotUi.IsSelected);
//                 }
//             }
//         }
//     }
// }

