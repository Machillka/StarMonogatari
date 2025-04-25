using System.Text.RegularExpressions;
using UnityEngine;

[System.Serializable]
public class CropDetails
{
    public int SeedItemID;

    public int[] GrowthDays;                    // 不同阶段需要的天数
    public int TotalGrouthDays                  // 总共需要的天数
    {
        get
        {
            int amount = 0;
            foreach (var day in GrowthDays)
            {
                amount += day;
            }
            return amount;
        }
    }

    public GameObject[] GrowthStagePrefab;      // 不同生长阶段在地图上生成的预制体
    public Sprite[] GrowthStageSprites;         // 不同生长阶段的图片
    public Seasons[] Seasons;                   // 作物生长的季节

    [Space]
    [Header("Harvest Information")]
    public int[] HarvestToolItemID;             // 收获所需的工具 ID
    public int[] RequireActionCount;            // 收获所需的动作次数

    //NOTE 感觉变成 index -> 表示收获之后变成的成长状态, 如果为 -1 就是直接干没了
    public int transferItemID;                  // 收获后转化的物品 ID

    [Space]
    [Header("Harvest Fruit Information")]
    public int[] ProducedItemID;                // 收获的物品 ID
    public int[] ProduceMinAmount;              // 随机收获从最小到最大数量的果实
    public int[] ProduceMaxAmount;

    public Vector2 SpawnRadius;                 // 物品在地图上生成的半径

    public int DaysToRegrow;                    // 收获后再生长所需的天数
    public int RegrowTimes;                     // 还可以重新生长的次数

    [Header("Options")]
    public bool GenerateAtPlayerPosition;
    public bool IsHadAnimation;
    public bool IsHadParticalEffect;
}
