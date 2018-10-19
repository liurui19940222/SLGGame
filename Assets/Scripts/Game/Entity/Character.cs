using Framework.AStar;
using Game.Common;
using Game.Component;
using Game.Config;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Entity
{
    public class Character : Actor
    {
        private CharacterConfig m_ChrConfig;
        private CharacterMovement m_Movement;
        private IPoint m_LastPoint = IPoint.Unavailable;
        private bool m_IsRangeViewShowing;

        public int Locomotivity { get { return m_ChrConfig.Locomotivity; } }

        public IPoint Point { get { return m_LastPoint; } }

        private System.Action<Character> m_OnMoveDone;

        public Character(int id, Transform parent)
        {
            m_ChrConfig = (CharacterConfig)GameManager.Instance.ResLoader.LoadAsset(ResourceLoader.CHAR_CFG_PATH, string.Format("char_{0:D3}", id));
            CreateInstance(ResourceLoader.CHAR_PATH, m_ChrConfig.PrefabName, parent);
            m_Movement = m_GameObject.AddComponent<CharacterMovement>();
            m_Movement.SetOnMoveDoneDelegate(() => {
                if (m_OnMoveDone != null)
                    m_OnMoveDone(this);
            });
        }

        public override int GetInCellState()
        {
            return GlobalDefines.CELL_STATE_CHAR;
        }

        public bool SetCellPos(IPoint point, bool dontUpdateTransformation = false)
        {
            if (!SLG.SLGGame.Instance.MAP_CanCharacterMoveOn(point))
            {
                Debug.LogError("the character can't be standed on this point:" + point.ToString());
                return false;
            }
            if (m_LastPoint != IPoint.Unavailable)
                SLG.SLGGame.Instance.MAP_RemoveActorAtPoint(this, point);
            SLG.SLGGame.Instance.MAP_AddActorAtPoint(this, point);
            if (!dontUpdateTransformation)
            {
                Vector3 worldPos = SLG.SLGGame.Instance.MAP_CellPosToWorldPos(point);
                worldPos.y = GlobalDefines.CHAR_Y;
                m_Transform.position = worldPos;
            }
            m_LastPoint = point;
            return true;
        }

        public bool ToggleRangeView()
        {
            m_IsRangeViewShowing = !m_IsRangeViewShowing;
            ShowRangeView(m_IsRangeViewShowing);
            return m_IsRangeViewShowing;
        }

        public void ShowRangeView(bool isShow, bool onlyAttackingRange = false)
        {
            m_IsRangeViewShowing = isShow;
            if (isShow)
                SLG.SLGGame.Instance.MAP_ShowRangeViewAtPoint(m_LastPoint, GID, onlyAttackingRange ? 0 : m_ChrConfig.Locomotivity, m_ChrConfig.AttackDistance);
            else
                SLG.SLGGame.Instance.MAP_CloseRangeView(GID);
        }

        // 是否完成了行动
        public bool GetIsWorkDid()
        {
            return false;
        }

        // 移动到指定位置
        public void MoveTo(IPoint targetPoint)
        {
            if (SetCellPos(targetPoint, true))
                m_Movement.MoveTo(SLG.SLGGame.Instance.MAP_CellPosToWorldPos(targetPoint), m_ChrConfig.MoveSpeed);
        }

        // 移动到指定位置
        public void MoveTo(Vector3 targetPos)
        {
            m_Movement.MoveTo(targetPos, m_ChrConfig.MoveSpeed);
        }

        // 沿着路径移动
        public void MoveAlongPath(IPoint[] path)
        {
            if (SetCellPos(path[path.Length - 1], true))
            {
                Vector3[] vecList = new Vector3[path.Length];
                Vector3 temp = new Vector3();
                for (int i = 0; i < path.Length; ++i)
                {
                    temp = SLG.SLGGame.Instance.MAP_CellPosToWorldPos(path[i]);
                    temp.y = GlobalDefines.CHAR_Y;
                    vecList[i] = temp;
                }
                m_Movement.MoveAlongPath(vecList, m_ChrConfig.MoveSpeed);
            }
        }

        // 沿着路径移动
        public void MoveAlongPath(Vector3[] path)
        {
            m_Movement.MoveAlongPath(path, m_ChrConfig.MoveSpeed);
        }

        public void SetOnMoveDoneDelegate(System.Action<Character> onMoveDone)
        {
            m_OnMoveDone = onMoveDone;
        }
    }
}
