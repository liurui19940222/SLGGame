using Framework.Common;
using Framework.Common.Message;
using Framework.FSM;
using Game.SLG.Turn;
using Game.SLG.Turn.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.SLG.Level;
using Game.SLG.Level.Condition;
using Game.SLG.Level.Action;
using Game.Config;
using Game.Entity;

namespace Game.SLG.System
{
    public class SLGBattleSystem : IGameSystem
    {

        public SLGBattleSystem()
        {
   
        }

        public override void OnInitialize(IResourceLoader loader, params object[] pars)
        {
            MessageCenter.Instance.AddListener(WorldMessage.BATTLE, this.OnBattleMsg);
        }

        public override void OnUpdate()
        {

        }

        public override void OnUninitialize()
        {
            MessageCenter.Instance.RemoveListener(WorldMessage.BATTLE, this.OnBattleMsg);
        }

        public override void OnInputMsg(InputMessage msg)
        {

        }

        private void OnBattleMsg(IMessage msg)
        {
            BattleMsg battleMsg = msg as BattleMsg;
            Fight(battleMsg.attacker, battleMsg.defender);
        }

        private void Fight(Character attacker, Character defender)
        {
            
        }
    }
}