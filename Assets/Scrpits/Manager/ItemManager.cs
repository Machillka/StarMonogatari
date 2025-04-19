using UnityEngine;

namespace Farm.Inventory
{
    public class ItemManager : MonoBehaviour
    {
        public Item ItemPrefab;
        private Transform _itemParentTransform;

        // private void Start()
        // {
        //     _itemParentTransform = GameObject.FindWithTag("ItemParent").transform;
        // }

        private void OnEnable()
        {
            EventHandler.InstantiateItemInScene += OnInstantiateItemInScene;
            EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        }

        private void OnDisable()
        {
            EventHandler.InstantiateItemInScene -= OnInstantiateItemInScene;
            EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        }

        private void OnAfterSceneLoadedEvent()
        {
            _itemParentTransform = GameObject.FindWithTag("ItemParent").transform;
        }

        private void OnInstantiateItemInScene(int itemID, Vector3 pos)
        {
            var item = Instantiate(ItemPrefab, pos, Quaternion.identity, _itemParentTransform);
            item.ItemID = itemID;
        }
    }
}
