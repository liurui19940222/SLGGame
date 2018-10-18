using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Config
{
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
    }
}