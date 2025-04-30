using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapDataSO", menuName = "Map/MapData", order = 0)]
public class MapDataSO : ScriptableObject
{
    [SceneName] public string SceneName;
    public List<TileProperty> TileProperties;

    //TODO 尝试使用代码自动获取这些变量
    //WORKFLOW 创建新地图填充以下字段
    public int gridWidth;
    public int gridHeight;

    public int originX;                                 // 左下角原点
    public int originY;
}