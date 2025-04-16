using System;
using UnityEngine;

namespace Farm.Inventory
{
    public class Item : MonoBehaviour
    {
        public int ItemID;
        public ItemDetails ItemDetails { get => _itemDetails;}

        private SpriteRenderer _spriteRenderer;
        private BoxCollider2D _collider2D;
        private ItemDetails _itemDetails;


        private void Awake()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _collider2D = GetComponent<BoxCollider2D>();
        }

        private void Start()
        {
            if (ItemID != 0)
            {
                Init(ItemID);
            }
        }

        public void Init(int ID)
        {
            ItemID = ID;
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
        }
    }
}