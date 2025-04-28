using System;
using UnityEngine;
using Farm.CropPlant;

namespace Farm.Inventory
{
    public class Item : MonoBehaviour
    {
        public int ItemID;
        public ItemDetails ItemDetails { get => _itemDetails; }

        private SpriteRenderer _spriteRenderer;
        private BoxCollider2D _collider2D;
        private ItemDetails _itemDetails;


        private void Awake()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _collider2D = GetComponent<BoxCollider2D>();
            // Debug.Log($"In Awake ID is :{ItemID}");
            if (ItemID != 0)
            {
                Init(ItemID);
            }
        }

        // public void Start()
        // {
        //     //NOTE: 认为还是在生成的时候执行一次比较好
        //     if (ItemID != 0)
        //     {
        //         Init(ItemID);
        //     }
        // }

        public void Init(int ID)
        {
            ItemID = ID;
            // Debug.Log($"In Init ID is :{ItemID}");
            _itemDetails = InventoryManager.Instance.GetItemDetails(ItemID);

            if (_itemDetails != null)
            {
                // _spriteRenderer.sprite = _itemDetails.ItemIcon;
                _spriteRenderer.sprite = _itemDetails.ItemOnWorldSprite != null ? _itemDetails.ItemOnWorldSprite : _itemDetails.ItemIcon;

                Vector2 newSize = new Vector2(_spriteRenderer.sprite.bounds.size.x, _spriteRenderer.sprite.bounds.size.y);
                _collider2D.size = newSize;
                _collider2D.offset = new Vector2(0, _spriteRenderer.sprite.bounds.size.y / 2);
            }
            else
            {
                Debug.LogError($"Item with ID {ItemID} not found.");
            }

            if (_itemDetails.ItemType == ItemType.ReapableScenery)
            {
                gameObject.AddComponent<ReapItem>();
                gameObject.GetComponent<ReapItem>().InitCropData(ItemID);
            }
        }
    }
}