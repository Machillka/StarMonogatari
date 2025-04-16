using System.Collections.Generic;
using UnityEngine;

// 物品基础信息
[System.Serializable]
public class ItemDetails
{
    public int ItemID;
    public string ItemName;
    public ItemType ItemType;
    public Sprite ItemIcon;
    public Sprite ItemOnWorldSprite;
    public string ItemDescription;
    public int ItemUseRadius;

    // 状态信息
    public bool CanPickedup;
    public bool CanDropped;
    public bool CanCarried;

    public int ItemPrice;
    [Range(0, 1)]
    public float SellPercentage;
}

[System.Serializable]
public struct InventoryItem
{
    public int ItemID;
    public int ItemAmount;
}