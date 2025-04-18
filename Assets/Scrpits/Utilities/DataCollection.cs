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
public struct InventoryItem                 // 结构体默认初始化为0, 所以可以作为特判的标准
{
    public int ItemID;                      // 存储 ID -> 对应在物品数据库中的 ID, 可供查找
    public int ItemAmount;                  // 存储该物体的数量
}

[System.Serializable]
public class AnimatorType
{
    public PlayerHoldPartTypes holdType;
    public PlayerBodyParts bodyPart;
    public AnimatorOverrideController overrideController;
}