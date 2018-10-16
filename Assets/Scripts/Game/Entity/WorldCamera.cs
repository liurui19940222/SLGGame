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
        private Rect m_MaxMapRect;

        public WorldCamera(Transform trans) : base(trans)
        {
            m_Camera = m_GameObject.GetComponent<Camera>();
            m_Movement = m_GameObject.AddComponent<Movement>();
            m_Transform.eulerAngles = GlobalDefines.WORLD_CAMERA_EULER;
            m_Camera.orthographicSize = GlobalDefines.WORLD_CAMERA_SIZE;
            m_Map = SLG.SLGGame.Instance.MapData;
            m_MaxMapRect = m_Map.GetWorldSpaceRect();
        }

        public void LookAtCellPos(IPoint point)
        {
            Vector3 worldPos = m_Map.CellToWorldSpacePos(point.X, point.Y);
            worldPos.y = GlobalDefines.WORLD_CAMERA_Y;
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
            float range_x = m_Camera.orthographicSize * m_Camera.aspect;
            float range_y = m_Camera.orthographicSize;
            float xMin = worldPos.x - range_x - GlobalDefines.WORLD_CAMERA_MAX_OUTOF_RANGE;
            float xMax = worldPos.x + range_x + GlobalDefines.WORLD_CAMERA_MAX_OUTOF_RANGE;
            float yMin = worldPos.z - range_y - GlobalDefines.WORLD_CAMERA_MAX_OUTOF_RANGE;
            float yMax = worldPos.z + range_y + GlobalDefines.WORLD_CAMERA_MAX_OUTOF_RANGE;

            if (xMin < m_MaxMapRect.xMin)
                worldPos.x = m_MaxMapRect.xMin - GlobalDefines.WORLD_CAMERA_MAX_OUTOF_RANGE + range_x;
            else if (xMax > m_MaxMapRect.xMax)
                worldPos.x = m_MaxMapRect.xMax + GlobalDefines.WORLD_CAMERA_MAX_OUTOF_RANGE - range_x;

            if (yMin < m_MaxMapRect.yMin)
                worldPos.z = m_MaxMapRect.yMin - GlobalDefines.WORLD_CAMERA_MAX_OUTOF_RANGE + range_y;
            else if (yMax > m_MaxMapRect.yMax)
                worldPos.z = m_MaxMapRect.yMax + GlobalDefines.WORLD_CAMERA_MAX_OUTOF_RANGE - range_y;

            worldPos.y = GlobalDefines.WORLD_CAMERA_Y;
            return worldPos;
        }
    }
}