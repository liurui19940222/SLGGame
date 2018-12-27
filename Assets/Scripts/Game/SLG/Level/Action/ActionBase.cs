using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SLG.Level.Action
{
    public abstract class ActionBase
    {
        public abstract void Trigger(Param param);
    }
}