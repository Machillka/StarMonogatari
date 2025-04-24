using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class GridMap : MonoBehaviour
{
    public MapDataSO MapData;
    public GridTypes GridType;
    private Tilemap currentTilemap;

    public void OnEnable()
    {
        if (!Application.IsPlaying(this))
        {
            currentTilemap = GetComponent<Tilemap>();
            if (MapData != null)
            {
                MapData.TileProperties.Clear();
                UpdateTileProperties();
            }
        }
    }

    private void OnDisable()
    {
        if (!Application.IsPlaying(this))
        {
            currentTilemap = GetComponent<Tilemap>();
            UpdateTileProperties();
#if UNITY_EDITOR
            if (MapData != null)
            {
                EditorUtility.SetDirty(MapData);
            }
#endif
        }
    }

    private void UpdateTileProperties()
    {
        currentTilemap.CompressBounds();

        if (!Application.IsPlaying(this))
        {
            if (MapData != null)
            {
                Vector3Int startPos = currentTilemap.cellBounds.min;
                Vector3Int endPos = currentTilemap.cellBounds.max;

                for (int i = startPos.x; i < endPos.x; i++)
                {
                    for (int j = startPos.y; j < endPos.y; j++)
                    {
                        TileBase tile = currentTilemap.GetTile(new Vector3Int(i, j, 0));

                        if (tile != null)
                        {
                            TileProperty newTile = new TileProperty
                            {
                                TileCoordinate = new Vector2Int(i, j),
                                TileType = GridType,
                                BoolTypeValue = true
                            };
                            MapData.TileProperties.Add(newTile);
                        }
                    }
                }
            }
        }
    }
}
