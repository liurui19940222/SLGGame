using Framework.AStar;
using Game.Common;
using Game.Component;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Entity
{
    public class GameArrow : EntityBase
    {
        private GameObject[] m_Parts;

        private List<GameObject> m_PathGos;

        public GameArrow(Transform parent) : base(ResourceLoader.COMP_PATH, "Arrow", parent)
        {
            m_PathGos = new List<GameObject>();
            m_Parts = new GameObject[4];
            for (int i = 0; i < m_Parts.Length; ++i)
            {
                m_Parts[i] = m_Transform.Find("arrow_0" + (i + 1)).gameObject;
                m_Parts[i].SetActive(false);
            }
        }

        public void ShowPath(List<IPoint> path)
        {
            if (path.Count < 2)
                return;
            for (int i = 0; i < path.Count; ++i)
            {

                Vector3 worldPos = SLG.SLGGame.Instance.MapData.CellToWorldSpacePos(path[i].X, path[i].Y);
            }
            
        }

        public void Close()
        {
            for (int i = 0; i < m_PathGos.Count; ++i)
            {
                GameObject.Destroy(m_PathGos[i]);
            }
            m_PathGos.Clear();
        }
    }
}