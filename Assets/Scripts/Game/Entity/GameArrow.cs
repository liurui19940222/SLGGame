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
            Close();
        }

        public void ShowPath(List<IPoint> path)
        {
            if (path.Count < 2)
                return;
            Vector3[] worldPosArr = new Vector3[path.Count];
            for (int i = 0; i < path.Count; ++i)
            {
                worldPosArr[i] = SLG.SLGGame.Instance.MAP_CellPosToWorldPos(path[i]);
                worldPosArr[i].y = GlobalDefines.ARROW_Y;
            }
            Vector3 endPos = worldPosArr[worldPosArr.Length - 1];
            worldPosArr[worldPosArr.Length - 1] += (endPos - worldPosArr[worldPosArr.Length - 2]).normalized * 0.2f;
            m_LineRenderer.positionCount = path.Count;
            m_LineRenderer.SetPositions(worldPosArr);
            m_LineRenderer.enabled = true;
            m_HeadTrans.position = endPos;

            Direction dir = GlobalFunctions.GetDirFromPointToAnother(path[path.Count - 2], path[path.Count - 1]);
            switch (dir)
            {
                case Direction.North:
                    m_HeadTrans.eulerAngles = new Vector3(0, 0, 0);
                    break;
                case Direction.South:
                    m_HeadTrans.eulerAngles = new Vector3(0, 180, 0);
                    break;
                case Direction.West:
                    m_HeadTrans.eulerAngles = new Vector3(0, -90, 0);
                    break;
                case Direction.East:
                    m_HeadTrans.eulerAngles = new Vector3(0, 90, 0);
                    break;
            }
            m_HeadTrans.gameObject.SetActive(true);
        }

        public void Close()
        {
            m_LineRenderer.enabled = false;
            m_HeadTrans.gameObject.SetActive(false);
        }
    }
}