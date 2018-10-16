using Framework.AStar;
using Game.Common;
using Game.Component;
using UnityEngine;

namespace Game.Entity
{
    public class GameCursor : EntityBase
    {
        private GridMap2D m_Map;
        private Movement m_Movement;
        private IPoint m_CurPoint;

        public GameCursor(Transform parent) : base(ResourceLoader.COMP_PATH, "Cursor", parent)
        {
            m_Map = SLG.SLGGame.Instance.MapData;
            m_Movement = m_GameObject.AddComponent<Movement>();
        }

        public IPoint CurPoint { get { return m_CurPoint; } }

        public void SetCellPos(IPoint point)
        {
            m_CurPoint = point;
            SetWorldPos(m_Map.CellToWorldSpacePos(point.X, point.Y));
        }

        public void Move(IPoint relative_offset)
        {
            m_CurPoint += relative_offset;
            m_CurPoint.X = m_Map.GetAvailableX(m_CurPoint.X);
            m_CurPoint.Y = m_Map.GetAvailableY(m_CurPoint.Y);
            Vector3 worldPos = m_Map.CellToWorldSpacePos(m_CurPoint.X, m_CurPoint.Y);
            worldPos.y = GlobalDefines.CURSOR_Y;
            m_Movement.MoveWithSpeed(worldPos, GlobalDefines.CURSOR_SPEED);
        }

        public bool IsMoving()
        {
            return m_Movement.IsMoving;
        }

        public float GetMovingProgress()
        {
            return m_Movement.MovingProgress;
        }

        private void SetWorldPos(Vector3 worldPos)
        {
            m_Transform.position = new Vector3(worldPos.x, GlobalDefines.CURSOR_Y, worldPos.z);
        }
    }
}
