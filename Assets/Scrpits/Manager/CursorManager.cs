using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Farm.Map;
using Farm.CropPlant;
using Unity.VisualScripting;
using System.Runtime.InteropServices;
using Farm.Inventory;

public class CursorManager : MonoBehaviour
{
    public Sprite Normal, Tool, Seed, Item;
    private Sprite _currentSprite;
    private Image _cursorImage;
    private RectTransform _cursorCanvas;

    private Image _buildImage;

    // 鼠标检测
    private Camera mainCamera;
    private Grid currentGrid;

    private Vector3 mouseWorldPos;
    private Vector3Int mouseGridPos;

    private bool isCursorEnabled;
    private bool _isCursorPositionValid;

    private ItemDetails _currentItem;

    private Transform _playerTransform => FindAnyObjectByType<PlayerMovement>().transform;

    private void Start()
    {
        _cursorCanvas = GameObject.FindWithTag("CursorCanvas").GetComponent<RectTransform>();
        _cursorImage = _cursorCanvas.GetChild(0).GetComponent<Image>();
        _buildImage = _cursorCanvas.GetChild(1).GetComponent<Image>();
        _buildImage.gameObject.SetActive(false);

        _currentSprite = Normal;
        SetCursorImage(_currentSprite);

        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (_cursorCanvas == null)
            return;
        _cursorImage.transform.position = Input.mousePosition;

        if (!IsInteractWithUI() && isCursorEnabled)
        {
            SetCursorImage(_currentSprite);
            CheckCursorValid();
            CheckPlayerInput();
        }
        else
        {
            SetCursorImage(Normal);
            _buildImage.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.BeforeSceneLoadedEvent += OnBeforeSceneLoadedEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.BeforeSceneLoadedEvent -= OnBeforeSceneLoadedEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }

    #region 设置鼠标样式

    private void SetCursorImage(Sprite sprite)
    {
        _cursorImage.sprite = sprite;
        _cursorImage.color = new Color(1, 1, 1, 1);
    }

    private void SetCursorValid()
    {
        _isCursorPositionValid = true;
        _cursorImage.color = new Color(1, 1, 1, 1);
        _buildImage.color = new Color(1, 1, 1, 0.5f);
    }

    private void SetCursorInValid()
    {
        _isCursorPositionValid = false;
        _cursorImage.color = new Color(1, 0, 0, 0.4f);
        _buildImage.color = new Color(1, 0, 0, 0.5f);
    }

    private void OnItemSelectedEvent(ItemDetails itemInformation, bool isSelected)
    {
        if (!isSelected)
        {
            _currentItem = null;
            isCursorEnabled = false;
            _currentSprite = Normal;
            _buildImage.gameObject.SetActive(false);
            return;
        }

        _currentItem = itemInformation;
        //WORKFLOW : 根据物品类型来设置鼠标样式
        _currentSprite = itemInformation.ItemType switch
        {
            ItemType.ChopTool => Tool,
            ItemType.HoeTool => Tool,
            ItemType.WaterTool => Tool,
            ItemType.CollectTool => Tool,
            ItemType.BreakTool => Tool,
            ItemType.ReapTool => Tool,
            ItemType.Seed => Seed,
            ItemType.Commodity => Item,
            _ => Normal
        };
        isCursorEnabled = true;

        if (itemInformation.ItemType == ItemType.Furniture)
        {
            Debug.Log("Select Build Image");
            _buildImage.gameObject.SetActive(true);
            _buildImage.sprite = itemInformation.ItemOnWorldSprite;
            _buildImage.SetNativeSize();
        }
    }

    #endregion

    private void CheckPlayerInput()
    {
        if (InputManager.Instance.IsLeftMouseButtonPressed && _isCursorPositionValid)
        {
            // 执行对应方法
            EventHandler.CallMouseClickEvent(mouseWorldPos, _currentItem);
        }
    }

    private void CheckCursorValid()
    {
        mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
        // Debug.Log(mouseWorldPos);
        mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);

        // Build Image followes mouse position
        _buildImage.rectTransform.position = Input.mousePosition;

        // 判断使用范围内
        Vector3Int playerGridPos = currentGrid.WorldToCell(_playerTransform.position);
        if (Mathf.Abs(mouseGridPos.x - playerGridPos.x) > _currentItem.ItemUseRadius || Mathf.Abs(mouseGridPos.y - playerGridPos.y) > _currentItem.ItemUseRadius)
        {
            SetCursorInValid();
            return;
        }

        TileDetails currentTile = GridMapManager.Instance.GetTileDetailsOnMousePosition(mouseGridPos);
        // Debug.Log($"CurrentTileInformation:{currentTile}");
        //TODO: 比如杂草等不应只能在有 "Tile" 的地方使用 -> //FIXME修改属性
        if (currentTile != null)
        {
            CropDetails currentCrop = CropManager.Instance.GetCropDetails(currentTile.seedItemID);
            Crop crop = GridMapManager.Instance.GetCropObject(mouseWorldPos);
            // Debug.Log($"Switching ItemType; itemtype = {_currentItem.ItemType}");
            //WORKFLOW 补齐所有物品判断
            switch (_currentItem.ItemType)
            {
                case ItemType.Seed:
                    if (currentTile.daySinceDug > -1 && currentTile.seedItemID == -1)
                        SetCursorValid();
                    else
                        SetCursorInValid();
                    break;
                case ItemType.Commodity:
                    if (currentTile.CanDropItem && _currentItem.CanDropped)
                        SetCursorValid();
                    else
                        SetCursorInValid();
                    break;
                case ItemType.HoeTool:
                    if (currentTile.CanDig)
                        SetCursorValid();
                    else
                        SetCursorInValid();
                    break;
                case ItemType.WaterTool:
                    if (currentTile.daySinceDug > -1 && currentTile.daySinceWatered == -1)
                        SetCursorValid();
                    else
                        SetCursorInValid();
                    break;
                case ItemType.CollectTool:
                    if (currentCrop != null && currentTile.growthDays >= currentCrop.TotalGrouthDays && currentCrop.CheckToolAvaliable(_currentItem.ItemID))
                        SetCursorValid();
                    else
                        SetCursorInValid();
                    break;
                case ItemType.BreakTool:
                case ItemType.ChopTool: //FIXME: 树的判断有问题
                    if (crop != null)
                    {
                        // Debug.Log($"GrouthTotalDays = {crop.CropInformation.TotalGrouthDays}; GrouthDays = {crop.tileDetails.growthDays}");
                        if (currentCrop != null && crop.CanHarvest && crop.CropInformation.CheckToolAvaliable(_currentItem.ItemID))
                            SetCursorValid();
                        else
                            SetCursorInValid();
                    }
                    else
                        SetCursorInValid();
                    break;
                case ItemType.ReapTool:
                    if (GridMapManager.Instance.HaveReapableItemsInRadius(mouseWorldPos, _currentItem))
                        SetCursorValid();
                    else
                        SetCursorInValid();
                    break;
                case ItemType.Furniture:
                    _buildImage.gameObject.SetActive(true);
                    // Debug.Log($"CanPlace Furniture  {currentTile.CanPlaceFurniture}");
                    // Debug.Log($"Checking Stock  {InventoryManager.Instance.CheckStock(_currentItem.ItemID)}");
                    if (currentTile.CanPlaceFurniture && InventoryManager.Instance.CheckStock(_currentItem.ItemID))
                        SetCursorValid();
                    else
                        SetCursorInValid();
                    break;
            }
        }
        else
        {
            SetCursorInValid();
        }
    }

    private bool IsInteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        return false;
    }

    private void OnAfterSceneLoadedEvent()
    {
        currentGrid = FindAnyObjectByType<Grid>();
        // isCursorEnabled = true;
    }

    private void OnBeforeSceneLoadedEvent()
    {
        isCursorEnabled = false;
    }
}
