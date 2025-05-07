using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataListSO", menuName = "Inventory/ItemDataListSO")]
public class ItemDataListSO : ScriptableObject
{
    public List<ItemDetails> ItemDetailList;

}
