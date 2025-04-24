using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    private Vector3 mouseGridPos;

    private bool isCursorEnabled;

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

    private void SetCursorImage(Sprite sprite)
    {
        _cursorImage.sprite = sprite;
        _cursorImage.color = new Color(1, 1, 1, 1);
    }

    private void OnItemSelectedEvent(ItemDetails itemInformation, bool isSelected)
    {
        if (!isSelected)
        {
            _currentSprite = Normal;
            return;
        }

        _currentSprite = itemInformation.ItemType switch
        {
            ItemType.ChopTool => Tool,
            ItemType.Seed => Seed,
            ItemType.Commodity => Item,
            _ => Normal
        };
    }

    private void CheckCursorValid()
    {
        mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);

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
        isCursorEnabled = true;
    }

    private void OnBeforeSceneLoadedEvent()
    {
        isCursorEnabled = false;
    }
}
