using System.Collections.Generic;
using UnityEngine;

namespace Farm.Map
{
    public class GridMapManager : MonoBehaviour
    {
        [Header("Grid Information")]
        public List<MapDataSO> MapDataList;

        private Dictionary<string, TileDetails> itemDetailsDict = new Dictionary<string, TileDetails>();

        private void Start()
        {
            //TODO: 利用addressable加载地图数据 或者其他动态加载的方式
            foreach (var mapInformation in MapDataList)
            {
                InitTileDetailDictionary(mapInformation);
            }
        }

        private void InitTileDetailDictionary(MapDataSO mapInformation)
        {
            foreach (TileProperty tileProperty in mapInformation.TileProperties)
            {
                TileDetails tileDetail = new TileDetails
                {
                    GridX = tileProperty.TileCoordinate.x,
                    GridY = tileProperty.TileCoordinate.y,
                };

                string key = tileProperty.TileCoordinate.x + "x" + tileProperty.TileCoordinate.y + "y" + mapInformation.SceneName;

                if (GetTileDetails(key) != null)
                {
                    tileDetail = GetTileDetails(key);
                }

                switch (tileProperty.TileType)
                {
                    case GridTypes.Diggable:
                        tileDetail.CanDig = tileProperty.BoolTypeValue;
                        break;
                    case GridTypes.DropItem:
                        tileDetail.CanDropItem = tileProperty.BoolTypeValue;
                        break;
                    case GridTypes.PlaceFurniture:
                        tileDetail.CanPlaceFurniture = tileProperty.BoolTypeValue;
                        break;
                    case GridTypes.NPCObstacle:
                        tileDetail.IsNPCObstacle = tileProperty.BoolTypeValue;
                        break;
                }

                if (GetTileDetails(key) != null)
                {
                    itemDetailsDict[key] = tileDetail;
                }
                else
                {
                    itemDetailsDict.Add(key, tileDetail);
                }
            }
        }

        private TileDetails GetTileDetails(string key)
        {
            if (itemDetailsDict.ContainsKey(key))
            {
                return itemDetailsDict[key];
            }
            return null;
        }
    }
}

