using UnityEngine;

public class NPCFunction : MonoBehaviour
{
    public InventoryBagSO shopData;
    private bool _isOpen;

    private void Update()
    {
        if (_isOpen && InputManager.Instance.IsExitShopButtonPressed)
        {
            // 关闭背包
            CloseShop();
        }
    }

    public void OpenShop()
    {
        _isOpen = true;
        EventHandler.CallBaseBagOpenEvent(SlotTypes.Shop, shopData);
        EventHandler.CallUpdateGameStateEvent(GameStates.Pause);
    }

    public void CloseShop()
    {
        _isOpen = false;
        EventHandler.CallBaseBagCloseEvent(SlotTypes.Shop, shopData);
        EventHandler.CallUpdateGameStateEvent(GameStates.GamePlay);
    }
}
