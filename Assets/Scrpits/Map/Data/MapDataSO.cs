using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapDataSO", menuName = "Map/MapData", order = 0)]
public class MapDataSO : ScriptableObject
{
    [SceneName] public string SceneName;
    public List<TileProperty> TileProperties;
}