﻿using System.Collections;
using System.Collections.Generic;
using Framework.Common.Message;
using Game.Common;
using Game.SLG.System;
using Game.SLG.Turn.Message;
using Game.UI;
using UnityEngine;

namespace Game.SLG.Turn
{
    public class SLGTransitionTurn : SLGTurn
    {
        // 持续时间
        private const float DURATION_TIME = 2.0f;

        private float m_Timer;

        private ETurnType m_TargetTurn;

        public SLGTransitionTurn(TurnAgent agent) : base(agent, TurnDefines.TRANSITION_TURN)
        {

        }

        public override void OnEnter(IMessage param = null)
        {
            Debug.Log("transition turn enter target:" + (param as TransitionTurnMsg).targetTurn);
            base.OnEnter(param);
            m_TargetTurn = (param as TransitionTurnMsg).targetTurn;
            m_Timer = 0.0f;
            GameManager.Instance.UIMgr.SendEvent(UIDefines.ID_SHOW_TURN, new UI.Message.ShowTurnMsg()
            {
                TurnType = m_TargetTurn
            });
        }

        public override void OnExit()
        {
            Debug.Log("transition turn exit");
            base.OnExit();
        }

        public override int OnUpdate()
        {
            m_Timer += Time.deltaTime;
            if (m_Timer >= DURATION_TIME)
            {
                switch (m_TargetTurn)
                {
                    case ETurnType.System:
                        break;
                    case ETurnType.OwnSide:
                        return TurnDefines.PLAYER_TURN;
                    case ETurnType.Friendly:
                        break;
                    case ETurnType.Opposite:
                        return TurnDefines.OPPOSITE_TURN;
                }
            }
            return base.OnUpdate();
        }

        protected override ETurnType GetNextTurn()
        {
            return m_TargetTurn;
        }

        protected override ECharacterRelation GetChRelation()
        {
            return ECharacterRelation.Nothing;
        }
    }
}