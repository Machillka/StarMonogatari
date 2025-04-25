using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Farm.Inventory
{
    public class ItemManager : MonoBehaviour
    {
        public Item ItemPrefab;
        private Transform _itemParentTransform;

        private Dictionary<string, List<SceneItem>> _sceneItemDictionary = new Dictionary<string, List<SceneItem>>();

        private void OnEnable()
        {
            EventHandler.InstantiateItemInScene += OnInstantiateItemInScene;
            EventHandler.DropItemInScene += OnDropItemInScene;
            EventHandler.BeforeSceneLoadedEvent += OnBeforeSceneLoadedEvent;
            EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        }

        private void OnDisable()
        {
            EventHandler.InstantiateItemInScene -= OnInstantiateItemInScene;
            EventHandler.DropItemInScene -= OnDropItemInScene;
            EventHandler.BeforeSceneLoadedEvent -= OnBeforeSceneLoadedEvent;
            EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        }

        private void OnBeforeSceneLoadedEvent()
        {
            GetAllSceneItems();
        }

        private void OnAfterSceneLoadedEvent()
        {
            _itemParentTransform = GameObject.FindWithTag("ItemParent").transform;
            RecreateAllItems();
        }

        private void OnInstantiateItemInScene(int itemID, Vector3 pos)
        {
            var item = Instantiate(ItemPrefab, pos, Quaternion.identity, _itemParentTransform);
            item.ItemID = itemID;
        }

        private void OnDropItemInScene(int itemID, Vector3 pos)
        {
            // TODO: 处理实际效果
            var item = Instantiate(ItemPrefab, pos, Quaternion.identity, _itemParentTransform);
            item.ItemID = itemID;
        }

        /// <summary>
        /// 获得场景中所有物体
        /// </summary>
        private void GetAllSceneItems()
        {
            List<SceneItem> currentSceneItems = new List<SceneItem>();

            foreach (var item in FindObjectsByType<Item>(FindObjectsSortMode.None))
            {
                SceneItem sceneItem = new()
                {
                    ItemID = item.ItemID,
                    Position = new SerializableVector3(item.transform.position)
                };

                currentSceneItems.Add(sceneItem);
            }

            if (_sceneItemDictionary.ContainsKey(SceneManager.GetActiveScene().name))
            {
                // 找到就更新
                _sceneItemDictionary[SceneManager.GetActiveScene().name] = currentSceneItems;
            }
            else
            {
                // 没找到就添加
                _sceneItemDictionary.Add(SceneManager.GetActiveScene().name, currentSceneItems);
            }
        }

        /// <summary>
        /// 刷新重建当前场景物品
        /// </summary>
        private void RecreateAllItems()
        {
            List<SceneItem> currentSceneItems = new List<SceneItem>();

            if (_sceneItemDictionary.TryGetValue(SceneManager.GetActiveScene().name, out currentSceneItems))
            {
                if (currentSceneItems != null)
                {
                    foreach (var item in FindObjectsByType<Item>(FindObjectsSortMode.InstanceID))
                    {
                        item.transform.SetParent(null); // 解除父子关系，避免引用问题
                        Destroy(item.gameObject); // 确保物体被销毁
                    }
                    foreach (var item in currentSceneItems)
                    {
                        var newItem = Instantiate(ItemPrefab, item.Position.ToVector3(), Quaternion.identity, _itemParentTransform);
                        newItem.Init(item.ItemID);
                    }
                }

            }
        }
    }
}
