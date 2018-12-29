using Framework.AStar;
using Framework.Common;
using Framework.Common.Message;
using Framework.UI;
using Game.Data;
using Game.Entity;
using Game.SLG.Level;
using Game.SLG.System;
using Game.UI.GameHud;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SLG
{
    public enum ESLGSystem
    {
        Map,
        Character,
        Turn,
        Level,
        Battle,
        Count,
    }

    public class SLGGame : Singleton<SLGGame>
    {
        private Transform m_RootTf;
        private GridMap2D m_MapData;
        private TurnAgent m_TurnAgent;
        private Environment m_Environment;

        private IGameSystem[] m_Systems;

        public GridMap2D MapData { get { return m_MapData; } }

        public Environment Environment { get { return m_Environment; } }

        public void Load(Component.MapRes mapRes, Transform rootTf)
        {
            m_Systems = new IGameSystem[(int)ESLGSystem.Count];

            m_RootTf = rootTf;
            m_MapData = mapRes.mapData;

            m_Environment = new Environment();
            m_TurnAgent = new TurnAgent();
            m_TurnAgent.Cursor = new GameCursor(m_RootTf);
            m_TurnAgent.Arrow = new GameArrow(m_RootTf);
            m_TurnAgent.WorldCamera = new WorldCamera(GameObject.Find("GameLoop/Camera").transform);

            RegisterSystem<SLGMapSystem>(ESLGSystem.Map, m_MapData);
            RegisterSystem<SLGCharacterSystem>(ESLGSystem.Character, m_RootTf);
            RegisterSystem<SLGTurnSystem>(ESLGSystem.Turn, m_TurnAgent);
            RegisterSystem<SLGLevelSystem>(ESLGSystem.Level, mapRes.levelConfig);
            RegisterSystem<SLGBattleSystem>(ESLGSystem.Battle);

            Character ch = CS_CreateCharacterAtPoint(ECharacterRelation.OwnSide, 1, new IPoint(12, 2));
            Character ch2 = CS_CreateCharacterAtPoint(ECharacterRelation.OwnSide, 2, new IPoint(14, 2));
            m_TurnAgent.Cursor_SetCellPos(ch.Point);
            m_TurnAgent.WorldCamera_FollowCellPos(ch.Point);

            GameManager.Instance.UIMgr.OpenUI<GameHud>();
            GameManager.Instance.AddInputEvent(OnInputMsg, 0);
            Start();
        }

        public void Start()
        {
            MessageCenter.Instance.SendMessage(WorldMessage.ON_SLGGAME_START, null);
            System<SLGTurnSystem>().SwitchTurn(Common.ETurnType.OwnSide);
        }

        public void Unload()
        {
            foreach (IGameSystem gs in m_Systems)
            {
                gs.OnUninitialize();
            }
        }

        public void OnUpdate()
        {
            foreach (IGameSystem gs in m_Systems)
            {
                gs.OnUpdate();
            }
        }

        public bool OnInputMsg(InputMessage msg)
        {
            Debug.Log(string.Format("world:{0} isdown:{1}", msg.Word, msg.IsDown));
            System<SLGTurnSystem>().OnInputMsg(msg);
            return false;
        }

        public T System<T>() where T : IGameSystem
        {
            foreach (IGameSystem gs in m_Systems)
                if (gs is T)
                    return gs as T;
            return null;
        }

        #region Private Functions

        private void RegisterSystem<T>(ESLGSystem system, params object[] pars) where T : IGameSystem, new()
        {
            m_Systems[(int)system] = GameManager.Instance.CreateSystem<T>(pars);
        }

        #endregion

        #region CharacterSystem Functions

        public Character CS_CreateCharacterAtPoint(ECharacterRelation relation, int id, IPoint point)
        {
            return System<SLGCharacterSystem>().CreateCharacterAtPoint(relation, id, point);
        }

        public void CS_RefreshActions(ECharacterRelation relation)
        {
            System<SLGCharacterSystem>().RefreshActions(relation);
        }

        public int CS_GetCanActCount(ECharacterRelation relation)
        {
            return System<SLGCharacterSystem>().GetCanActCount(relation);
        }

        #endregion

        #region MapSystem Functions

        // 寻路
        public List<IPoint> MAP_FindPath(IPoint from, IPoint to)
        {
            return System<SLGMapSystem>().FindPath(from, to);
        }

        // 添加状态
        public void MAP_AddCellState(IPoint point, int state)
        {
            System<SLGMapSystem>().AddCellState(point, state);
        }

        // 移除状态
        public void MAP_RemoveCellState(IPoint point, int state)
        {
            System<SLGMapSystem>().RemoveCellState(point, state);
        }

        // 是否有状态
        public bool MAP_HasCellState(IPoint point, int state)
        {
            return System<SLGMapSystem>().HasCellState(point, state);
        }

        // 格子坐标转换为世界坐标
        public Vector3 MAP_CellPosToWorldPos(IPoint point)
        {
            return System<SLGMapSystem>().CellPosToWorldPos(point);
        }

        // 显示角色范围
        public void MAP_ShowRangeViewAtPoint(IPoint point, int gid, int locomotivity, int attackDistance)
        {
            System<SLGMapSystem>().ShowRangeViewAtPoint(point, gid, locomotivity, attackDistance);
        }

        // 关闭角色范围显示
        public void MAP_CloseRangeView(int gid)
        {
            System<SLGMapSystem>().CloseRangeView(gid);
        }

        // 角色是否能移动进入
        public bool MAP_CanCharacterMoveOn(IPoint point)
        {
            return System<SLGMapSystem>().CanCharacterMoveOn(point);
        }

        // 从指定位置得到一个Actor
        public T MAP_GetActorAtPoint<T>(IPoint point) where T : Actor
        {
            return System<SLGMapSystem>().GetActorAtPoint<T>(point);
        }

        // 将一个Actor添加到指定位置
        public void MAP_AddActorAtPoint(Actor actor, IPoint point)
        {
            System<SLGMapSystem>().AddActorAtPoint(actor, point);
        }

        // 讲一个Actor从指定位置移除
        public void MAP_RemoveActorAtPoint(Actor actor, IPoint point)
        {
            System<SLGMapSystem>().RemoveActorAtPoint(actor, point);
        }

        #endregion

        #region TurnSystem Functions

        public void Turn_SwitchTurn(Game.Common.ETurnType turn)
        {
            System<SLGTurnSystem>().SwitchTurn(turn);
        }

        #endregion
    }
}
