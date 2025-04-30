using UnityEngine;
using System;

[Serializable]
public class ScheduleDetails : IComparable<ScheduleDetails>
{
    public int hour, minute, day;
    public Seasons season;
    public int priority;                        // 优先级越小越先执行
    public string targetScene;
    public Vector2Int targetGridPosition;
    public AnimationClip stopClip;
    public bool isInteractable;

    public int Time => (hour * 100) + minute;

    public ScheduleDetails(int hour, int minute, int day, Seasons season, int priority, string targetScene, Vector2Int targetGridPosition, AnimationClip stopClip, bool isInteractable)
    {
        this.hour = hour;
        this.minute = minute;
        this.day = day;
        this.season = season;
        this.priority = priority;
        this.targetScene = targetScene;
        this.targetGridPosition = targetGridPosition;
        this.stopClip = stopClip;
        this.isInteractable = isInteractable;
    }

    public int CompareTo(ScheduleDetails other)
    {
        if (Time == other.Time)
        {
            if (priority > other.priority)
                return 1;
            return -1;
        }
        else if (Time > other.Time)
        {
            return 1;
        }
        else if (Time < other.Time)
        {
            return -1;
        }

        return 0;
    }
}
