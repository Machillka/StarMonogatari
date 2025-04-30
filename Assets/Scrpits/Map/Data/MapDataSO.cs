using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapDataSO", menuName = "Map/MapData", order = 0)]
public class MapDataSO : ScriptableObject
{
    [SceneName] public string SceneName;
    public List<TileProperty> TileProperties;

    public int gridWidth;
    public int gridHeight;

    public int originX;                                 // 左下角原点
    public int originY;
}