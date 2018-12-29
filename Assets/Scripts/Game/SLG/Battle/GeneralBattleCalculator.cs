using Game.Config;
using Game.Data;
using UnityEngine;

namespace Game.SLG.Battle
{

    public class GeneralBattleCalculator : IBattleCalculator
    {
        public BattleData Calculate(BattleMsg msg)
        {
            if (msg.attacker == null || msg.defender == null)
                return null;
            BattleProp propA = msg.attacker.BattleProp;
            BattleProp propB = msg.defender.BattleProp;
            BattleData data = new BattleData();
            data.details.Add(new BattleDetailData
            {
                chIndex = 0,
                process = BattleDetailData.Process.Turn,
                value = 0
            });
            data.details.Add(new BattleDetailData
            {
                chIndex = 1,
                process = BattleDetailData.Process.Hurt,
                value = Mathf.Max(0, propA.atk - propB.def),
            });
            if (propA.spd - propB.spd >= 4)
            {

            }
            return data;
        }

        private void AddAttackProcess(int attackerIndex, int defenderIndex, BattleProp attackerProp, BattleProp defenerProp)
        {

        }
    }

}