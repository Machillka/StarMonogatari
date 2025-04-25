using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace Farm.Map
{
    public class GridMapManager : Singleton<GridMapManager>
    {
        [Header("Grid Information")]
        public List<MapDataSO> MapDataList;
        private Dictionary<string, TileDetails> itemDetailsDict = new Dictionary<string, TileDetails>();
        private Grid _currentGrid;

        [Header("Grid Transition Information")]
        public RuleTile WaterTile;
        public RuleTile DigTile;
        private Tilemap _digTileMap;
        private Tilemap _waterTileMap;

        private void Start()
        {
            //TODO: 利用addressable加载地图数据 或者其他动态加载的方式
            foreach (var mapInformation in MapDataList)
            {
                InitTileDetailDictionary(mapInformation);
            }
        }

        private void OnEnable()
        {
            EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
            EventHandler.ExcuteActionAfterAnimation += OnExcuteActionAfterAnimation;
        }

        private void OnDisable()
        {
            EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
            EventHandler.ExcuteActionAfterAnimation -= OnExcuteActionAfterAnimation;
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

        /// <summary>
        /// 根据鼠标的网格坐标返回瓦片信息
        /// </summary>
        /// <param name="mouseGridPosition"></param>
        /// <returns></returns>
        public TileDetails GetTileDetailsOnMousePosition(Vector3Int mouseGridPosition)
        {
            string key = mouseGridPosition.x + "x" + mouseGridPosition.y + "y" + SceneManager.GetActiveScene().name;
            if (itemDetailsDict.ContainsKey(key))
            {
                return itemDetailsDict[key];
            }
            return null;
        }

        private void OnAfterSceneLoadedEvent()
        {
            _currentGrid = FindAnyObjectByType<Grid>();
            _digTileMap = GameObject.FindWithTag("Dig").GetComponent<Tilemap>();
            _waterTileMap = GameObject.FindWithTag("Water").GetComponent<Tilemap>();
        }

        /// <summary>
        /// 执行实际效果
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <param name="item"></param>
        private void OnExcuteActionAfterAnimation(Vector3 mousePosition, ItemDetails item)
        {
            var mouseGridPos = _currentGrid.WorldToCell(mousePosition);
            var currentTile = GetTileDetailsOnMousePosition(mouseGridPos);

            if (currentTile != null)
            {
                //WORKFLOW: 根据不同的物品类型来执行不同的操作
                switch (item.ItemType)
                {
                    case ItemType.Commodity:
                        EventHandler.CallDropItemInScene(item.ItemID, mousePosition);
                        break;
                    case ItemType.HoeTool://NOTE 是返回拷贝还是引用
                        SetDigGround(currentTile);
                        currentTile.daySinceDug = 0;
                        currentTile.CanDig = false;
                        currentTile.CanDropItem = false;
                        break;
                    case ItemType.WaterTool:
                        SetWaterGround(currentTile);
                        currentTile.daySinceWatered = 0;
                        break;
                }
            }
        }

        /// <summary>
        /// 设置挖地的瓦片
        /// </summary>
        /// <param name="tile"></param>
        private void SetDigGround(TileDetails tile)
        {
            if (_digTileMap == null)
            {
                return;
            }

            Vector3Int pos = new Vector3Int(tile.GridX, tile.GridY, 0);
            _digTileMap.SetTile(pos, DigTile);
        }

        /// <summary>
        /// 设置浇水的瓦片
        /// </summary>
        /// <param name="tile"></param>
        private void SetWaterGround(TileDetails tile)
        {
            if (_waterTileMap == null)
            {
                return;
            }

            Vector3Int pos = new Vector3Int(tile.GridX, tile.GridY, 0);
            _waterTileMap.SetTile(pos, WaterTile);
        }
    }
}

