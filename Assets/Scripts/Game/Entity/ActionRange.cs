using Framework.AStar;
using Game.Common;
using Game.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Entity
{
    public class ActionRangeCellObj
    {
        private GameObject gameObject;
        private Transform transform;
        private Material material;

        public ActionRangeCellObj()
        {
            gameObject = GameManager.Instance.ResLoader.LoadAndInstantiateAsset(ResourceLoader.COMP_PATH, "Cell");
            transform = gameObject.transform;
            material = gameObject.GetComponent<MeshRenderer>().material;
        }

        public void SetPos(float x, float z)
        {
            this.transform.position = new Vector3(x, GlobalDefines.RANGE_CELL_Y, z);
        }

        public void SetParent(Transform parent)
        {
            this.transform.SetParent(parent);
        }

        public void SetAttacking()
        {
            material.color = new Color(1.0f, 0.0f, 0.0f, 0.6f);
        }

        public void SetMoving()
        {
            material.color = new Color(0.0f, 0.0f, 1.0f, 0.6f);
        }

        public void Release()
        {
            GameObject.Destroy(this.gameObject);
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
                ActionRangeCellObj obj = new ActionRangeCellObj();
                obj.SetMoving();
                Vector3 pos = map.CellToWorldSpacePos(cellData.X, cellData.Y);
                obj.SetPos(pos.x, pos.z);
                obj.SetParent(parent);
                m_ObjList.Add(obj);
            }

            foreach (ActionCellData cellData in data.AttackingList)
            {
                ActionRangeCellObj obj = new ActionRangeCellObj();
                obj.SetAttacking();
                Vector3 pos = map.CellToWorldSpacePos(cellData.X, cellData.Y);
                obj.SetPos(pos.x, pos.z);
                obj.SetParent(parent);
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