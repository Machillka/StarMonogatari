using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryBagSO", menuName = "Inventory/InventoryBagSO")]
public class InventoryBagSO : ScriptableObject
{
    public List<InventoryItem> InventoryItemList;

    public InventoryItem GetInventoryItem(int itemID)
    {
        return InventoryItemList.Find(item => item.ItemID == itemID);
    }
}
