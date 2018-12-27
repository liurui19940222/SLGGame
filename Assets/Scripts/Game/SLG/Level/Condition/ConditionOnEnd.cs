using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SLG.Level.Condition
{
    public class ConditionOnEnd : ConditionBase
    {
        public override bool Check(Param param, Environment env)
        {
            return true;
        }
    }
}