using UnityEngine;

namespace Farm.CropPlant
{
    public class ReapItem : MonoBehaviour
    {
        private CropDetails _cropDetails;
        private Transform _playerTransform => FindAnyObjectByType<PlayerMovement>().transform;

        public void InitCropData(int itemID)
        {
            _cropDetails = CropManager.Instance.GetCropDetails(itemID);
        }


        /// <summary>
        /// 生成收获的物品
        /// </summary>
        private void SpawnHarvestItems()
        {
            for (int i = 0; i < _cropDetails.ProducedItemID.Length; i++)
            {
                int amountToProduce;
                // Debug.Log($"ProduceMinAmount:{_cropDetails.ProduceMinAmount[i]}  | Max:{_cropDetails.ProduceMaxAmount[i]} ");
                if (_cropDetails.ProduceMinAmount[i] == _cropDetails.ProduceMaxAmount[i])
                {
                    amountToProduce = _cropDetails.ProduceMinAmount[i];
                }
                else
                {
                    amountToProduce = Random.Range(_cropDetails.ProduceMinAmount[i], _cropDetails.ProduceMaxAmount[i] + 1);
                }
                // Debug.Log($"Produce {amountToProduce} of item");
                for (int j = 0; j < amountToProduce; j++)
                {
                    if (_cropDetails.GenerateAtPlayerPosition)
                    {
                        // Debug.Log("Spawn at Player Position");
                        EventHandler.CallHarvestAtPlaterPositionEvent(_cropDetails.ProducedItemID[i]);
                    }
                    else        // TODO: 世界地图生成
                    {
                        var dirX = transform.position.x > _playerTransform.position.x ? 1 : -1;
                        // 随机生成物品的坐标
                        var spawnPos = new Vector3(
                            transform.position.x + Random.Range(dirX, _cropDetails.SpawnRadius.x * dirX),
                            transform.position.y + Random.Range(-_cropDetails.SpawnRadius.y, _cropDetails.SpawnRadius.y),
                            0f
                        );
                        EventHandler.CallInstantiateItemInScene(_cropDetails.ProducedItemID[i], spawnPos);
                    }
                }
            }
        }
    }
}
