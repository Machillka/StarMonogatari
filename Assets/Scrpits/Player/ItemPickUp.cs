using UnityEngine;

namespace Farm.Inventory
{
    public class ItemPickUp : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Item item = collision.GetComponent<Item>();
            if (item != null)
            {
                if (item.ItemDetails != null)
                {
                    InventoryManager.Instance.AddItem(item, true);
                }
            }
        }
    }

}

