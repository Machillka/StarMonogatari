using System.Collections.Generic;
using Farm.Map;
using Unity.Burst.Intrinsics;
using UnityEngine;

namespace Farm.Astar
{
    public class AStar : Singleton<AStar>
    {
        private GridNodeManager _gridNodes;
        private Node _startNode;
        private Node _targetNode;

        private int _gridWidth;
        private int _gridHeight;

        private int _originX;
        private int _originY;

        private List<Node> _openNodeList = new List<Node>();            // 当前节点周围可搜索的 8 个点
        private HashSet<Node> _closeNodeList = new HashSet<Node>();     // 已经搜索过的节点

        private bool _isPathFound = false;                              // 是否查找到路径

        /// <summary>
        /// 构建路径更新 Stack
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="startPos"></param>
        /// <param name="targetPos"></param>
        /// <param name="npcMovementSteps"></param>
        public void BuildPath(string sceneName, Vector2Int startPos, Vector2Int targetPos, Stack<MovementStep> npcMovementSteps)
        {
            _isPathFound = false;

            if (GenerateGridNodes(sceneName, startPos, targetPos) == false)
            {
                Debug.LogError("生成网格节点失败");
                return;
            }

            if (FindShortestPath())
            {
                // 构建路径信息
                UpdatePathOnMovementStepStack(sceneName, npcMovementSteps);
            }
        }

        private bool FindShortestPath()
        {
            // 添加起点
            _openNodeList.Add(_startNode);

            while (_openNodeList.Count > 0)
            {
                // 从已有的节点排序
                _openNodeList.Sort();
                // 因为已经排序 所以直接得到最小值
                Node closeNode = _openNodeList[0];

                _openNodeList.RemoveAt(0);
                _closeNodeList.Add(closeNode);

                if (closeNode == _targetNode)
                {
                    _isPathFound = true;
                    break;
                }

                // 添加周围八个点的信息
                EvaluateNeighborNodes(closeNode);
            }

            return _isPathFound;
        }

        /// <summary>
        /// 评估周围八个点的价值
        /// </summary>
        /// <param name="currentNode"></param>
        private void EvaluateNeighborNodes(Node currentNode)
        {
            Vector2Int currentPos = currentNode.gridPosition;
            Node validNeighborNode;

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    validNeighborNode = GetValidNwighborNode(currentPos.x + x, currentPos.y + y);
                    if (validNeighborNode != null)
                    {
                        if (!_openNodeList.Contains(validNeighborNode))
                        {
                            validNeighborNode.gCost = currentNode.gCost + GetDistance(currentNode, validNeighborNode);
                            validNeighborNode.hCost = GetDistance(validNeighborNode, _targetNode);

                            validNeighborNode.parentNode = currentNode;
                            _openNodeList.Add(validNeighborNode);
                        }
                    }
                }
            }
        }

        private Node GetValidNwighborNode(int posX, int posY)
        {
            if (posX < 0 || posX >= _gridWidth || posY < 0 || posY >= _gridHeight)
            {
                return null;
            }

            Node node = _gridNodes.GetGridNode(posX, posY);

            if (node.isObstacle || _closeNodeList.Contains(node))
            {
                return null;
            }

            return node;
        }

        /// <summary>
        /// 生成网格节点的信息
        /// </summary>
        /// <param name="scaneName"></param>
        /// <param name="startPos"></param>
        /// <param name="targetPos"></param>
        /// <returns></returns>
        private bool GenerateGridNodes(string scaneName, Vector2Int startPos, Vector2Int targetPos)
        {
            if (GridMapManager.Instance.GetGridDimensions(scaneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin))
            {
                // 生成信息
                _gridWidth = gridDimensions.x;
                _gridHeight = gridDimensions.y;
                _originX = gridOrigin.x;
                _originY = gridOrigin.y;
                _gridNodes = new GridNodeManager(_gridWidth, _gridHeight);

                _openNodeList = new List<Node>();
                _closeNodeList = new HashSet<Node>();
            }
            else
                return false;

            _startNode = _gridNodes.GetGridNode(startPos.x - _originX, startPos.y - _originY);
            _targetNode = _gridNodes.GetGridNode(targetPos.x - _originX, targetPos.y - _originY);

            for (int x = 0; x < _gridWidth; x++)
            {
                for (int y = 0; y < _gridHeight; y++)
                {
                    var key = (x + _originX) + "x" + (y + _originY) + "y" +scaneName;
                    Vector3Int tilePos = new Vector3Int(x + _originX, y + _originY, 0);
                    TileDetails tile = GridMapManager.Instance.GetTileDetails(key);
                    if (tile != null)
                    {
                        Node node = _gridNodes.GetGridNode(x, y);

                        if (tile.IsNPCObstacle)
                        {
                            node.isObstacle = true;
                        }
                    }
                }
            }

            return true;
        }

        private int GetDistance(Node nodeA, Node nodeB)
        {
            int xDistance = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
            int yDistance = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

            if (xDistance > yDistance)
            {
                return 14 * yDistance + 10 * (xDistance - yDistance);
            }

            return 14 * xDistance + 10 * (yDistance - xDistance);
        }

        private void UpdatePathOnMovementStepStack(string sceneName, Stack<MovementStep> npcMovementSteps)
        {
            Node nextNode = _targetNode;

            while (nextNode != null)
            {
                MovementStep newStep = new MovementStep();
                newStep.sceneName = sceneName;
                newStep.gridCoordinate = new Vector2Int(nextNode.gridPosition.x + _originX, nextNode.gridPosition.y + _originY);

                npcMovementSteps.Push(newStep);

                nextNode = nextNode.parentNode;
            }
        }
    }
}
