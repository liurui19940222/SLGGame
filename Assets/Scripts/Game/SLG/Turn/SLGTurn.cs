using UnityEngine;
using System.Collections;
using Framework.FSM;

namespace Game.SLG.Turn
{
    public class SLGTurn : FSMState
    {
        protected TurnAgent m_Agent;

        public SLGTurn(TurnAgent agent, int id) : base(id)
        {
            m_Agent = agent;
        }
    }
}