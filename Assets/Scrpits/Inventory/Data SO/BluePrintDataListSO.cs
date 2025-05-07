using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BluePrintDataListSO", menuName = "Inventory/BluePrintDataListSO", order = 0)]
public class BluePrintDataListSO : ScriptableObject
{
    public List<BluePrintDetails> bluePrints;

    /// <summary>
    /// 通过 itemID 查找图纸
    /// </summary>
    /// <param name="itemID"></param>
    /// <returns></returns>
    public BluePrintDetails GetBluePrintDetails(int itemID)
    {
        return bluePrints.Find(item => item.ID == itemID);
    }
}

[System.Serializable]
public class BluePrintDetails
{
    public int ID;
    public InventoryItem[] resourceItems = new InventoryItem[4];
    public GameObject buildPrefab;
}