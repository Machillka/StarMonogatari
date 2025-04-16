using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryBagSO", menuName = "Invotory/InventoryBagSO")]
public class InventoryBagSO : ScriptableObject
{
    public List<InventoryItem> InventoryItemList;
}
