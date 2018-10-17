using Framework.AStar;
using Framework.Common;
using Game.Common;
using Game.Data;
using Game.Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SLG.System
{
    public class SLGMapCellInfo
    {
        private List<Actor> m_Actors;

        public SLGMapCellInfo()
        {
            m_Actors = new List<Actor>();
        }

        public void AddActor(Actor actor)
        {
            m_Actors.Add(actor);
        }

        public void RemoveActor(Actor actor)
        {
            m_Actors.Remove(actor);
        }

        public T GetActor<T>() where T : Actor
        {
            foreach (Actor actor in m_Actors)
            {
                if (actor is T)
                    return actor as T;
            }
            return null;
        }
    }

    public class SLGMapSystem : IGameSystem
    {
        private GridMap2D m_MapData;
        private Dictionary<int, ActionRangeView> m_RangeViewDic;
        private SLGMapCellInfo[] m_MapCells;

        public SLGMapCellInfo this[int x, int y]
        {
            get
            {
                return m_MapCells[y * m_MapData.m_colCount + x];
            }
            set
            {
                m_MapCells[y * m_MapData.m_colCount + x] = value;
            }
        }

        public override void OnInitialize(IResourceLoader loader, params object[] pars)
        {
            m_MapData = pars[0] as GridMap2D;
            m_MapData.StateCanMove = (x, y) =>
            {
                if (m_MapData.HasState(x, y, GlobalDefines.CELL_STATE_CHAR))
                    return false;
                return true;
            };
            m_RangeViewDic = new Dictionary<int, ActionRangeView>();
            int cellCount = m_MapData.m_colCount * m_MapData.m_rowCount;
            m_MapCells = new SLGMapCellInfo[cellCount];
            for (int i = 0; i < cellCount; ++i)
            {
                m_MapCells[i] = new SLGMapCellInfo();
            }
        }

        public override void OnUpdate()
        {
           
        }

        public override void OnUninitialize()
        {
            m_MapData.ClearAllCellsState();
        }

        // 添加状态
        public void AddCellState(IPoint point, int state)
        {
            m_MapData.AddState(point.X, point.Y, state);
        }

        // 移除状态
        public void RemoveCellState(IPoint point, int state)
        {
            m_MapData.RemoveState(point.X, point.Y, state);
        }

        // 是否有状态
        public bool HasCellState(IPoint point, int state)
        {
            return m_MapData.HasState(point.X, point.Y, state);
        }

        // 格子坐标转换为世界坐标
        public Vector3 CellPosToWorldPos(IPoint point)
        {
            return m_MapData.CellToWorldSpacePos(point.X, point.Y);
        }

        // 角色是否能移动进入
        public bool CanCharacterMoveOn(IPoint point)
        {
            return !m_MapData.HasState(point.X, point.Y, GlobalDefines.CELL_STATE_CHAR);
        }

        // 将一个Actor添加到指定位置
        public void AddActorAtPoint(Actor actor, IPoint point)
        {
            AddCellState(point, actor.GetInCellState());
            this[point.X, point.Y].AddActor(actor);
        }

        // 讲一个Actor从指定位置移除
        public void RemoveActorAtPoint(Actor actor, IPoint point)
        {
            RemoveCellState(point, actor.GetInCellState());
            this[point.X, point.Y].RemoveActor(actor);
        }

        // 从指定位置得到一个Actor
        public T GetActorAtPoint<T>(IPoint point) where T : Actor
        {
            return this[point.X, point.Y].GetActor<T>();
        }

        // 显示角色范围
        public void ShowRangeViewAtPoint(IPoint point, int gid, int locomotivity, int attackDistance)
        {
            Character ch = this[point.X, point.Y].GetActor<Character>();
            if (ch == null)
            {
                Debug.LogError("this cell don't have any characters that x:" + point.X + " y:" + point.Y);
                return;
            }
            ActionRangeView view = null;
            if (!m_RangeViewDic.TryGetValue(gid, out view))
            {
                view = new ActionRangeView();
                m_RangeViewDic.Add(gid, view);
            }
            view.Release();
            ActionRangeData rangeData = new ActionRangeData();
            int cost = 0;
            IPoint p = new IPoint();
            for (int y = - locomotivity - attackDistance - 1; y <= locomotivity + attackDistance + 1; ++y)
            { 
                for (int x = -locomotivity - attackDistance - 1; x <= locomotivity + attackDistance + 1; ++x)
                {
                    p.X = x + point.X;
                    p.Y = y + point.Y;
                    if (!m_MapData.IsAvailable(p.X, p.Y))
                        continue;
                    cost = Mathf.Abs(x)+ Mathf.Abs(y);
                    if (cost <= locomotivity)
                    {
                        //Debug.Log(string.Format("x:{0} y:{1} px:{2} py:{3} cost:{4}", x, y, p.X, p.Y, cost));
                        rangeData.MovingList.Add(new Data.ActionCellData(p.X, p.Y));
                    }
                    else if (cost <= locomotivity + attackDistance)
                    {
                        rangeData.AttackingList.Add(new Data.ActionCellData(p.X, p.Y));
                    }
                }
            }
            view.Create(rangeData, null);
        }

        // 关闭角色范围显示
        public void CloseRangeView(int gid)
        {
            ActionRangeView view = null;
            if (m_RangeViewDic.TryGetValue(gid, out view))
            {
                view.Release();
            }
        }
    }
}