using Framework.AStar;
using Game.Common;
using Game.Config;
using UnityEngine;

namespace Game.Entity
{
    public class Character : Actor
    {
        private CharacterConfig m_ChrConfig;
        private IPoint m_LastPoint = IPoint.Unavailable;
        private bool m_IsRangeViewShowing;

        public Character(int id, Transform parent)
        {
            m_ChrConfig = (CharacterConfig)GameManager.Instance.ResLoader.LoadAsset(ResourceLoader.CHAR_CFG_PATH, string.Format("char_{0:D3}", id));
            CreateInstance(ResourceLoader.CHAR_PATH, m_ChrConfig.PrefabName, parent);
        }

        public override int GetInCellState()
        {
            return GlobalDefines.CELL_STATE_CHAR;
        }

        public void SetCellPos(IPoint point)
        {
            if (!SLG.SLGGame.Instance.MAP_CanCharacterMoveOn(point))
            {
                Debug.LogError("the character can't be standed on this point:" + point.ToString());
                return;
            }
            if (m_LastPoint != IPoint.Unavailable)
                SLG.SLGGame.Instance.MAP_RemoveActorAtPoint(this, point);
            SLG.SLGGame.Instance.MAP_AddActorAtPoint(this, point);
            Vector3 worldPos = SLG.SLGGame.Instance.MAP_CellPosToWorldPos(point);
            worldPos.y = GlobalDefines.CHAR_Y;
            m_Transform.position = worldPos;
            m_LastPoint = point;
        }

        public void ToggleRangeView()
        {
            m_IsRangeViewShowing = !m_IsRangeViewShowing;
            ShowRangeView(m_IsRangeViewShowing);
        }

        public void ShowRangeView(bool isShow)
        {
            m_IsRangeViewShowing = isShow;
            if (isShow)
                SLG.SLGGame.Instance.MAP_ShowRangeViewAtPoint(m_LastPoint, GID, m_ChrConfig.Locomotivity, m_ChrConfig.AttackDistance);
            else
                SLG.SLGGame.Instance.MAP_CloseRangeView(GID);
        }
    }
}
