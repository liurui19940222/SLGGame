using Framework.AStar;
using Game.Common;
using Game.Entity;
using UnityEngine;

public class TurnAgent
{
    private GameCursor m_Cursor;

    private WorldCamera m_WorldCamera;

    public GameCursor Cursor { set { m_Cursor = value; } }

    public WorldCamera WorldCamera { set { m_WorldCamera = value; } }

    public void CursorSetCellPos(IPoint point)
    {
        m_Cursor.SetCellPos(point);
    }

    public void CursorMove(IPoint relative_offset)
    {
        m_Cursor.Move(relative_offset);
    }

    public float CursorGetMovingProgress()
    {
        return m_Cursor.GetMovingProgress();
    }

    public bool CursorIsMoving()
    {
        return m_Cursor.IsMoving();
    }

    public IPoint CursorGetCurPoint()
    {
        return m_Cursor.CurPoint;
    }

    public void WorldCameraLookAtCellPos(IPoint point)
    {
        m_WorldCamera.LookAtCellPos(point);
    }

    public void WorldCameraFollowCellPos(IPoint point)
    {
        m_WorldCamera.FollowCellPos(point);
    }
}
