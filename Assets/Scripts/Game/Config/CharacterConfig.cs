using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Config
{
    [System.Serializable]
    public class BattleProp
    {
        public int hp;      //血量
        public int atk;     //攻击力
        public int def;     //防御力
        public int spd;     //速度
        public int ski;     //技术
        public int lck;     //幸运
    }

    [CreateAssetMenu(menuName = "SLGGame/CharacterConfig")]
    public class CharacterConfig : ScriptableObject
    {
        public int Id;

        public string PrefabName;

        // 移动力
        public int Locomotivity;

        // 攻击距离
        public int AttackDistance;

        // 移动速度
        public float MoveSpeed;

        // 地图中的缩放
        public float ScaleInMap;

        public BattleProp BattleData;
    }
}