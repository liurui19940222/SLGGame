using Framework.AStar;
using Game.Common;
using Game.Component;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Entity
{
    public class GameArrow : EntityBase
    {
        private LineRenderer m_LineRenderer;
        private Transform m_HeadTrans;

        public GameArrow(Transform parent) : base(ResourceLoader.COMP_PATH, "Arrow", parent)
        {
            m_LineRenderer = m_Transform.Find("Line").GetComponent<LineRenderer>();
            m_HeadTrans = m_Transform.Find("Head");

            List<IPoint> list = new List<IPoint>();
            list.Add(new IPoint(11, 10));
            list.Add(new IPoint(11, 11));
            list.Add(new IPoint(11, 12));
            list.Add(new IPoint(11, 13));
            list.Add(new IPoint(11, 14));
            ShowPath(list);
        }

        public void ShowPath(List<IPoint> path)
        {
            Vector3[] worldPosArr = new Vector3[path.Count];
            for (int i = 0; i < path.Count; ++i)
            {
                worldPosArr[i] = SLG.SLGGame.Instance.MAP_CellPosToWorldPos(path[i]);
            }
            m_LineRenderer.SetPositions(worldPosArr);
        }

        public void Close()
        {

        }
    }
}