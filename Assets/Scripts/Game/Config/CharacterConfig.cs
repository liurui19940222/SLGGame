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
    }
}