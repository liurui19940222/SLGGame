using System.Collections;
using System.Collections.Generic;
using Framework.AStar;
using Framework.Common.Message;
using Framework.Input;
using Game.Common;
using Game.Entity;
using Game.SLG.System;
using Game.UI;
using Game.UI.Message;
using UnityEngine;

namespace Game.SLG.Turn
{
    public class SLGPlayerTurn : SLGTurn
    {
        private EInputWord m_PressingWord1;
        private EInputWord m_PressingWord2;
        private float m_PressMovingWordTime = 0.0f;

        // 当前光标选中的角色
        private Character m_CurSelectedCh;
        private List<IPoint> m_CursorPath;

        // 角色移动之前的位置
        private IPoint m_ChLastPoint = IPoint.Unavailable;

        public SLGPlayerTurn(TurnAgent agent) : base(agent, TurnDefines.PLAYER_TURN) { }

        public override void OnEnter(IMessage param = null)
        {
            base.OnEnter(param);
            Debug.Log("player turn enter");
            SLGGame.Instance.CS_RefreshActions(System.ECharacterRelation.OwnSide);
            MessageCenter.Instance.AddListener(UIDefines.ID_MENU_SELECTED, this.OnMenuSelected);
        }

        public override int OnUpdate()
        {
            UpdateCursor(false);
            return base.OnUpdate();
        }

        public override void OnExit()
        {
            base.OnExit();
            Debug.Log("player turn exit");

            MessageCenter.Instance.RemoveListener(UIDefines.ID_MENU_SELECTED, this.OnMenuSelected);
        }

        public override void HandleInput(InputMessage msg)
        {
            Debug.Log(string.Format("player turn handle input world:{0} isdown:{1}", msg.Word, msg.IsDown));
            if (HandleInputCursor(msg))
                return;

        }

        private bool HandleInputCursor(InputMessage msg)
        {
            switch (msg.Word)
            {
                case EInputWord.DPAD_UP:
                case EInputWord.DPAD_DOWN:
                case EInputWord.DPAD_LEFT:
                case EInputWord.DPAD_RIGHT:
                    if (msg.IsDown)
                        HandleMultipleWordDown(msg.Word);
                    else
                        HandleMultipleWordUp(msg.Word);
                    return true;
                case EInputWord.A:
                    if (msg.IsDown)
                        HandleConfirmButtonDown();
                    return true;
                case EInputWord.B:
                    if (msg.IsDown)
                        HandleCancelButtonDown();
                    return true;
            }

            return false;
        }

        private void HandleMultipleWordDown(EInputWord word)
        {
            if (m_PressingWord1 == EInputWord.NONE)
                m_PressingWord1 = word;
            else if (m_PressingWord2 == EInputWord.NONE)
                m_PressingWord2 = word;
            else if (m_PressingWord1 != word)
                m_PressingWord1 = word;
            else if (m_PressingWord2 != word)
                m_PressingWord2 = word;

            if (m_PressMovingWordTime == 0.0f)
                m_PressMovingWordTime = Time.time;
            UpdateCursor(true);
            Debug.Log(string.Format("down current input word:{0}  word1:{1}  word2:{2}", word, m_PressingWord1, m_PressingWord2));
        }

        private void HandleMultipleWordUp(EInputWord word)
        {
            if (m_PressingWord1 == word)
                m_PressingWord1 = EInputWord.NONE;
            if (m_PressingWord2 == word)
                m_PressingWord2 = EInputWord.NONE;

            if (m_PressingWord1 == m_PressingWord2 && m_PressingWord1 == EInputWord.NONE)
                m_PressMovingWordTime = 0.0f;
            Debug.Log(string.Format("up current input word:{0}  word1:{1}  word2:{2}", word, m_PressingWord1, m_PressingWord2));
        }

        // 处理确定按钮按下
        private void HandleConfirmButtonDown()
        {
            IPoint cursorPoint = m_Agent.Cursor_GetCurPoint();

            // 如果已经选中角色，判断当前光标指定的格子能否进入
            if (m_CurSelectedCh != null && SLG.SLGGame.Instance.MAP_CanCharacterMoveOn(cursorPoint))
            {
                if (IPoint.DistanceWithoutSlope(m_CurSelectedCh.Point, cursorPoint) <= m_CurSelectedCh.Locomotivity)
                {
                    m_ChLastPoint = m_CurSelectedCh.Point;
                    m_CurSelectedCh.MoveAlongPath(m_CursorPath.ToArray());
                    m_CurSelectedCh.SetOnMoveDoneDelegate(OnCharacterMoveDone);
                    return;
                }
            }

            if (SLG.SLGGame.Instance.MAP_HasCellState(cursorPoint, GlobalDefines.CELL_STATE_CHAR))
            {
                Character ch = SLG.SLGGame.Instance.MAP_GetActorAtPoint<Character>(cursorPoint);
                if (ch.HasAction)
                {
                    if (ch.ToggleRangeView())
                    {
                        m_CurSelectedCh = ch;
                    }
                    else
                    {
                        m_CurSelectedCh = null;
                        m_Agent.Arrow_Close();
                        m_CursorPath = null;
                    }
                }
                else
                {
                    OpenMenu();
                }
            }
            else
            {
                OpenMenu();
            }
        }

