using System;
using UnityEngine;

namespace Farm.Astar
{
    public class Node : IComparable<Node>
    {
        public Vector2Int gridPosition;         // 网格坐标
        public int gCost = 0;                   // 当前节点到 Start 节点的距离
        public int hCost = 0;                   // 当前节点到 Target 节点的距离
        public int FCost => gCost + hCost;      // 当前节点总代价

        public bool isObstacle = false;         // 是否可以通行

        public Node parentNode;

        public Node(Vector2Int position)
        {
            gridPosition = position;
        }

        public int CompareTo(Node other)
        {
            int result = FCost.CompareTo(other.FCost);

            if (result == 0)                    // 总距离相等
            {
                result = hCost.CompareTo(other.hCost);
            }

            return result;
        }
    }
}

