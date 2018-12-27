using Game.SLG.System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SLG.Level.Condition
{
    [CreateAssetMenu(menuName = "SLGGame/Condition/OnTurnStartCondition")]
    public class OnTurnStartConditionParam : Param
    {
        public int turn;
        public ECharacterRelation whosTurn;
    }

    public class ConditionOnTurnStart : ConditionBase
    {
        public override bool Check(Param param, Environment env)
        {
            OnTurnStartConditionParam _param = param as OnTurnStartConditionParam;
            return env.curTurn == _param.turn && env.whosTurn == _param.whosTurn;
        }
    }
}