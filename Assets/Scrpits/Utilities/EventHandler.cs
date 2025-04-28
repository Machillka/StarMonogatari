using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Video;
public class EventHandler
{
    public static event Action<InventoryLocation, List<InventoryItem>> UpdateInventoryUI;

    public static void CallUpdateInventoryUI(InventoryLocation location, List<InventoryItem> items)
    {
        UpdateInventoryUI?.Invoke(location, items);
    }

    public static event Action<int, Vector3> InstantiateItemInScene;
    public static void CallInstantiateItemInScene(int itemID, Vector3 pos)
    {
        InstantiateItemInScene?.Invoke(itemID, pos);
    }

    public static event Action<int, Vector3, ItemType> DropItemInScene;
    public static void CallDropItemInScene(int itemID, Vector3 pos, ItemType itemType)
    {
        DropItemInScene?.Invoke(itemID, pos, itemType);
    }

    public static UnityAction<ItemDetails, bool> ItemSelectedEvent;
    public static void CallItemSelectedEvent(ItemDetails item, bool isSelected)
    {
        // Debug.Log($"ItemSelectedEvent, item = {item.ItemName}, isSelected = {isSelected}");
        ItemSelectedEvent?.Invoke(item, isSelected);
    }

    public static UnityAction<int, int> GameMinuteEvent;
    public static void CallGameMinuteChangeEvent(int minute, int hour)
    {
        GameMinuteEvent?.Invoke(minute, hour);
    }

    public static UnityAction<int, Seasons> GameDayEvent;
    public static void CallGameDayChangeEvent(int day, Seasons season)
    {
        GameDayEvent?.Invoke(day, season);
        // Debug.Log("day change");
    }

    public static event Action<int, int, int, int, Seasons> GameDateEvent;//TODO: 观察者模式，对于每一个时间单位都写一个事件
    public static void CallDataChangeEvent(int hour, int day, int month, int year, Seasons season)
    {
        GameDateEvent?.Invoke(hour, day, month, year, season);
    }

    public static UnityAction<string, Vector3> TransitionEvent;
    public static void CallTransitionEvent(string sceneToLoadName, Vector3 targetPosition)
    {
        TransitionEvent?.Invoke(sceneToLoadName, targetPosition);
    }

    public static UnityAction BeforeSceneLoadedEvent;
    public static void CallBeforeSceneLoadedEvent()
    {
        BeforeSceneLoadedEvent?.Invoke();
    }

    public static UnityAction AfterSceneLoadedEvent;
    public static void CallAfterSceneLoadedEvent()
    {
        AfterSceneLoadedEvent?.Invoke();
    }

    public static UnityAction<Vector3> MoveToPosition;
    public static void CallMoveToPosition(Vector3 targetPosition)
    {
        MoveToPosition?.Invoke(targetPosition);
    }

    public static UnityAction<Vector3, ItemDetails> MouseClickEvent;
    public static void CallMouseClickEvent(Vector3 targetPosition, ItemDetails item)
    {
        MouseClickEvent?.Invoke(targetPosition, item);
    }

    public static UnityAction<Vector3, ItemDetails> ExcuteActionAfterAnimation;
    public static void CallExcuteActionAfterAnimation(Vector3 targetPosition, ItemDetails item)
    {
        ExcuteActionAfterAnimation?.Invoke(targetPosition, item);
    }

    public static UnityAction<int, TileDetails> PlantSeedEvent;
    public static void CallPlantSeedEvent(int seedID, TileDetails tile)
    {
        PlantSeedEvent?.Invoke(seedID, tile);
    }

    public static UnityAction<int> HarvestAtPlaterPositionEvent;
    public static void CallHarvestAtPlaterPositionEvent(int itemID)
    {
        HarvestAtPlaterPositionEvent?.Invoke(itemID);
    }

    public static UnityAction RefreshCurrentMapEvent;
    public static void CallRefreshCurrentMapEvent()
    {
        RefreshCurrentMapEvent?.Invoke();
    }

    public static event Action<InputAction.CallbackContext> SelectSlotEvent;
    public static void CallOnSelectSlotEvent(InputAction.CallbackContext context)
    {
        SelectSlotEvent?.Invoke(context);
    }

    public static event Action<ParticalEffetcTypes, Vector3> ParticalEffectEvent;
    public static void CallParticalEffectEvent(ParticalEffetcTypes type, Vector3 pos)
    {
        ParticalEffectEvent?.Invoke(type, pos);
    }

    public static UnityAction GenerateCropEvent;
    public static void CallGenerateCropEvent()
    {
        GenerateCropEvent?.Invoke();
    }
}
