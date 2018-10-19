using Framework.Common.Message;
using Game.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SLG.Turn
{
    public static class TurnDefines
    {
        // 玩家回合
        public const int PLAYER_TURN = 1;

        // 敌方回合
        public const int OPPOSITE_TURN = 2;

        // 跳转回合
        public const int TRANSITION_TURN = 3;
    }

    namespace Message
    {

        // Transition状态进入时的传入参数
        public class TransitionTurnMsg : IMessage
        {
            public ETurnType targetTurn;
        }

    }
}