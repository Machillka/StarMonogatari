using UnityEngine;

namespace Farm.CropPlant
{
    public class CropManager : Singleton<CropManager>
    {
        public CropDataListSO CropDataBase;
        private Transform _cropParent;
        private Grid _currentGrid;
        private Seasons _currentSeason;

        private void OnEnable()
        {
            EventHandler.PlantSeedEvent += OnPlantSeedEvent;
            EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
            EventHandler.GameDayEvent += OnGameDayEvent;
        }

        private void OnDisable()
        {
            EventHandler.PlantSeedEvent -= OnPlantSeedEvent;
            EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
            EventHandler.GameDayEvent -= OnGameDayEvent;
        }

        private void OnGameDayEvent(int day, Seasons season)
        {
            _currentSeason = season;
        }

        private void OnAfterSceneLoadedEvent()
        {
            _currentGrid = FindAnyObjectByType<Grid>();
            _cropParent = GameObject.FindWithTag("CropParent").transform;
        }

        private void OnPlantSeedEvent(int itemID, TileDetails tileInformation)
        {
            CropDetails currentCrop = GetCropDetails(itemID);

            if (currentCrop == null)
            {
                Debug.LogError($"种子库中没有找到 itemID: {itemID} 的信息");
                return;
            }

            if (!SeasonAvaliable(currentCrop))
            {
                Debug.LogError($"当前季节 {_currentSeason} 不适合种植 {currentCrop.SeedItemID}");
                return;
            }

            // 空格子
            if (tileInformation.seedItemID == -1)
            {
                // Debug.Log("Space to Plant");
                tileInformation.seedItemID = itemID;
                tileInformation.growthDays = 0;
            }

            DisplayCropPlant(tileInformation, currentCrop);

        }

        /// <summary>
        /// 通过种子的 itemid 找到在种子库中对应的信息
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public CropDetails GetCropDetails(int itemID)
        {
            return CropDataBase.CropDataList.Find(crop => crop.SeedItemID == itemID);
        }

        private bool SeasonAvaliable(CropDetails crop)
        {
            for (int i = 0; i < crop.Seasons.Length; i++)
            {
                if (crop.Seasons[i] == _currentSeason)
                    return true;
            }
            return false;
        }

        private void DisplayCropPlant(TileDetails tileInformation, CropDetails cropInformation)
        {
            int growStages = cropInformation.GrowthDays.Length;
            int currentStage = 0;
            int dayCounter = cropInformation.TotalGrouthDays;

            for (int i = growStages - 1; i >= 0; i--)
            {
                if (tileInformation.growthDays >= dayCounter)
                {
                    currentStage = i;
                    break;
                }

                dayCounter -= cropInformation.GrowthDays[i];
            }

            GameObject cropPrefab = cropInformation.GrowthStagePrefab[currentStage];
            Sprite cropSprite = cropInformation.GrowthStageSprites[currentStage];
            Vector3 cropPosition = new Vector3(tileInformation.GridX + 0.5f, tileInformation.GridY + 0.5f, 0);
            GameObject crop = Instantiate(cropPrefab, cropPosition, Quaternion.identity, _cropParent);
            //TODO: 使用事件实现
            crop.GetComponentInChildren<SpriteRenderer>().sprite = cropSprite;
            crop.GetComponent<Crop>().CropDetail = cropInformation;
        }
    }
}

