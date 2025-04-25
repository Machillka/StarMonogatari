using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Farm.Inventory
{
    [RequireComponent(typeof(SlotUiController))]
    public class ShowItemToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private SlotUiController _slotUI;
        private InventoryUIController _inventoryUI => GetComponentInParent<InventoryUIController>();

        private bool _isHovering;
        private Coroutine _hoverCoroutine;

        private void Awake()
        {
            _slotUI = GetComponent<SlotUiController>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovering = true;
            _hoverCoroutine = StartCoroutine(HoverCheck());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _inventoryUI.ItemToolTip.gameObject.SetActive(false);

            // 如果执行了检测携程, 在鼠标移出物体之后停止携程
            if (_hoverCoroutine != null)
            {
                StopCoroutine(_hoverCoroutine);
            }
        }

        private IEnumerator HoverCheck()
        {
            yield return new WaitForSeconds(Settings.ItemTipShowUpTimeOffset);
            // 延迟了这么长时间后 如果依旧在 hovering 则使用方法
            if (_isHovering)
            {
                if (_slotUI.SlotItem != null)
                {
                    _inventoryUI.ItemToolTip.gameObject.SetActive(true);
                    _inventoryUI.ItemToolTip.SetupToolTip(_slotUI.SlotItem, _slotUI.SlotType);

                    // 设置显示位置
                    _inventoryUI.ItemToolTip.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
                    _inventoryUI.ItemToolTip.transform.position = transform.position + Vector3.up * Settings.ToolTipOffset;
                }
            }
        }
    }
}