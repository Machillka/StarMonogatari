using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Farm.Map;
using Farm.CropPlant;

public class CursorManager : MonoBehaviour
{
    public Sprite Normal, Tool, Seed, Item;
    private Sprite _currentSprite;
    private Image _cursorImage;
    private RectTransform _cursorCanvas;

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
    }

    private void SetCursorInValid()
    {
        _isCursorPositionValid = false;
        _cursorImage.color = new Color(1, 0, 0, 0.4f);
    }

    private void OnItemSelectedEvent(ItemDetails itemInformation, bool isSelected)
    {
        if (!isSelected)
        {
            _currentItem = null;
            isCursorEnabled = false;
            _currentSprite = Normal;
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
            ItemType.Seed => Seed,
            ItemType.Commodity => Item,
            _ => Normal
        };
        isCursorEnabled = true;
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

        // 判断使用范围内
        Vector3Int playerGridPos = currentGrid.WorldToCell(_playerTransform.position);
        if (Mathf.Abs(mouseGridPos.x - playerGridPos.x) > _currentItem.ItemUseRadius || Mathf.Abs(mouseGridPos.y - playerGridPos.y) > _currentItem.ItemUseRadius)
        {
            SetCursorInValid();
            return;
        }


        TileDetails currentTile = GridMapManager.Instance.GetTileDetailsOnMousePosition(mouseGridPos);

        if (currentTile != null)
        {
            CropDetails currentCrop = CropManager.Instance.GetCropDetails(currentTile.seedItemID);
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
