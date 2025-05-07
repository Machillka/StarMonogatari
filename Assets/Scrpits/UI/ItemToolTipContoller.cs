using Farm.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemToolTipContoller : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _typeText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _valueText;
    [SerializeField] private GameObject bottomPart;

    [Header("Building")]
    [SerializeField] private GameObject resourcePanel;
    [SerializeField] private Image[] resourceItem;

    public GameObject ResourcePanel => resourcePanel;

    //TODO: 设置鼠标划入一段时间后才显示 tip 面板

    public void SetupToolTip(ItemDetails itemInformation, SlotTypes slotType)
    {
        _nameText.text = itemInformation.ItemName;
        _typeText.text = itemInformation.ItemType.ToString();
        _descriptionText.text = itemInformation.ItemDescription;

        if (itemInformation.ItemType == ItemType.Seed || itemInformation.ItemType == ItemType.Commodity || itemInformation.ItemType == ItemType.Furniture)
        {
            bottomPart.SetActive(true);

            var price = itemInformation.ItemPrice;

            if (slotType == SlotTypes.Bag)
            {
                price = (int)(price * itemInformation.SellPercentage);
            }

            _valueText.text = price.ToString();
        }
        else
        {
            bottomPart.SetActive(false);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public void SetupResourcePanel(int itemID)
    {
        BluePrintDetails bluePrint = InventoryManager.Instance.bluePrintData.GetBluePrintDetails(itemID);
        for (int i = 0; i < resourceItem.Length; i++)
        {
            if (i < bluePrint.resourceItems.Length)
            {
                InventoryItem item = bluePrint.resourceItems[i];
                resourceItem[i].gameObject.SetActive(true);
                resourceItem[i].sprite = InventoryManager.Instance.GetItemDetails(item.ItemID).ItemIcon;
                resourceItem[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.ItemAmount.ToString();
            }
            else
            {
                resourceItem[i].gameObject.SetActive(false);
            }
        }
    }
}
