using Unity.VisualScripting;
using UnityEngine;

namespace Farm.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        [Header("Item Data")]
        public ItemDataListSO itemDataListSO;
        [Header("Inventory Data")]
        public InventoryBagSO playerBag;


        /// <summary>
        /// Retrieves the details of an item based on its unique identifier (ID).
        /// </summary>
        /// <param name="ID">The unique identifier of the item to retrieve details for.</param>
        /// <returns>
        /// An <see cref="ItemDetails"/> object containing the details of the item with the specified ID.
        /// Returns null if no item with the given ID is found.
        /// </returns>
        public ItemDetails GetItemDetails(int ID)
        {
            return itemDataListSO.ItemDetailList.Find(item => item.ItemID == ID);
        }

        /// <summary>
        /// 生成新物品
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isDestory">是否要销毁</param>
        public void AddItem(Item item, bool isDestory)
        {
            if (isDestory)
            {
                Destroy(item.gameObject);
            }

            var indexInBag = GetItemIndexInBag(item.ItemID);

            AddItemAtIndex(item.ItemID, indexInBag, 1);

            //TODO: 优化逻辑
        }

        /// <summary>
        /// 判断物体是否在背包中
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns>添加的物品在背包中的id, 如果不存在则返回 -1</returns>
        public int GetItemIndexInBag(int itemID)
        {
            foreach (var item in playerBag.InventoryItemList)
            {
                if (item.ItemID == itemID)
                {
                    return item.ItemID;
                }
            }
            return -1;
        }

        /// <summary>
        /// 判断背包是否有空位
        /// </summary>
        /// <returns></returns>
        public bool CheckBagCapcity()
        {
            foreach (var item in playerBag.InventoryItemList)
            {
                if (item.ItemID == 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 在指定位置添加一定数量的物品
        /// </summary>
        /// <param name="ID">添加的物品的id</param>
        /// <param name="indexInBag">添加物品在背包中的id</param>
        /// <param name="amount">需要添加的数量</param>
        public void AddItemAtIndex(int ID, int indexInBag, int amount)
        {
            if (indexInBag == -1)                           // 背包里没有这个物体
            {
                if(!CheckBagCapcity())                      // 背包满了
                {
                    Debug.Log("背包已满");
                    return;
                }
                var item = new InventoryItem
                {
                    ItemID = ID,
                    ItemAmount = amount
                };

                for (int i = 0; i < playerBag.InventoryItemList.Count; i++)
                {
                    if (playerBag.InventoryItemList[i].ItemID == 0)
                    {
                        playerBag.InventoryItemList[i] = item;
                        break;
                    }
                }
            }
            else
            {
                var item = new InventoryItem
                {
                    ItemID = ID,
                    ItemAmount = amount + playerBag.InventoryItemList[indexInBag].ItemAmount
                };

                playerBag.InventoryItemList[indexInBag] = item;
            }
        }
    }
}

