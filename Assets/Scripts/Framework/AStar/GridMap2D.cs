using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.AStar
{
    /*
     * GridMap2D plane was defined in X-Z plane of the world space 
     * Its forward direction is from (-x, -z) to (x, z)
     * **/
    [CreateAssetMenu]
    public class GridMap2D : ScriptableObject
    {
        #region Memebers

        //the offset in world space
        public Vector3 m_offsetInWorldSapce;

        //row count of the map
        public int m_rowCount;

        //col count of the map
        public int m_colCount;

        //the width of the each cell 
        public float m_cellWidth;

        //the height of the each cell
        public float m_cellHeight;

        //every cell
        [SerializeField]
        private List<Cell> m_cells;

        public GridCustom m_GridCustom;

        private AStar m_astar;

        #endregion

        #region Properties

        public float Length
        {
            get
            {
                return m_cellWidth * m_colCount;
            }
        }

        public float Height
        {
            get
            {
                return m_cellHeight * m_rowCount;
            }
        }

        public AStar _AStar
        {
            get
            {
                if (m_astar == null)
                    m_astar = new AStar(this);
                return m_astar;
            }
        }

        #endregion

        #region Public Methods

        public GridMap2D() { }

        public int GetType(int x, int y)
        {
            if (!IsAvailable(x, y)) return -1;
            return this[x, y].Type;
        }

        public bool HasType(int x, int y, int type)
        {
            if (!IsAvailable(x, y)) return false;
            return (this[x, y].Type & type) > 0;
        }

        public string GetTag(int x, int y)
        {
            if (!IsAvailable(x, y)) return null;
            return this[x, y].Tag;
        }

        public void SetType(int x, int y, int type)
        {
            if (!IsAvailable(x, y)) return;
            this[x, y].Type = type;
        }

        public void SetTag(int x, int y, string tag)
        {
            if (!IsAvailable(x, y)) return;
            this[x, y].Tag = tag;
        }

        public bool IsAvailable(int x, int y)
        {
            return x >= 0 && x < m_colCount && y >= 0 && y < m_rowCount;
        }

        /*
         * cell coordinate to world space position
         * */
        public Vector3 CellToWorldSpacePos(int x, int y)
        {
            Vector3 pos = default(Vector3);
            pos.x = x * m_cellWidth + m_cellWidth * 0.5f - Length * 0.5f + m_offsetInWorldSapce.x;
            pos.z = y * m_cellHeight + m_cellHeight * 0.5f - Height * 0.5f + m_offsetInWorldSapce.z;
            pos.y = m_offsetInWorldSapce.y;
            return pos;
        }

        /*
         * world space position to cell coordinate
         * */
        public IPoint WorldSpaceToCellPos(Vector3 pos)
        {
            IPoint point = new IPoint();
            float x = (pos.x - (-Length * 0.5f + m_offsetInWorldSapce.x)) / m_cellWidth;
            float y = (pos.z - (-Height * 0.5f + m_offsetInWorldSapce.z)) / m_cellHeight;
            point.X = (int)x;
            point.Y = (int)y;
            return point;
        }

        /*
         * resize the array size to match the col and row
         * */
        public void Fit()
        {
            if (m_cells == null)
            {
                m_cells = new List<Cell>(m_colCount * m_rowCount);
            }
            int count = m_rowCount * m_colCount;
            if (m_cells.Count != count)
            {
                while (m_cells.Count < count)
                {
                    m_cells.Add(new Cell());
                }
                while (m_cells.Count > count)
                {
                    m_cells.RemoveAt(m_cells.Count - 1);
                }
            }
        }

        public List<IPoint> FindPath(Vector3 from, Vector3 to, bool ignoreCorners = false)
        {
            IPoint f = WorldSpaceToCellPos(from);
            IPoint t = WorldSpaceToCellPos(to);
            return _AStar.FindPath(f.X, f.Y, t.X, t.Y, 1, ignoreCorners);
        }

        public List<IPoint> FindPath(IPoint from, IPoint to, bool ignoreCorners = false)
        {
            return _AStar.FindPath(from.X, from.Y, to.X, to.Y, 1, ignoreCorners);
        }

        public Cell this[int x, int y]
        {
            get
            {
                return m_cells[y * m_colCount + x];
            }
            set
            {
                m_cells[y * m_colCount + x] = value;
            }
        }

        #endregion

        #region Private Methods

        private void InitAllCell()
        {
            for (int y = 0; y < m_rowCount; ++y)
            {
                for (int x = 0; x < m_colCount; ++x)
                {
                    if (this[x, y] == null)
                    {
                        this[x, y] = new Cell();
                    }
                }
            }
        }

        #endregion
    }

    public enum ECellType : int
    {
        EMPYY = 0x0,
        OBSTACLE = 0x1,
    }
}