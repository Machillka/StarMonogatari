using UnityEngine;


namespace Farm.Inventory
{
    public class BoxController : MonoBehaviour
    {
        public InventoryBagSO boxBagTemplate;
        public InventoryBagSO boxBagData;

        public GameObject mouseIconSign;

        private bool _canOpen = false;
        private bool _isOpening;

        private void OnEnable()
        {
            if (boxBagData == null)
            {
                boxBagData = Instantiate(boxBagTemplate);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _canOpen = true;
                mouseIconSign.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _canOpen = false;
                mouseIconSign.SetActive(false);
            }
        }

        private void Update()
        {
            if (!_isOpening && _canOpen && InputManager.Instance.IsOpenBagButtonPressed)
            {
                EventHandler.CallBaseBagOpenEvent(SlotTypes.Box, boxBagData);
                _isOpening = true;
            }

            if ((!_canOpen && _isOpening) || (_isOpening && InputManager.Instance.IsExitShopButtonPressed))
            {
                EventHandler.CallBaseBagCloseEvent(SlotTypes.Box, boxBagData);
                _isOpening = false;
            }
        }
    }
}

