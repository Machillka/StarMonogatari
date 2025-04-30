using UnityEngine;
namespace Farm.Astar
{
    public class GridNodeManager
    {
        private int _width;
        private int _height;

        private Node[,] _gridNodes;

        /// <summary>
        /// 构造函数, 初始化网格地图信息
        /// </summary>
        /// <param name="width">GridMap 地图宽度</param>
        /// <param name="height">GridMap 地图高度</param>
        public GridNodeManager(int width, int height)
        {
            _width = width;
            _height = height;

            _gridNodes = new Node[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    _gridNodes[x, y] = new Node(new Vector2Int(x, y));
                }
            }
        }

        public Node GetGridNode(int xPos, int yPos)
        {
            if (xPos < _width && yPos < _height)
                return _gridNodes[xPos, yPos];

            Debug.LogWarning("超出网格范围");

            return null;
        }
    }
}

