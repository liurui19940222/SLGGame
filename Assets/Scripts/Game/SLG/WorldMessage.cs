using Framework.Common.Message;
using Game.SLG.System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SLG
{
    public static class WorldMessage
    {

        public const int ON_SLGGAME_START = 0;

        public const int ON_TURN_START = 1;

    }

    public class TurnStartMsg : IMessage
    {
        public int turn;

        public ECharacterRelation relation;
    }
}
