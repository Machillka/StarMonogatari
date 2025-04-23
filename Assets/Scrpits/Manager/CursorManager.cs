using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CursorManager : MonoBehaviour
{
    public Sprite Normal, Tool, Seed, Item;
    private Sprite _currentSprite;
    private Image _cursorImage;
    private RectTransform _cursorCanvas;

    private void Start()
    {
        _cursorCanvas = GameObject.FindWithTag("CursorCanvas").GetComponent<RectTransform>();
        _cursorImage = _cursorCanvas.GetChild(0).GetComponent<Image>();

        _currentSprite = Normal;
        SetCursorImage(_currentSprite);
    }

    private void Update()
    {
        if (_cursorCanvas == null)
            return;
        _cursorImage.transform.position = Input.mousePosition;

        if (!IsInteractWithUI())
        {
            SetCursorImage(_currentSprite);
        }
        else
        {
            SetCursorImage(Normal);
        }
    }

    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
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

    private bool IsInteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        return false;
    }
}
