using Framework.AStar;
using Framework.Common;
using Framework.Common.Message;
using Game.Data;
using Game.Entity;
using Game.SLG.System;
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
            m_TurnAgent.CursorSetCellPos(new IPoint(11, 10));

            m_TurnAgent.WorldCamera = new WorldCamera(GameObject.Find("GameLoop/Camera").transform);

            GameManager.Instance.AddInputEvent(OnInputMsg, 0);
            m_MapSystem = GameManager.Instance.CreateSystem<SLGMapSystem>(m_MapData);
            m_CharacterSystem = GameManager.Instance.CreateSystem<SLGCharacterSystem>(m_RootTf);
            m_TurnSystem = GameManager.Instance.CreateSystem<SLGTurnSystem>(m_TurnAgent);

            //ActionRangeData rangeData = new ActionRangeData();
            //rangeData.MovingList.Add(new Data.ActionCellData(10, 10));
            //rangeData.MovingList.Add(new Data.ActionCellData(11, 10));
            //rangeData.MovingList.Add(new Data.ActionCellData(12, 10));
            //rangeData.AttackingList.Add(new Data.ActionCellData(10, 11));
            //rangeData.AttackingList.Add(new Data.ActionCellData(11, 11));
            //rangeData.AttackingList.Add(new Data.ActionCellData(12, 11));
            //ActionRangeView view = new ActionRangeView();
            //view.Create(rangeData, m_RootTf);

            //Rect rect = m_MapData.GetWorldSpaceRect();
            //Util.DebugCube(new Vector3(rect.xMin, 0, rect.yMin), Vector3.one * 0.3f, Color.red);
            //Util.DebugCube(new Vector3(rect.xMin, 0, rect.yMax), Vector3.one * 0.3f, Color.red);
            //Util.DebugCube(new Vector3(rect.xMax, 0, rect.yMin), Vector3.one * 0.3f, Color.red);
            //Util.DebugCube(new Vector3(rect.xMax, 0, rect.yMax), Vector3.one * 0.3f, Color.red);

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
            m_MapSystem.OnUninitialize();
        }

        public bool OnInputMsg(InputMessage msg)
        {
            Debug.Log(string.Format("world:{0} isdown:{1}", msg.Word, msg.IsDown));
            m_TurnSystem.OnInputMsg(msg);
            return false;
        }

        public Character CS_CreateCharacterAtPoint(ECharacterRelation relation, int id)
        {
            return m_CharacterSystem.CreateCharacterAtPoint(relation, id);
        }

        public void MAP_AddCellState(IPoint point, int state)
        {
            m_MapSystem.AddCellState(point, state);
        }

        public void MAP_RemoveCellState(IPoint point, int state)
        {
            m_MapSystem.RemoveCellState(point, state);
        }

        // 角色是否能移动进入
        public bool MAP_CanCharacterMoveOn(IPoint point)
        {
            return m_MapSystem.CanCharacterMoveOn(point);
        }
    }
}
