using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Game.SLG.System;

namespace Game.SLG.Level.Action
{
    [CreateAssetMenu(menuName = "SLGGame/Action/StoryActionParam")]
    public class StoryActionParam : Param
    {

    }

    public class StoryAction : ActionBase
    {
        public override void Trigger(Param param)
        {
            StoryActionParam _param = param as StoryActionParam;
        }
    }
}