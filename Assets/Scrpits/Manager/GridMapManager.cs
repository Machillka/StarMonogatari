using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Farm.CropPlant;

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

        private Seasons _currentSeason;

        [Header("Scene Information")]
        private Dictionary<string, bool> _isFirstLoadScene = new Dictionary<string, bool>();

        private void Start()
        {
            //TODO: 利用addressable加载地图数据 或者其他动态加载的方式
            foreach (var mapInformation in MapDataList)
            {
                _isFirstLoadScene.Add(mapInformation.SceneName, true);
                InitTileDetailDictionary(mapInformation);
            }
        }

        private void OnEnable()
        {
            EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
            EventHandler.ExcuteActionAfterAnimation += OnExcuteActionAfterAnimation;
            EventHandler.GameDayEvent += OnGameDayEvent;
            EventHandler.RefreshCurrentMapEvent += RefreshMap;
        }

        private void OnDisable()
        {
            EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
            EventHandler.ExcuteActionAfterAnimation -= OnExcuteActionAfterAnimation;
            EventHandler.GameDayEvent -= OnGameDayEvent;
            EventHandler.RefreshCurrentMapEvent -= RefreshMap;
        }

        private void OnAfterSceneLoadedEvent()
        {
            _currentGrid = FindAnyObjectByType<Grid>();
            _digTileMap = GameObject.FindWithTag("Dig").GetComponent<Tilemap>();
            _waterTileMap = GameObject.FindWithTag("Water").GetComponent<Tilemap>();

            if (_isFirstLoadScene[SceneManager.GetActiveScene().name])
            {
                EventHandler.CallGenerateCropEvent();
                _isFirstLoadScene[SceneManager.GetActiveScene().name] = false;
            }

            // DisplayMap(SceneManager.GetActiveScene().name);
                RefreshMap();
        }

        /// <summary>
        /// 执行实际效果
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <param name="item"></param>
        private void OnExcuteActionAfterAnimation(Vector3 mousePosition, ItemDetails item)
        {
            Debug.Log($"OnExcuteActionAfterAnimation, mousePosition = {mousePosition}, item = {item.ItemName}");
            var mouseGridPos = _currentGrid.WorldToCell(mousePosition);
            var currentTile = GetTileDetailsOnMousePosition(mouseGridPos);

            if (currentTile != null)
            {
                Crop currentCrop = GetCropObject(mousePosition);
                //WORKFLOW: 根据不同的物品类型来执行不同的操作
                switch (item.ItemType)
                {
                    case ItemType.Seed:
                        EventHandler.CallPlantSeedEvent(item.ItemID, currentTile);
                        EventHandler.CallDropItemInScene(item.ItemID, mousePosition, item.ItemType);
                        break;

                    case ItemType.Commodity:
                        EventHandler.CallDropItemInScene(item.ItemID, mousePosition, item.ItemType);
                        break;
                    case ItemType.HoeTool://NOTE 是返回拷贝还是引用 -> 是引用
                        SetDigGround(currentTile);
                        currentTile.daySinceDug = 0;
                        currentTile.CanDig = false;
                        currentTile.CanDropItem = false;
                        break;
                    case ItemType.WaterTool:
                        SetWaterGround(currentTile);
                        currentTile.daySinceWatered = 0;
                        break;
                    case ItemType.CollectTool:
                        if (currentCrop != null)
                        {
                            currentCrop.ProcessToolAction(item, currentTile);
                        }
                        break;
                    case ItemType.BreakTool:
                        if (currentCrop != null)
                        {
                            currentCrop.ProcessToolAction(item, currentCrop.tileDetails);
                        }
                        break;
                    case ItemType.ChopTool:
                        if (currentCrop != null)
                        {
                            currentCrop?.ProcessToolAction(item, currentCrop.tileDetails);
                        }
                        break;
                }

                UpdateTileDetails(currentTile);
            }
        }

        /// <summary>
        /// 检测是否有农作物被点击
        /// </summary>
        /// <param name="mouseWorldPosition"></param>
        /// <returns></returns>
        public Crop GetCropObject(Vector3 mouseWorldPosition)
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(mouseWorldPosition);
            Crop currentCrop = null;

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].GetComponent<Crop>())
                {
                    currentCrop = colliders[i].GetComponent<Crop>();
                    break;
                }
            }

            return currentCrop;
        }

        private void OnGameDayEvent(int day, Seasons season)
        {
            _currentSeason = season;
            foreach (var key in itemDetailsDict.Keys.ToList())
            {
                if (itemDetailsDict[key].daySinceWatered > -1)
                {
                    itemDetailsDict[key].daySinceWatered = -1;
                }
                if (itemDetailsDict[key].daySinceDug > -1)
                {
                    itemDetailsDict[key].daySinceDug++;
                }

                if (itemDetailsDict[key].daySinceDug > 3 && itemDetailsDict[key].seedItemID == -1)
                {
                    itemDetailsDict[key].daySinceDug = -1;
                    itemDetailsDict[key].CanDig = true;
                    itemDetailsDict[key].growthDays = -1;
                }

                if (itemDetailsDict[key].seedItemID != -1)
                {
                    itemDetailsDict[key].growthDays++;
                }
            }

            EventHandler.CallGenerateCropEvent();

            RefreshMap();
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
        /// <returns>对于得到的网格信息的引用</returns>
        public TileDetails GetTileDetailsOnMousePosition(Vector3Int mouseGridPosition)
        {
            string key = mouseGridPosition.x + "x" + mouseGridPosition.y + "y" + SceneManager.GetActiveScene().name;
            if (itemDetailsDict.ContainsKey(key))
            {
                return itemDetailsDict[key];
            }
            return null;
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

        public void UpdateTileDetails(TileDetails tileInformation)
        {
            string key = tileInformation.GridX + "x" + tileInformation.GridY + "y" + SceneManager.GetActiveScene().name;
            if (itemDetailsDict.ContainsKey(key))
            {
                itemDetailsDict[key] = tileInformation;
            }
            else
            {
                itemDetailsDict.Add(key, tileInformation);
            }
        }

        private void DisplayMap(string sceneName)
        {
            foreach (var tilePair in itemDetailsDict)
            {
                var key = tilePair.Key;
                var tileDetail = tilePair.Value;

                if (key.Contains(sceneName))
                {
                    if (tileDetail.daySinceDug > -1)
                    {
                        SetDigGround(tileDetail);
                    }
                    if (tileDetail.daySinceWatered > -1)
                    {
                        SetWaterGround(tileDetail);
                    }
                    if (tileDetail.seedItemID != -1)
                    {
                        EventHandler.CallPlantSeedEvent(tileDetail.seedItemID, tileDetail);
                    }
                }
            }
        }

        private void RefreshMap()
        {
            if (_digTileMap != null)
            {
                _digTileMap.ClearAllTiles();
            }
            if (_waterTileMap != null)
            {
                _waterTileMap.ClearAllTiles();
            }

            foreach (var crop in FindObjectsByType<Crop>(FindObjectsSortMode.None))
            {
                // Debug.Log($"Destroying {crop}");
                Destroy(crop.gameObject);
                // FIXME: 不应该直接销毁，一个是开销；另一个是可能会有引用问题（Dowtween 做树木透明度变化的时候会出现空物体）
            }

            DisplayMap(SceneManager.GetActiveScene().name);
        }

    }
}

