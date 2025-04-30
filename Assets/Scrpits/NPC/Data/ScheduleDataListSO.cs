using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ScheduleDataListSO", menuName = "NPC Schdule/ScheduleDataListSO", order = 0)]
public class ScheduleDataListSO : ScriptableObject
{
    public List<ScheduleDetails> schduleList;
}
