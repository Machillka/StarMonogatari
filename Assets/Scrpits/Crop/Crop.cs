using UnityEngine;

public class Crop : MonoBehaviour
{
    public CropDetails CropDetail;
    private int _harvestActionCount;

    private void Awake()
    {
        InitCrop();
    }

    private void InitCrop()
    {
        _harvestActionCount = 0;
    }

    public void ProcessToolAction(ItemDetails tool)
    {
        // Debug.Log("Processing!");

        int requireActionCount = CropDetail.GetTotalRequireCount(tool.ItemID);

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
            if (CropDetail.GenerateAtPlayerPosition)
            {
                // Debug.Log("Harvesting");
                SpawnHarvestItems();
            }
        }
    }

    private void SpawnHarvestItems()
    {
        for (int i = 0; i < CropDetail.ProducedItemID.Length; i++)
        {
            int amountToProduce;
            // Debug.Log($"ProduceMinAmount:{CropDetail.ProduceMinAmount[i]}  | Max:{CropDetail.ProduceMaxAmount[i]} ");
            if (CropDetail.ProduceMinAmount[i] == CropDetail.ProduceMaxAmount[i])
            {
                amountToProduce = CropDetail.ProduceMinAmount[i];
            }
            else
            {
                amountToProduce = Random.Range(CropDetail.ProduceMinAmount[i], CropDetail.ProduceMaxAmount[i] + 1);
            }
            // Debug.Log($"Produce {amountToProduce} of item");
            for (int j = 0; j < amountToProduce; j++)
            {
                if (CropDetail.GenerateAtPlayerPosition)
                {
                    // Debug.Log("Spawn at Player Position");
                    EventHandler.CallHarvestAtPlaterPositionEvent(CropDetail.ProducedItemID[i]);
                }
            }
        }
    }
}
