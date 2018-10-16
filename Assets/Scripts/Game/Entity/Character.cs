using Framework.AStar;
using Game.Common;
using Game.Config;
using UnityEngine;

namespace Game.Entity
{
    public class Character : EntityBase
    {
        private CharacterConfig m_Config;
        private IPoint m_LastPoint = IPoint.Unavailable;

        public Character(int id, Transform parent)
        {
            m_Config = (CharacterConfig)GameManager.Instance.ResLoader.LoadAsset(ResourceLoader.CHAR_CFG_PATH, string.Format("char_{0:D3}", id));
            CreateInstance(ResourceLoader.CHAR_PATH, m_Config.PrefabName, parent);
        }

        public void SetCellPos(IPoint point)
        {
            if (!SLG.SLGGame.Instance.MAP_CanCharacterMoveOn(point))
            {
                Debug.LogError("the character can't be standed on this point:" + point.ToString());
                return;
            }
            if (m_LastPoint != IPoint.Unavailable)
                SLG.SLGGame.Instance.MAP_RemoveCellState(m_LastPoint, GlobalDefines.CELL_STATE_CHAR);
            SLG.SLGGame.Instance.MAP_AddCellState(point, GlobalDefines.CELL_STATE_CHAR);
            
            m_LastPoint = point;
        }
    }
}
