using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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

    public static UnityAction<ItemDetails, bool> ItemSelectedEvent;
    public static void CallItemSelectedEvent(ItemDetails item, bool isSelected)
    {
        ItemSelectedEvent?.Invoke(item, isSelected);
    }

    public static UnityAction<int, int> GameMinuteEvent;
    public static void CallGameMinuteChangeEvent(int minute, int hour)
    {
        GameMinuteEvent?.Invoke(minute, hour);
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
}
