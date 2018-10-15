using System;
using System.Collections.Generic;

namespace Framework.AStar
{
    public class AStar
    {
        private const int STRAIGHT_LEN = 10;
        private const int OBLIQUE_LEN = 14;

        private GridMap2D m_map;
        private List<AStarNode> m_openList;
        private Dictionary<int, AStarNode> m_openMap;
        private HashSet<int> m_closeMap;

        private readonly static IPoint[] m_range = {
        new IPoint(-1, 1), new IPoint(0, 1), new IPoint(1, 1),
        new IPoint(-1, 0), /*  （⊙o⊙）  */  new IPoint(1, 0),
        new IPoint(-1, -1), new IPoint(0, -1), new IPoint(1, -1),
    };

        private readonly static IPoint[] m_ignoredCornerRange = {
        /*  （⊙o⊙）  */ new IPoint(0, 1), /*  （⊙o⊙）  */
        new IPoint(-1, 0), /*  （⊙o⊙）  */ new IPoint(1, 0),
        /*  （⊙o⊙）  */ new IPoint(0, -1), /*  （⊙o⊙）  */
    };

        public AStar(GridMap2D map)
        {
            m_map = map;
            m_openList = new List<AStarNode>();
            m_openMap = new Dictionary<int, AStarNode>();
            m_closeMap = new HashSet<int>();
        }

        public List<IPoint> FindPath(int fromX, int fromY, int toX, int toY, int obstacle, bool ignoreCorners = false)
        {
            m_openList.Clear();
            m_openMap.Clear();
            m_closeMap.Clear();
            AStarNode endNode = null;
            AStarNode topNode = new AStarNode(fromX, fromY);
            IPoint endPoint = new IPoint(toX, toY);
            List<IPoint> path = new List<IPoint>();
            m_openList.Add(topNode);
            m_openMap.Add(fromX << 16 | fromY, topNode);
            while (m_openList.Count > 0)
            {
                topNode = PopTheSmallestNode();
                if (topNode.X == toX && topNode.Y == toY)
                {
                    endNode = topNode;
                    break;
                }
                FindAroundNode(topNode, endPoint, obstacle, ignoreCorners);
            }
            if (endNode != null)
            {
                path.Add(new IPoint(endNode.X, endNode.Y));
                while (endNode.Parent != null)
                {
                    path.Add(new IPoint(endNode.Parent.X, endNode.Parent.Y));
                    endNode = endNode.Parent;
                }
            }
            return path;
        }

        private void FindAroundNode(AStarNode centerNode, IPoint endPoint, int obstacle, bool ignoreCorners = false)
        {
            IPoint[] range = ignoreCorners ? m_ignoredCornerRange : m_range;
            AStarNode tempNode = null;
            int tempKey = 0;
            for (int i = 0; i < range.Length; ++i)
            {
                IPoint point = new IPoint(range[i].X, range[i].Y);
                point.X += centerNode.X;
                point.Y += centerNode.Y;
                tempKey = point.X << 16 | point.Y;
                if (m_closeMap.Contains(tempKey))
                    continue;
                // 检查point是否有效
                if (!m_map.IsAvailable(point.X, point.Y) || m_map.HasType(point.X, point.Y, obstacle))
                {
                    m_closeMap.Add(tempKey);
                    continue;
                }
                // 如果point和center是斜对的两个格子，检查是否有斜方向阻挡
                if (IsDiagonally(centerNode.X, centerNode.Y, point.X, point.Y))
                {
                    if (m_map.HasType(point.X, centerNode.Y, obstacle))
                        continue;
                    if (m_map.HasType(centerNode.X, point.Y, obstacle))
                        continue;
                }
                if (m_openMap.ContainsKey(tempKey))
                {
                    tempNode = m_openMap[tempKey];
                    int g = CalculateG(centerNode, point);
                    if (tempNode.G > g)
                    {
                        tempNode.Parent = centerNode;
                        tempNode.G = g;
                    }
                }
                else
                {
                    tempNode = new AStarNode(point.X, point.Y);
                    tempNode.Parent = centerNode;
                    m_openList.Add(tempNode);
                    m_openMap.Add(tempKey, tempNode);
                    tempNode.G = CalculateG(centerNode, point);
                }
                tempNode.H = CalculateH(point, endPoint);
            }
        }

        private int CalculateG(AStarNode parent, IPoint curPoint)
        {
            int value = Math.Abs(curPoint.X - parent.X) + Math.Abs(curPoint.Y - parent.Y);
            return parent.G + (value == 2 ? OBLIQUE_LEN : STRAIGHT_LEN);
        }

        private int CalculateH(IPoint curPoint, IPoint endPoint)
        {
            return (Math.Abs(curPoint.X - endPoint.X) + Math.Abs(curPoint.Y - endPoint.Y)) * STRAIGHT_LEN;
        }

        private AStarNode PopTheSmallestNode()
        {
            if (m_openList.Count > 1)
            {
                m_openList.Sort((n1, n2) =>
                {
                    if (n1.F > n2.F)
                        return 1;
                    else if (n1.F < n2.F)
                        return -1;
                    return 0;
                });
            }
            AStarNode node = m_openList[0];
            m_openList.RemoveAt(0);
            return node;
        }

        private bool IsDiagonally(int x1, int y1, int x2, int y2)
        {
            return (Math.Abs(x2 - x1) + Math.Abs(y2 - y1)) > 1;
        }

        public class AStarNode
        {
            public AStarNode(int x, int y)
            {
                X = x;
                Y = y;
            }

            public AStarNode Parent { get; set; }

            public int G { get; set; }

            public int H { get; set; }

            public int F { get { return G + H; } }

            public int X { get; set; }

            public int Y { get; set; }
        }
    }

    [System.Serializable]
    public class Cell
    {
        public int X;

        public int Y;

        public int Type;

        public string Tag;
    }

    public class IPoint
    {
        public int X { get; set; }
        public int Y { get; set; }

        public IPoint() { }

        public IPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(IPoint a, IPoint b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(IPoint a, IPoint b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public override bool Equals(object obj)
        {
            if (obj is IPoint)
                return obj as IPoint == this;
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "x:" + X + "\ty:" + Y;
        }

        public static readonly IPoint Unavailable = new IPoint(0x7FFFFFFF, 0x7FFFFFFF);
    }
}