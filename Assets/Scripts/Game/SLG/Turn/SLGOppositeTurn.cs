using System.Collections;
using System.Collections.Generic;
using Framework.Common.Message;
using Game.Common;
using Game.SLG.System;
using Game.SLG.Turn.Message;
using Game.UI;
using UnityEngine;

namespace Game.SLG.Turn
{
    public class SLGOppositeTurn : SLGTurn
    {
        public SLGOppositeTurn(TurnAgent agent) : base(agent, TurnDefines.OPPOSITE_TURN)
        {

        }

        public override void OnEnter(IMessage param = null)
        {
            Debug.Log("opposite turn enter");
            base.OnEnter(param);
            CheckCanActCount();
        }

        public override void OnExit()
        {
            Debug.Log("opposite turn exit");
            base.OnExit();
        }

        public override int OnUpdate()
        {
            return base.OnUpdate();
        }

        protected override ETurnType GetNextTurn()
        {
            return ETurnType.OwnSide;
        }

        protected override ECharacterRelation GetChRelation()
        {
            return ECharacterRelation.Opposed;
        }
    }
}