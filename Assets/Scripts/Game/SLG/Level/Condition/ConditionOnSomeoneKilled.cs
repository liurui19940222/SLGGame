using Game.SLG.System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SLG.Level.Condition
{
    [CreateAssetMenu(menuName = "SLGGame/Condition/OnSomeoneKilledCondition")]
    public class OnSomeoneKilledConditionParam : Param
    {
        public int characterId;
    }

    public class ConditionOnSomeoneKilled : ConditionBase
    {
        public override bool Check(Param param, Environment env)
        {
            OnSomeoneKilledConditionParam _param = param as OnSomeoneKilledConditionParam;
            return _param.characterId == env.lastKilledId;
        }
    }
}