using System.Collections;
using System.Collections.Generic;
using Framework.AStar;
using Framework.Common.Message;
using Framework.Input;
using Game.Common;
using Game.Entity;
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

        public SLGPlayerTurn(TurnAgent agent) : base(agent, SLGTurn.PLAYER_TURN) { }

        public override void OnEnter(IMessage param = null)
        {
            base.OnEnter(param);
            Debug.Log("player turn enter");
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
                
            }

            if (SLG.SLGGame.Instance.MAP_HasCellState(cursorPoint, GlobalDefines.CELL_STATE_CHAR))
            {
                Character ch = SLG.SLGGame.Instance.MAP_GetActorAtPoint<Character>(cursorPoint);
                if (ch.ToggleRangeView())
                {
                    m_CurSelectedCh = ch;
                }
                else
                {
                    m_CurSelectedCh = null;
                    m_Agent.Arrow_Close();
                }
            }
            else
            {
                Debug.Log("打开菜单");
            }
        }

        // 处理取消按钮按下
        private void HandleCancelButtonDown()
        {
            if (m_CurSelectedCh != null)
            {
                m_CurSelectedCh.ShowRangeView(false);
                m_Agent.Arrow_Close();
                m_CurSelectedCh = null;
            }
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
                    List<IPoint> path = SLG.SLGGame.Instance.MAP_FindPath(m_CurSelectedCh.Point, m_Agent.Cursor_GetCurPoint());
                    path.Reverse();
                    m_Agent.Arrow_ShowPath(path);
                }
            }
        }
    }
}