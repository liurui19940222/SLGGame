using Framework.AStar;
using Game.Common;
using Game.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Entity
{
    public class ActionRangeCellObj : EntityBase
    {
        private Material material;

        public ActionRangeCellObj(Transform parent) : base(ResourceLoader.COMP_PATH, "Cell", parent)
        {
            material = m_GameObject.GetComponent<MeshRenderer>().material;
        }

        public void SetWorldPos(Vector3 worldPos)
        {
            m_Transform.position = new Vector3(worldPos.x, GlobalDefines.RANGE_CELL_Y, worldPos.z);
        }

        public void SetAttacking()
        {
            material.color = new Color(1.0f, 0.0f, 0.0f, 0.6f);
        }

        public void SetMoving()
        {
            material.color = new Color(0.0f, 0.0f, 1.0f, 0.6f);
        }
    }

    public class ActionRangeView
    {
        private List<ActionRangeCellObj> m_ObjList;

        public ActionRangeView()
        {

        }

        public void Create(ActionRangeData data, Transform parent)
        {
            GridMap2D map = SLG.SLGGame.Instance.MapData;
            m_ObjList = new List<ActionRangeCellObj>();
            foreach (ActionCellData cellData in data.MovingList)
            {
                ActionRangeCellObj obj = new ActionRangeCellObj(parent);
                obj.SetMoving();
                Vector3 pos = map.CellToWorldSpacePos(cellData.X, cellData.Y);
                obj.SetWorldPos(pos);
                m_ObjList.Add(obj);
            }

            foreach (ActionCellData cellData in data.AttackingList)
            {
                ActionRangeCellObj obj = new ActionRangeCellObj(parent);
                obj.SetAttacking();
                Vector3 pos = map.CellToWorldSpacePos(cellData.X, cellData.Y);
                obj.SetWorldPos(pos);
                m_ObjList.Add(obj);
            }
        }

        public void Release()
        {
            foreach (ActionRangeCellObj obj in m_ObjList)
            {
                obj.Release();
            }
            m_ObjList.Clear();
        }
    }

}