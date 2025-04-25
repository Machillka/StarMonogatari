using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CropDataListSO", menuName = "Crop/CropDataListSO", order = 0)]
public class CropDataListSO : ScriptableObject
{
    public List<CropDetails> CropDataList;
}