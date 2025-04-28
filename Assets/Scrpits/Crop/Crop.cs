using System.Collections;
using UnityEngine;
/*
此处存放的 tiledetail 是当前种植的格子信息，与全局信息 " 无关 "
因为在全局刷新地图的时候, 会 destroy 所有包含有 <crop> 组件的物体
*/
public class Crop : MonoBehaviour
{
    public CropDetails CropInformation;
    private int _harvestActionCount;
    public TileDetails tileDetails;
    private Animator _anim;
    private Transform _playerTransform => FindAnyObjectByType<PlayerMovement>().transform;

    public bool CanHarvest => tileDetails != null && tileDetails.growthDays >= CropInformation.TotalGrouthDays;

    // public Vector3 particalEffectPosition => transform.position + new Vector3(0, 0.5f, 0);

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
            if (CropInformation.IsHadParticalEffect)
                EventHandler.CallParticalEffectEvent(CropInformation.effectType, transform.position + CropInformation.particalEffectPos);
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
            else
            {
                SpawnHarvestItems();
            }
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
        if (CropInformation.transferItemID > 0)
            CreateTransferCrop();
    }

    private void CreateTransferCrop()
    {
        tileDetails.seedItemID = CropInformation.transferItemID;
        tileDetails.growthDays = 0;
        tileDetails.daySinceLastHarvest = -1;

        EventHandler.CallRefreshCurrentMapEvent();
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
                    var dirX = transform.position.x > _playerTransform.position.x ? 1 : -1;
                    // 随机生成物品的坐标
                    var spawnPos = new Vector3(
                        transform.position.x + Random.Range(dirX, CropInformation.SpawnRadius.x * dirX),
                        transform.position.y + Random.Range(-CropInformation.SpawnRadius.y, CropInformation.SpawnRadius.y),
                        0f
                    );
                    EventHandler.CallInstantiateItemInScene(CropInformation.ProducedItemID[i], spawnPos);
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
