using System.Collections;
using UnityEngine;

public class Crop : MonoBehaviour
{
    public CropDetails CropInformation;
    private int _harvestActionCount;
    public TileDetails tileDetails;
    private Animator _anim;
    private Transform _playerTransform => FindAnyObjectByType<PlayerMovement>().transform;

    public bool CanHarvest => tileDetails != null && tileDetails.growthDays >= CropInformation.TotalGrouthDays;

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
        tileDetails = tileInformattion;

        int requireActionCount = CropInformation.GetTotalRequireCount(tool.ItemID);

        if (requireActionCount == -1)
            return;

        _anim = GetComponentInChildren<Animator>();

        // TODO[x]: 计数器
        if (_harvestActionCount < requireActionCount)
        {
            _harvestActionCount++;

            // TODO: 判断动画
            if (_anim != null && CropInformation.IsHadAnimation)
            {
                if (_playerTransform.position.x < transform.position.x)
                {
                    _anim.SetTrigger("RotateRight");
                }
                else
                {
                    _anim.SetTrigger("RotateLeft");
                }
            }

            // TODO: 判断特效
        }

        if (_harvestActionCount >= requireActionCount)
        {
            if (CropInformation.GenerateAtPlayerPosition)
            {
                // Debug.Log("Harvesting");
                SpawnHarvestItems();
            }
            else if (CropInformation.IsHadAnimation)
            {
                if (_playerTransform.position.x < transform.position.x)
                {
                    _anim.SetTrigger("FallRight");
                }
                else
                {
                    _anim.SetTrigger("FallLeft");
                }

                StartCoroutine(HarvestAfterAnimation());
            }
            // else
            // {
            //     SpawnHarvestItems();
            // }
        }
    }

    private IEnumerator HarvestAfterAnimation()
    {
        while (_anim.GetCurrentAnimatorStateInfo(0).IsName("End"))
        {
            yield return null;
        }

        SpawnHarvestItems();

        // 转化物品
    }

    /// <summary>
    /// 生成收获的物品
    /// </summary>
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

        if (tileDetails != null)
        {
            tileDetails.daySinceLastHarvest++;

            if (CropInformation.DaysToRegrow > 0 && tileDetails.daySinceLastHarvest < CropInformation.RegrowTimes)
            {
                tileDetails.growthDays = CropInformation.TotalGrouthDays - CropInformation.DaysToRegrow;
                EventHandler.CallRefreshCurrentMapEvent();
            }
            else
            {
                tileDetails.daySinceLastHarvest = -1;
                tileDetails.seedItemID = -1;

                // 重新挖坑
                // tileDetails.daySinceDug = -1;
            }

            Destroy(gameObject);
        }
    }
}
