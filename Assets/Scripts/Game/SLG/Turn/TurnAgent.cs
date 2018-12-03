using Framework.AStar;
using Game.Common;
using Game.Entity;
using Game.SLG.System;
using System.Collections.Generic;
using UnityEngine;

public class TurnAgent
{
    private GameCursor m_Cursor;

    private GameArrow m_Arrow;

    private WorldCamera m_WorldCamera;

    private SLGTurnSystem m_TurnSystem;

    public GameCursor Cursor { set { m_Cursor = value; } }

    public GameArrow Arrow { set { m_Arrow = value; } }

    public WorldCamera WorldCamera { set { m_WorldCamera = value; } }

    public SLGTurnSystem TurnSystem { set { m_TurnSystem = value; } }

    #region Cursor Functions

    public void Cursor_SetCellPos(IPoint point)
    {
        m_Cursor.SetCellPos(point);
    }

    public void Cursor_Move(IPoint relative_offset)
    {
        m_Cursor.Move(relative_offset);
    }

    public float Cursor_GetMovingProgress()
    {
        return m_Cursor.GetMovingProgress();
    }

    public bool Cursor_IsMoving()
    {
        return m_Cursor.IsMoving();
    }

    public IPoint Cursor_GetCurPoint()
    {
        return m_Cursor.CurPoint;
    }

    #endregion

    #region Arrow Functions

    public void Arrow_ShowPath(List<IPoint> path)
    {
        m_Arrow.ShowPath(path);
    }

    public void Arrow_Close()
    {
        m_Arrow.Close();
    }

    #endregion

    #region WorldCamera Functions

    public void WorldCamera_LookAtCellPos(IPoint point)
    {
        m_WorldCamera.LookAtCellPos(point);
    }

    public void WorldCamera_FollowCellPos(IPoint point)
    {
        m_WorldCamera.FollowCellPos(point);
    }

    #endregion

    #region TurnSystem Functions

    public void Turn_SwitchTurn(Game.Common.ETurnType turn)
    {
        m_TurnSystem.SwitchTurn(turn);
    }

    #endregion
}
