using Game.SLG.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Config
{
    [CreateAssetMenu(menuName = "SLGGame/LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        public int Id;

        public List<LevelEvent> Events;
    }
}