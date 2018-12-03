using UnityEngine;
using System.Collections;
using Framework.FSM;
using Game.Common;
using Game.SLG.Turn.Message;
using Framework.Common.Message;
using Game.SLG.System;

namespace Game.SLG.Turn
{
    public abstract class SLGTurn : FSMState
    {
        protected TurnAgent m_Agent;

        public SLGTurn(TurnAgent agent, int id) : base(id)
        {
            m_Agent = agent;
        }

        // 切换到下一势力回合
        public void SwitchToNextTurn()
        {
            m_Agent.Turn_SwitchTurn(GetNextTurn());
        }

        // 检查可行动角色数量
        public void CheckCanActCount()
        {
            if (SLGGame.Instance.CS_GetCanActCount(GetChRelation()) == 0)
            {
                SwitchToNextTurn();
            }
        }

        protected abstract ECharacterRelation GetChRelation();

        protected abstract ETurnType GetNextTurn();
    }
}