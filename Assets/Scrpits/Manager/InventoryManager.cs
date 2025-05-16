using System.Collections.Generic;
using UnityEngine;

namespace Farm.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        [Header("Item Data")]
        public ItemDataListSO itemDataListSO;
        [Header("Inventory Data")]
        public InventoryBagSO playerBag;
        private InventoryBagSO currentBoxBag;

        [Header("Blueprint Data")]
        public BluePrintDataListSO bluePrintData;

        [Header("Currency")]
        public int playerCurrency;

        private void Start()
        {
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.InventoryItemList);
        }

        private void OnEnable()
        {
            EventHandler.DropItemInScene += OnDropItemInScene;
            EventHandler.HarvestAtPlaterPositionEvent += OnHarvestAtPlaterPositionEvent;
            EventHandler.BuildFurnitureEvent += OnBuildFurnitureEvent;
            EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;
        }

        private void OnDisable()
        {
            EventHandler.DropItemInScene -= OnDropItemInScene;
            EventHandler.HarvestAtPlaterPositionEvent -= OnHarvestAtPlaterPositionEvent;
            EventHandler.BuildFurnitureEvent -= OnBuildFurnitureEvent;
            EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;
        }

        private void OnBaseBagOpenEvent(SlotTypes slotType, InventoryBagSO bagData)
        {
            currentBoxBag = bagData;
        }

        private void OnBuildFurnitureEvent(int itemID, Vector3 position)
        {
            RemoveItem(itemID, 1);
            BluePrintDetails bluePrint = bluePrintData.GetBluePrintDetails(itemID);

            foreach (var resourceItem in bluePrint.resourceItems)
            {
                RemoveItem(resourceItem.ItemID, resourceItem.ItemAmount);
            }

        }

        private void OnHarvestAtPlaterPositionEvent(int itemID)
        {
            AddItem(itemID);
        }

        private void OnDropItemInScene(int itemID, Vector3 pos, ItemType itemType)
        {
            RemoveItem(itemID, 1);
        }

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
            // Debug.Log($"Index:{indexInBag}");
            AddItemAtIndex(item.ItemID, indexInBag, 1);

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.InventoryItemList);

            //TODO: 优化逻辑
        }

        public void AddItem(int itemID)
        {
            var indexInBag = GetItemIndexInBag(itemID);
            // Debug.Log($"Index:{indexInBag}");
            AddItemAtIndex(itemID, indexInBag, 1);

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.InventoryItemList);

        }

        /// <summary>
        /// 判断物体是否在背包中
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns>添加的物品在玩家背包中的id, 如果不存在则返回 -1</returns>
        public int GetItemIndexInBag(int itemID)
        {
            // Debug.Log($"In Function, itemid = {itemID}");
            for (int i = 0; i < playerBag.InventoryItemList.Count; i++)
            {
                if (playerBag.InventoryItemList[i].ItemID == itemID)
                {
                    return i;
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
                if (!CheckBagCapcity())                      // 背包满了
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
                        // Debug.Log($"i = {i}");
                        break;
                    }
                }
            }
            else
            {
                // Debug.Log($"ID:{ID}, Amount:{amount + playerBag.InventoryItemList[indexInBag].ItemAmount}");
                // Debug.Log($"indexInBag{indexInBag}");
                var item = new InventoryItem
                {
                    ItemID = ID,
                    ItemAmount = amount + playerBag.InventoryItemList[indexInBag].ItemAmount
                };

                playerBag.InventoryItemList[indexInBag] = item;
            }
        }

        public void SwapPlayerBagItem(int fromIndex, int targetIndex)
        {
            InventoryItem currentItem = playerBag.InventoryItemList[fromIndex];
            InventoryItem targetItem = playerBag.InventoryItemList[targetIndex];

            if (targetItem.ItemID != 0)
            {
                playerBag.InventoryItemList[fromIndex] = targetItem;
                playerBag.InventoryItemList[targetIndex] = currentItem;
            }
            else
            {
                playerBag.InventoryItemList[fromIndex] = new InventoryItem();
                playerBag.InventoryItemList[targetIndex] = currentItem;
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.InventoryItemList);
        }

        /// <summary>
        /// swap item datas between two different bags
        /// </summary>
        /// <param name="locationFrom"></param>
        /// <param name="fromIndex"></param>
        /// <param name="locationTarget"></param>
        /// <param name="targetIndex"></param>
        public void SwapItem(InventoryLocation locationFrom, int fromIndex, InventoryLocation locationTarget, int targetIndex)
        {
            var currentList = GetItemList(locationFrom);
            var targetList = GetItemList(locationTarget);

            InventoryItem currentItem = currentList[fromIndex];

            if (targetIndex < targetList.Count)
            {
                InventoryItem targetItem = targetList[targetIndex];

                // Drag two different items
                if (targetItem.ItemID != 0 && currentItem.ItemID != targetItem.ItemID)
                {
                    currentList[fromIndex] = targetItem;
                    targetList[targetIndex] = currentItem;
                }
                // two same items
                else if (currentItem.ItemID == targetItem.ItemID)
                {
                    targetItem.ItemAmount += currentItem.ItemAmount;
                    currentList[fromIndex] = new InventoryItem();
                    targetList[targetIndex] = targetItem;
                }
                // target is a empty slot   //TODO: Combine case 1 and case 3
                else
                {
                    targetList[targetIndex] = currentItem;
                    currentList[fromIndex] = new InventoryItem();
                }

                EventHandler.CallUpdateInventoryUI(locationFrom, currentList);
                EventHandler.CallUpdateInventoryUI(locationTarget, targetList);

            }
        }

        /// <summary>
        /// 移出指定数量的玩家背包内的物品
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="removeAmount"></param>
        public void RemoveItem(int itemID, int removeAmount)
        {
            var indexInGag = GetItemIndexInBag(itemID);

            if (playerBag.InventoryItemList[indexInGag].ItemAmount > removeAmount)
            {
                var amount = playerBag.InventoryItemList[indexInGag].ItemAmount - removeAmount;
                var item = new InventoryItem
                {
                    ItemID = itemID,
                    ItemAmount = amount
                };
                playerBag.InventoryItemList[indexInGag] = item;
            }
            else if (playerBag.InventoryItemList[indexInGag].ItemAmount == removeAmount)
            {
                playerBag.InventoryItemList[indexInGag] = new InventoryItem();
            }
            else
            {
                // TODO: 商品数量不够逻辑
            }
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.InventoryItemList);
        }

        /// <summary>
        /// 交易物品
        /// </summary>
        /// <param name="item">交易物品种类</param>
        /// <param name="amount">交易数量</param>
        /// <param name="isSellTrade">标记是否购买</param>
        public void TradeItem(ItemDetails item, int amount, bool isSellTrade)
        {
            int cost = item.ItemPrice * amount;
            int index = GetItemIndexInBag(item.ItemID);

            if (isSellTrade)            // 卖出商品
            {
                if (playerBag.InventoryItemList[index].ItemAmount >= amount)
                {
                    RemoveItem(item.ItemID, amount);
                    cost = (int)(cost * item.SellPercentage);
                    playerCurrency += cost;
                }
            }
            else
            {
                if (playerCurrency - cost >= 0)
                {
                    if (CheckBagCapcity())
                    {
                        AddItemAtIndex(item.ItemID, index, amount);
                    }
                    playerCurrency -= cost;
                }
            }
            //TODO 人物动画问题 —— 选中物品的动画没有取消
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.InventoryItemList);
        }

        /// <summary>
        /// 检查物品库存是否符合建造数量
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public bool CheckStock(int itemID)
        {
            var bluePrintInformation = bluePrintData.GetBluePrintDetails(itemID);

            foreach (var resourceItem in bluePrintInformation.resourceItems)
            {
                // Debug.Log($"Resource ItemID = {resourceItem.ItemID}, Need = {resourceItem.ItemAmount}");
                var itemStock = playerBag.GetInventoryItem(resourceItem.ItemID);
                // Debug.Log($"Bag Item ID = {itemStock.ItemID}, Bag Item Amout = {itemStock.ItemAmount}");
                if (itemStock.ItemAmount >= resourceItem.ItemAmount)
                {
                    continue;
                }
                else
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 根据位置返回对应背包的数据列表的引用
        /// </summary>
        /// <param name="localtion"></param>
        /// <returns></returns>
        private List<InventoryItem> GetItemList(InventoryLocation location)
        {
            return location switch
            {
                InventoryLocation.Player => playerBag.InventoryItemList,
                InventoryLocation.Box => currentBoxBag.InventoryItemList,
                _ => null
            };
        }
    }
}