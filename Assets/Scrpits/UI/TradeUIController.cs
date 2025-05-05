using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TradeUIController : MonoBehaviour
{
    [Header("UI Component")]
    public Image itemIcon;
    public TextMeshProUGUI itemName;
    public TMP_InputField tradeAmount;
    public Button submitBtn;
    public Button cancelBtn;

    private ItemDetails _currentItem;
    private bool _isSellTrade;

    private void Awake()
    {
        cancelBtn.onClick.AddListener(CancelTrade);
    }

    public void SetupTradeUI(ItemDetails item, bool isSell)
    {
        _currentItem = item;
        itemIcon.sprite = item.ItemIcon;
        itemName.text = item.ItemName;
        _isSellTrade = isSell;
        tradeAmount.text = string.Empty;
    }

    private void CancelTrade()
    {
        this.gameObject.SetActive(false);
    }
}
