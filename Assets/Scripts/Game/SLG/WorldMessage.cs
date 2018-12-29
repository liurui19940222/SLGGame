using Framework.Common.Message;
using Game.Entity;
using Game.SLG.System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SLG
{
    public static class WorldMessage
    {
        // 战棋游戏开始
        public const int ON_SLGGAME_START = 0;

        // 回合开始
        public const int ON_TURN_START = 1;

        // 战斗
        public const int BATTLE = 100;
    }

    public class TurnStartMsg : IMessage
    {
        public int turn;

        public ECharacterRelation relation;
    }

    public class BattleMsg : IMessage
    {
        public Character attacker;
        public Character defender;
    }
}
