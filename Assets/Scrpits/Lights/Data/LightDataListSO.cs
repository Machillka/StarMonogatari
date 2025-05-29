using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LightDataListSO", menuName = "Light/Patten")]
public class LightDataListSO : ScriptableObject
{
    public List<LightDetails> lightDetailsList;
    public LightDetails GetLightDetails(Seasons season, LightShifts lightShift)
    {
        return lightDetailsList.Find(patten => patten.season == season && patten.lightShift == lightShift);
    }
}

[System.Serializable]
public class LightDetails
{
    public Seasons season;
    public LightShifts lightShift;
    public Color color;
    public float lightAmount;
}
