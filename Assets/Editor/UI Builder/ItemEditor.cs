using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemEditor : EditorWindow
{
    private ItemDataListSO dataBase;
    private List<ItemDetails> itemDetailsList = new List<ItemDetails>();
    private VisualTreeAsset itemRowTemplate;
    private ListView itemListView;
    private ScrollView itemDetailsSection;
    private ItemDetails activeItem;

    private Sprite defaultIcon;
    private VisualElement iconPreview;

    // [SerializeField]
    // private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("MachillkaEditor/ItemEditor")]
    public static void ShowExample()
    {
        ItemEditor wnd = GetWindow<ItemEditor>();
        wnd.titleContent = new GUIContent("ItemEditor");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI Builder/ItemEditor.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        // 获得item排列模板
        itemRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI Builder/ItemRowTemplate.uxml");

        defaultIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Art/Items/Icons/icon_Game.png");

        itemListView = root.Q<VisualElement>("ItemList").Q<ListView>("ListView");
        itemDetailsSection = root.Q<ScrollView>("ItemDetails");
        iconPreview = itemDetailsSection.Q<VisualElement>("Icon");

        root.Q<Button>("AddItemButton").clicked += OnAddItemButtonClicked;
        root.Q<Button>("DeleteButton").clicked += OnDeleteButtonClicked;

        LoadDatabase();
        GenerateListView();
    }

    private void OnDeleteButtonClicked()
    {
        // var activeItem = itemListView.selectedItem as ItemDetails;
        // if (activeItem != null)
        // {
        //     itemDetailsList.Remove(activeItem);
        // }
        itemDetailsList.Remove(activeItem);
        itemListView.Rebuild();
        itemDetailsSection.visible = false;
    }

    private void OnAddItemButtonClicked()
    {
        ItemDetails newItem = new ItemDetails();
        newItem.ItemID = itemDetailsList.Count + 1001;
        newItem.ItemName = "New Item";
        itemDetailsList.Add(newItem);
        itemListView.Rebuild();
    }

    private void LoadDatabase()
    {
        // AssetDatabase.LoadAssetAtPath<ItemDataListSO>("Assets/Scrpits/Inventory/Data SO/ItemDataListSO.asset");
        var dataArray = AssetDatabase.FindAssets("ItemDataListSO");

        if (dataArray.Length > 1)
        {
            var path = AssetDatabase.GUIDToAssetPath(dataArray[0]);
            dataBase = AssetDatabase.LoadAssetAtPath(path, typeof(ItemDataListSO)) as ItemDataListSO;
        }

        itemDetailsList = dataBase.ItemDetailList;

        // NOTE: 不标记则无法保存数据
        EditorUtility.SetDirty(dataBase);
    }

    private void GenerateListView()
    {
        Func<VisualElement> makeItem = () => itemRowTemplate.CloneTree();

        Action<VisualElement, int> bindItem = (e, i) =>
        {
            if (i < itemDetailsList.Count)
            {
                if (itemDetailsList[i].ItemIcon != null)
                    e.Q<VisualElement>("Icon").style.backgroundImage = new StyleBackground(itemDetailsList[i].ItemIcon);
                e.Q<Label>("Name").text = itemDetailsList[i] == null ? "Undefined Item" : itemDetailsList[i].ItemName;
            }
        };
        itemListView.fixedItemHeight = 50;
        itemListView.itemsSource = itemDetailsList;
        itemListView.makeItem = makeItem;
        itemListView.bindItem = bindItem;

        itemListView.selectionChanged += OnListSelectionChange;

        itemDetailsSection.visible = false;
    }

    private void OnListSelectionChange(IEnumerable<object> selectedItem)
    {
        activeItem = (ItemDetails)selectedItem.First();
        GetItemDetails();
        itemDetailsSection.visible = true;
    }

    private void GetItemDetails()
    {
        itemDetailsSection.MarkDirtyRepaint();

        itemDetailsSection.Q<IntegerField>("ItemID").value = activeItem.ItemID;
        itemDetailsSection.Q<IntegerField>("ItemID").RegisterValueChangedCallback(evt =>
        {
            activeItem.ItemID = evt.newValue;
        });

        itemDetailsSection.Q<TextField>("ItemName").value = activeItem.ItemName;
        itemDetailsSection.Q<TextField>("ItemName").RegisterValueChangedCallback(evt =>
        {
            activeItem.ItemName = evt.newValue;
            itemListView.Rebuild();
        });

        iconPreview.style.backgroundImage = activeItem.ItemIcon == null ? defaultIcon.texture : activeItem.ItemIcon.texture;
        itemDetailsSection.Q<ObjectField>("ItemIcon").value = activeItem.ItemIcon;
        itemDetailsSection.Q<ObjectField>("ItemIcon").RegisterValueChangedCallback(evt =>
        {
            Sprite newIcon = evt.newValue as Sprite;
            activeItem.ItemIcon = newIcon;

            iconPreview.style.backgroundImage = newIcon == null ? defaultIcon.texture : newIcon.texture;
            itemListView.Rebuild();
        });

        //其他所有变量的绑定
        itemDetailsSection.Q<ObjectField>("ItemSprite").value = activeItem.ItemOnWorldSprite;
        itemDetailsSection.Q<ObjectField>("ItemSprite").RegisterValueChangedCallback(evt =>
        {
            activeItem.ItemOnWorldSprite = (Sprite)evt.newValue;
        });

        itemDetailsSection.Q<EnumField>("ItemType").Init(activeItem.ItemType);
        itemDetailsSection.Q<EnumField>("ItemType").value = activeItem.ItemType;
        itemDetailsSection.Q<EnumField>("ItemType").RegisterValueChangedCallback(evt =>
        {
            activeItem.ItemType = (ItemType)evt.newValue;
        });

        itemDetailsSection.Q<TextField>("Description").value = activeItem.ItemDescription;
        itemDetailsSection.Q<TextField>("Description").RegisterValueChangedCallback(evt =>
        {
            activeItem.ItemDescription = evt.newValue;
        });

        itemDetailsSection.Q<IntegerField>("ItemUseRadius").value = activeItem.ItemUseRadius;
        itemDetailsSection.Q<IntegerField>("ItemUseRadius").RegisterValueChangedCallback(evt =>
        {
            activeItem.ItemUseRadius = evt.newValue;
        });

        itemDetailsSection.Q<Toggle>("CanPickedup").value = activeItem.CanPickedup;
        itemDetailsSection.Q<Toggle>("CanPickedup").RegisterValueChangedCallback(evt =>
        {
            activeItem.CanPickedup = evt.newValue;
        });

        itemDetailsSection.Q<Toggle>("CanDropped").value = activeItem.CanDropped;
        itemDetailsSection.Q<Toggle>("CanDropped").RegisterValueChangedCallback(evt =>
        {
            activeItem.CanDropped = evt.newValue;
        });

        itemDetailsSection.Q<Toggle>("CanCarried").value = activeItem.CanCarried;
        itemDetailsSection.Q<Toggle>("CanCarried").RegisterValueChangedCallback(evt =>
        {
            activeItem.CanCarried = evt.newValue;
        });

        itemDetailsSection.Q<IntegerField>("Price").value = activeItem.ItemPrice;
        itemDetailsSection.Q<IntegerField>("Price").RegisterValueChangedCallback(evt =>
        {
            activeItem.ItemPrice = evt.newValue;
        });

        itemDetailsSection.Q<Slider>("SellPercentage").value = activeItem.SellPercentage;
        itemDetailsSection.Q<Slider>("SellPercentage").RegisterValueChangedCallback(evt =>
        {
            activeItem.SellPercentage = evt.newValue;
        });
    }
}
