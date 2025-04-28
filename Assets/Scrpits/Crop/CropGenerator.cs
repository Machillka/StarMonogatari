using Farm.Map;
using UnityEngine;

namespace Farm.CropPlant
{
    public class CropGenerator : MonoBehaviour
    {
        private Grid _currentGrid;
        public int seedItemID;
        public int growthDays;

        private void Awake()
        {
            _currentGrid = FindAnyObjectByType<Grid>();
        }

        private void OnEnable()
        {
            EventHandler.GenerateCropEvent += GenerateCrop;
        }

        private void OnDisable()
        {
            EventHandler.GenerateCropEvent -= GenerateCrop;
        }

        private void GenerateCrop()
        {
            Vector3Int cropGridPos = _currentGrid.WorldToCell(transform.position);

            if (seedItemID != 0)
            {
                // Debug.Log("Generate in CropGenerator, Seed ID = " + seedItemID);
                TileDetails tile = GridMapManager.Instance.GetTileDetailsOnMousePosition(cropGridPos);

                tile ??= new TileDetails();

                tile.daySinceWatered = -1;
                tile.seedItemID = seedItemID;
                tile.growthDays = growthDays;

                GridMapManager.Instance.UpdateTileDetails(tile);
            }
        }
    }
}

