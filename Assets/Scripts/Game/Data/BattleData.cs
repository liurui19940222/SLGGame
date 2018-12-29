using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    public class BattleDetailData
    {
        public enum Process
        {
            Turn,
            Attack,
            Hurt,
            Die,
        }
        public int chIndex;
        public Process process;
        public int value;
    }

    public class BattleData
    {
        public List<BattleDetailData> details = new List<BattleDetailData>();
    }
}