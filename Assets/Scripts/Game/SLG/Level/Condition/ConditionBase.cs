using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SLG.Level.Condition
{
    public abstract class ConditionBase
    {
        public abstract bool Check(Param param, Environment env);
    }
}