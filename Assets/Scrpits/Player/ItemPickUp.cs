using UnityEngine;

namespace Farm.Inventory
{
    public class ItemPickUp : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Debug.Log("Trigger");
            //BUG[x]: 新生成的物品不会被拾取
            Item item = collision.GetComponent<Item>();

            if (item == null)
                return;

            if (item.ItemDetails != null)
            {
                InventoryManager.Instance.AddItem(item, true);
            }
        }
    }

}

