using Framework.AStar;
using Game.Common;
using Game.Component;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Entity
{
    public class WorldCamera : EntityBase
    {
        private Camera m_Camera;
        private Movement m_Movement;
        private GridMap2D m_Map;
        private Rect m_PosRect;

        public WorldCamera(Transform trans) : base(trans)
        {
            m_Camera = m_GameObject.GetComponent<Camera>();
            m_Movement = m_GameObject.AddComponent<Movement>();
            m_Transform.eulerAngles = GlobalDefines.WORLD_CAMERA_EULER;
            m_Camera.orthographicSize = GlobalDefines.WORLD_CAMERA_SIZE;
            m_Map = SLG.SLGGame.Instance.MapData;
            ComputeRange();
        }

        public void LookAtCellPos(IPoint point)
        {
            Vector3 worldPos = m_Map.CellToWorldSpacePos(point.X, point.Y);
            worldPos.y = GlobalDefines.WORLD_CAMERA_Y;
            worldPos.z += GlobalDefines.WORLD_CAMERA_Z_OFFSET;
            m_Transform.position = worldPos;
        }

        public void FollowCellPos(IPoint point)
        {
            Vector3 worldPos = m_Map.CellToWorldSpacePos(point.X, point.Y);
            worldPos = LimitWorldPos(worldPos);
            m_Movement.MoveWithSpeed(worldPos, GlobalDefines.CURSOR_SPEED);
        }

        public Vector3 LimitWorldPos(Vector3 worldPos)
        {
            worldPos.x = Mathf.Clamp(worldPos.x, m_PosRect.xMin, m_PosRect.xMax);
            worldPos.z = Mathf.Clamp(worldPos.z, m_PosRect.yMin, m_PosRect.yMax);

            worldPos.z += GlobalDefines.WORLD_CAMERA_Z_OFFSET;
            worldPos.y = GlobalDefines.WORLD_CAMERA_Y;
            return worldPos;
        }

        private void ComputeRange()
        {
            Rect maxMapRect = m_Map.GetWorldSpaceRect();
            float range_x = m_Camera.orthographicSize * m_Camera.aspect;
            float range_y = m_Camera.orthographicSize;
            m_PosRect.xMin = maxMapRect.xMin - GlobalDefines.WORLD_CAMERA_MAX_OUTOF_RANGE_LEFT + range_x;
            m_PosRect.xMax = maxMapRect.xMax + GlobalDefines.WORLD_CAMERA_MAX_OUTOF_RANGE_RIGHT - range_x;
            m_PosRect.yMin = maxMapRect.yMin - GlobalDefines.WORLD_CAMERA_MAX_OUTOF_RANGE_BOTTOM + range_y;
            m_PosRect.yMax = maxMapRect.yMax + GlobalDefines.WORLD_CAMERA_MAX_OUTOF_RANGE_TOP - range_y;
        }
    }
}