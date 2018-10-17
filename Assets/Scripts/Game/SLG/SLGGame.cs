using Framework.AStar;
using Framework.Common;
using Framework.Common.Message;
using Game.Data;
using Game.Entity;
using Game.SLG.System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SLG
{
    public class SLGGame : Singleton<SLGGame>
    {
        private Transform m_RootTf;
        private GridMap2D m_MapData;
        private SLGCharacterSystem m_CharacterSystem;
        private SLGTurnSystem m_TurnSystem;
        private SLGMapSystem m_MapSystem;
        private TurnAgent m_TurnAgent;

        public GridMap2D MapData { get { return m_MapData; } }

        public void Load(GridMap2D mapData, Transform rootTf)
        {
            m_RootTf = rootTf;
            m_MapData = mapData;

            m_TurnAgent = new TurnAgent();
            m_TurnAgent.Cursor = new GameCursor(m_RootTf);
            m_TurnAgent.Corsor_SetCellPos(new IPoint(11, 10));

            m_TurnAgent.Arrow = new GameArrow(m_RootTf);

            m_TurnAgent.WorldCamera = new WorldCamera(GameObject.Find("GameLoop/Camera").transform);

            GameManager.Instance.AddInputEvent(OnInputMsg, 0);
            m_MapSystem = GameManager.Instance.CreateSystem<SLGMapSystem>(m_MapData);
            m_CharacterSystem = GameManager.Instance.CreateSystem<SLGCharacterSystem>(m_RootTf);
            m_TurnSystem = GameManager.Instance.CreateSystem<SLGTurnSystem>(m_TurnAgent);

            Character ch = CS_CreateCharacterAtPoint(ECharacterRelation.OwnSide, 1);
            ch.SetCellPos(new IPoint(11, 10));
        }

        public void Unload()
        {
            m_TurnSystem.OnUninitialize();
            m_MapSystem.OnUninitialize();
        }

        public void OnUpdate()
        {
            m_TurnSystem.OnUpdate();
            m_MapSystem.OnUpdate();
        }

        public bool OnInputMsg(InputMessage msg)
        {
            Debug.Log(string.Format("world:{0} isdown:{1}", msg.Word, msg.IsDown));
            m_TurnSystem.OnInputMsg(msg);
            return false;
        }

        #region CharacterSystem Functions

        public Character CS_CreateCharacterAtPoint(ECharacterRelation relation, int id)
        {
            return m_CharacterSystem.CreateCharacterAtPoint(relation, id);
        }

        #endregion

        #region MapSystem Functions

        // 添加状态
        public void MAP_AddCellState(IPoint point, int state)
        {
            m_MapSystem.AddCellState(point, state);
        }

        // 移除状态
        public void MAP_RemoveCellState(IPoint point, int state)
        {
            m_MapSystem.RemoveCellState(point, state);
        }

        // 是否有状态
        public bool MAP_HasCellState(IPoint point, int state)
        {
            return m_MapSystem.HasCellState(point, state);
        }

        // 格子坐标转换为世界坐标
        public Vector3 MAP_CellPosToWorldPos(IPoint point)
        {
            return m_MapSystem.CellPosToWorldPos(point);
        }

        // 显示角色范围
        public void MAP_ShowRangeViewAtPoint(IPoint point, int gid, int locomotivity, int attackDistance)
        {
            m_MapSystem.ShowRangeViewAtPoint(point, gid, locomotivity, attackDistance);
        }

        // 关闭角色范围显示
        public void MAP_CloseRangeView(int gid)
        {
            m_MapSystem.CloseRangeView(gid);
        }

        // 角色是否能移动进入
        public bool MAP_CanCharacterMoveOn(IPoint point)
        {
            return m_MapSystem.CanCharacterMoveOn(point);
        }

        // 从指定位置得到一个Actor
        public T MAP_GetActorAtPoint<T>(IPoint point) where T : Actor
        {
            return m_MapSystem.GetActorAtPoint<T>(point);
        }

        // 将一个Actor添加到指定位置
        public void MAP_AddActorAtPoint(Actor actor, IPoint point)
        {
            m_MapSystem.AddActorAtPoint(actor, point);
        }

        // 讲一个Actor从指定位置移除
        public void MAP_RemoveActorAtPoint(Actor actor, IPoint point)
        {
            m_MapSystem.RemoveActorAtPoint(actor, point);
        }

        #endregion
    }
}
