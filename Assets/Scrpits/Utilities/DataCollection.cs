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

[System.Serializable]
public class SerializableVector3
{
    public float x;
    public float y;
    public float z;

    public SerializableVector3(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }

    public Vector2Int ToVector2()
    {
        return new Vector2Int((int)x, (int)y);
    }
}

[System.Serializable]
public class SceneItem
{
    public int ItemID;
    public SerializableVector3 Position;
}

[System.Serializable]
public class TileProperty
{
    public Vector2Int TileCoordinate;
    public GridTypes TileType;
    public bool BoolTypeValue;
}

[System.Serializable]
public class TileDetails
{
    public int GridX, GridY;
    public bool CanDig;
    public bool CanDropItem;//TODO: 取反一下，选取不能仍东西的逻辑可能空间消耗会更少
    public bool CanPlaceFurniture;
    public bool IsNPCObstacle;

    public int daySinceDug = -1;
    public int daySinceWatered = -1;

    public int seedItemID = -1;
    public int growthDays = -1;

    public int daySinceLastHarvest = -1;

    public GridTypes Itemtype;
}

[System.Serializable]
public class NPCPosition
{
    public Transform npcTransform;
    public string startSceneName;
    public Vector3 position;
}