using Game.SLG.System;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SLG.Level
{
    public class Environment
    {
        public int curTurn;

        public ECharacterRelation whosTurn;

        public int lastKilledId;

        public void Reset()
        {
            curTurn = 0;
            whosTurn = ECharacterRelation.Nothing;
            lastKilledId = 0;
        }
    }

    public enum ECondition
    {
        OnStart,
        OnTurnStart,
        OnSomeoneKilled,
        OnEnd,
    }

    public enum EAction
    {
        Spawn,
        Story,
    }

    public class Param : ScriptableObject
    {

    }

    [Serializable]
    public class LevelEvent
    {
        public ECondition condition;

        public Param conditionParam;

        public EAction action;

        public Param actionParam;
    }
}