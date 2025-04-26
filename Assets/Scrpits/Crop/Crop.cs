using UnityEngine;

public class Crop : MonoBehaviour
{
    public CropDetails CropInformation;
    private int _harvestActionCount;
    private TileDetails _tileDetails;

    private void Awake()
    {
        InitCrop();
    }

    private void InitCrop()
    {
        _harvestActionCount = 0;
    }

    public void ProcessToolAction(ItemDetails tool, TileDetails tileInformattion)
    {
        // Debug.Log("Processing!");
        _tileDetails = tileInformattion;

        int requireActionCount = CropInformation.GetTotalRequireCount(tool.ItemID);

        // Debug.Log("RequireActionCount: " + requireActionCount.ToString());

        if (requireActionCount == -1)
            return;

        // TODO: 判断动画

        // TODO: 判断特效

        // TODO[x]: 计数器
        if (_harvestActionCount < requireActionCount)
        {
            _harvestActionCount++;
            // Debug.Log("HarvestActionCount + 1");
        }

        if (_harvestActionCount >= requireActionCount)
        {
            if (CropInformation.GenerateAtPlayerPosition)
            {
                // Debug.Log("Harvesting");
                SpawnHarvestItems();
            }
        }
    }

    private void SpawnHarvestItems()
    {
        for (int i = 0; i < CropInformation.ProducedItemID.Length; i++)
        {
            int amountToProduce;
            // Debug.Log($"ProduceMinAmount:{CropInformation.ProduceMinAmount[i]}  | Max:{CropInformation.ProduceMaxAmount[i]} ");
            if (CropInformation.ProduceMinAmount[i] == CropInformation.ProduceMaxAmount[i])
            {
                amountToProduce = CropInformation.ProduceMinAmount[i];
            }
            else
            {
                amountToProduce = Random.Range(CropInformation.ProduceMinAmount[i], CropInformation.ProduceMaxAmount[i] + 1);
            }
            // Debug.Log($"Produce {amountToProduce} of item");
            for (int j = 0; j < amountToProduce; j++)
            {
                if (CropInformation.GenerateAtPlayerPosition)
                {
                    // Debug.Log("Spawn at Player Position");
                    EventHandler.CallHarvestAtPlaterPositionEvent(CropInformation.ProducedItemID[i]);
                }
                else        // TODO: 世界地图生成
                {

                }
            }
        }

        if (_tileDetails != null)
        {
            _tileDetails.daySinceLastHarvest++;

            if (CropInformation.DaysToRegrow > 0 && _tileDetails.daySinceLastHarvest < CropInformation.RegrowTimes)
            {
                _tileDetails.growthDays = CropInformation.TotalGrouthDays - CropInformation.DaysToRegrow;
                EventHandler.CallRefreshCurrentMapEvent();
            }
            else
            {
                _tileDetails.daySinceLastHarvest = -1;
                _tileDetails.seedItemID = -1;

                // 重新挖坑
                // _tileDetails.daySinceDug = -1;
            }

            Destroy(gameObject);
        }
    }
}
