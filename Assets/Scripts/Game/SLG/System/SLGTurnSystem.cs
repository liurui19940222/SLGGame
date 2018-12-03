using Framework.Common;
using Framework.Common.Message;
using Framework.FSM;
using Game.SLG.Turn;
using Game.SLG.Turn.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SLG.System
{
    public class SLGTurnSystem : IGameSystem
    {
        private FSMMachine m_TurnFSM;

        public override void OnInitialize(IResourceLoader loader, params object[] pars)
        {
            TurnAgent agent = pars[0] as TurnAgent;
            m_TurnFSM = new FSMMachine();
            m_TurnFSM.AddState(new SLGTransitionTurn(agent));
            m_TurnFSM.AddState(new SLGPlayerTurn(agent));
            m_TurnFSM.AddState(new SLGOppositeTurn(agent));
        }

        public override void OnUpdate()
        {
            m_TurnFSM.OnUpdate();
        }

        public override void OnUninitialize()
        {
            m_TurnFSM.Quit();
        }

        public override void OnInputMsg(InputMessage msg)
        {
            m_TurnFSM.HandleInput(msg);
        }

        public void SwitchTurn(Common.ETurnType turn)
        {
            TransitionTurnMsg msg = new TransitionTurnMsg();
            msg.targetTurn = turn;
            m_TurnFSM.SwitchToState(TurnDefines.TRANSITION_TURN, msg);
        }
    }
}