        // 处理取消按钮按下
        private void HandleCancelButtonDown()
        {
            if (m_CurSelectedCh != null)
            {
                if (m_CurSelectedCh.HasAction && m_ChLastPoint != IPoint.Unavailable)
                {
                    m_Agent.Cursor_SetCellPos(m_ChLastPoint);
                    m_Agent.WorldCamera_FollowCellPos(m_ChLastPoint);
                }
                m_CurSelectedCh.ShowRangeView(false);
                m_Agent.Arrow_Close();
                m_CurSelectedCh = null;
                m_CursorPath = null;
            }
        }

        // 打开菜单
        private void OpenMenu()
        {
            Debug.Log("打开菜单");
        }

        private void UpdateCursor(bool ignoreTime)
        {
            if (m_Agent.Cursor_IsMoving() && m_Agent.Cursor_GetMovingProgress() < GlobalDefines.CURSOR_MOVING_THRESHOLD)
                return;
            if (!ignoreTime && Time.time - m_PressMovingWordTime < GlobalDefines.CURSOR_HOLDON_THRESHOLD)
                return;
            IPoint point = GlobalFunctions.GetPointByInputWord(m_PressingWord1, 1);
            point += GlobalFunctions.GetPointByInputWord(m_PressingWord2, 1);
            if (point.X == 0 && point.Y == 0)
                return;
            m_Agent.Cursor_Move(point);
            m_Agent.WorldCamera_FollowCellPos(m_Agent.Cursor_GetCurPoint());

            if (m_CurSelectedCh != null && !m_CurSelectedCh.GetIsWorkDid())
            {
                if (IPoint.DistanceWithoutSlope(m_CurSelectedCh.Point, m_Agent.Cursor_GetCurPoint()) <= m_CurSelectedCh.Locomotivity)
                {
                    m_CursorPath = SLG.SLGGame.Instance.MAP_FindPath(m_CurSelectedCh.Point, m_Agent.Cursor_GetCurPoint());
                    m_CursorPath.Reverse();
                    m_Agent.Arrow_ShowPath(m_CursorPath);
                }
            }
        }

        // 当角色移动完成
        private void OnCharacterMoveDone(Character ch)
        {
            if (ch == m_CurSelectedCh)
            {
                GameManager.Instance.UIMgr.SendEvent(UIDefines.ID_SHOW_ACTION_MENU, null);
                m_Agent.Arrow_Close();
                ch.ShowRangeView(false);
            }
        }

        // 当取消操作选单
        private void OnMenuSelected(IMessage imsg)
        {
            if (this.m_CurSelectedCh == null)
                return;
            MenuSelectedMsg msg = imsg as MenuSelectedMsg;
            switch (msg.option)
            {
                case UI.GameHud.EActionMenuOption.Cancel:
                    {
                        SLG.SLGGame.Instance.MAP_RemoveActorAtPoint(m_CurSelectedCh, m_CurSelectedCh.Point);
                        m_CurSelectedCh.SetCellPos(m_ChLastPoint, false, true);
                        m_CurSelectedCh.ShowRangeView(true);
                        m_Agent.Arrow_ShowPath(m_CursorPath);
                    }
                    break;
                case UI.GameHud.EActionMenuOption.Attack:
                    break;
                case UI.GameHud.EActionMenuOption.Skill:
                    break;
                case UI.GameHud.EActionMenuOption.Standby:
                    {
                        m_CurSelectedCh.Done();
                        m_CurSelectedCh = null;
                        m_CursorPath = null;
                        m_ChLastPoint = IPoint.Unavailable;
                        CheckCanActCount();
                    }
                    break;
            }
        }

        protected override ETurnType GetNextTurn()
        {
            return ETurnType.Opposite;
        }

        protected override ECharacterRelation GetChRelation()
        {
            return ECharacterRelation.OwnSide;
        }
    }
}