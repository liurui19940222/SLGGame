using Framework.AStar;
using Framework.Common;
using Game.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SLG.System
{
    public class SLGMapSystem : IGameSystem
    {
        private GridMap2D m_MapData;

        public override void OnInitialize(IResourceLoader loader, params object[] pars)
        {
            m_MapData = pars[0] as GridMap2D;
            m_MapData.StateCanMove = (x, y) =>
            {
                if (m_MapData.IsState(x, y, GlobalDefines.CELL_STATE_CHAR))
                    return false;
                return true;
            };
        }

        public override void OnUpdate()
        {
           
        }

        public override void OnUninitialize()
        {
            m_MapData.ClearAllCellsState();
        }

        public void AddCellState(IPoint point, int state)
        {
            m_MapData.AddState(point.X, point.Y, state);
        }

        public void RemoveCellState(IPoint point, int state)
        {
            m_MapData.RemoveState(point.X, point.Y, state);
        }

        // 角色是否能移动进入
        public bool CanCharacterMoveOn(IPoint point)
        {
            return !m_MapData.IsState(point.X, point.Y, GlobalDefines.CELL_STATE_CHAR);
        }
    }
}