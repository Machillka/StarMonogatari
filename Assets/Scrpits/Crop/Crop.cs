using UnityEngine;

public class Crop : MonoBehaviour
{
    public CropDetails CropDetail;
    private int _harvestActionCount;

    public void ProcessToolAction(ItemDetails tool)
    {
        int requireActionCount = CropDetail.GetTotalRequireCount(tool.ItemID);

        if (requireActionCount == -1)
            return;

        // TODO: 判断动画

        // TODO: 判断特效

        // TODO[x]: 计数器
        if (_harvestActionCount < requireActionCount)
        {
            _harvestActionCount++;
        }

        if (_harvestActionCount == requireActionCount)
        {
            if (CropDetail.GenerateAtPlayerPosition)
            {
                SpawnHarvestItems();
            }
        }
    }

    private void SpawnHarvestItems()
    {
        for (int i = 0; i < CropDetail.ProducedItemID.Length; i++)
        {
            int amountToProduce;
            if (CropDetail.ProduceMinAmount[i] == CropDetail.ProduceMaxAmount[i])
            {
                amountToProduce = CropDetail.ProduceMinAmount[i];
            }
            else
            {
                amountToProduce = Random.Range(CropDetail.ProduceMinAmount[i], CropDetail.ProduceMaxAmount[i] + 1);
            }

            for (int j = 0; j < amountToProduce; j++)
            {
                if (CropDetail.GenerateAtPlayerPosition)
                {
                    EventHandler.CallHarvestAtPlaterPositionEvent(CropDetail.ProducedItemID[i]);
                }
            }
        }
    }
}
