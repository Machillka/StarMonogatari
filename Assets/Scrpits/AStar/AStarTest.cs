using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace Farm.Astar
{
    public class AStarTest : MonoBehaviour
    {
        private AStar _aStar;

        [Header("Test")]
        public Vector2Int startPos;
        public Vector2Int targetPos;
        public Tilemap displayMap;
        public TileBase displayTile;
        public bool isDisplayStartAndTarget;
        public bool isDisplayPath;
        private Stack<MovementStep> npcMovementStepStack = new Stack<MovementStep>();

        private void Awake()
        {
            _aStar = GetComponent<AStar>();
        }

        private void Update()
        {
            ShowPathOnGridMap();
        }

        private void ShowPathOnGridMap()
        {
            if (displayMap != null && displayTile != null)
            {
                if (isDisplayStartAndTarget)
                {
                    displayMap.SetTile((Vector3Int)startPos, displayTile);
                    displayMap.SetTile((Vector3Int)targetPos, displayTile);
                }
                else
                {
                    displayMap.SetTile((Vector3Int)startPos, null);
                    displayMap.SetTile((Vector3Int)targetPos, null);
                }

                if (isDisplayPath)
                {
                    var sceneName = SceneManager.GetActiveScene().name;

                    _aStar.BuildPath(sceneName, startPos, targetPos, npcMovementStepStack);

                    foreach (var step in npcMovementStepStack)
                    {
                        displayMap.SetTile((Vector3Int)step.gridCoordinate, displayTile);
                    }
                }
                else
                {
                    if (npcMovementStepStack.Count > 0)
                    {
                        foreach (var step in npcMovementStepStack)
                        {
                            displayMap.SetTile((Vector3Int)step.gridCoordinate, null);
                        }
                        npcMovementStepStack.Clear();
                    }
                }
            }
        }
    }
}
