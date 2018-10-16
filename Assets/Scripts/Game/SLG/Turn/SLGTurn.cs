using UnityEngine;
using System.Collections;
using Framework.FSM;

namespace Game.SLG.Turn
{
    public class SLGTurn : FSMState
    {
        public const int PLAYER_TURN = 1;
        public const int OPPOSITE_TURN = 2;

        protected TurnAgent m_Agent;

        public SLGTurn(TurnAgent agent, int id) : base(id)
        {
            m_Agent = agent;
        }
    }
}