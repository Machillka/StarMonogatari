using UnityEngine;

public class Settings
{
    public const float itemFadeDuration = 0.3f;
    public const float spriteAlpha = 0.45f;
    public const float ToolTipOffset = 60;
    public const float ItemTipShowUpTimeOffset = 0.5f;              // 鼠标停留此时间后才显示物品详情页面

    // Time
    public const float secondThreshold = 0.012f;
    public const int secondHold = 59;
    public const int minuteHold = 59;
    public const int hourHold = 23;
    public const int dayHold = 10;                                  // 一个月几天
    public const int monthHold = 12;
    public const int seasonHold = 3;

    public const float sceneFadeDuration = 0.5f;

    public const int reapAmount = 3;                                // 一次收获的数量 //TODO: 设计成一个随机范围

    public const float gridCellSize = 1f;
    public const float gridCellDiagonalSize = 1.41f;
    public const float pixelSize = 0.05f;                           // 20 * 20 的格子占 1 个 unit 的 0.05
}
