using System.Collections;
using System.Collections.Generic;
using Framework.AStar;
using Framework.Common.Message;
using Framework.Input;
using Game.Common;
using UnityEngine;

namespace Game.SLG.Turn
{
    public class SLGPlayerTurn : SLGTurn
    {
        private EInputWord m_PressingWord1;
        private EInputWord m_PressingWord2;

        public SLGPlayerTurn(TurnAgent agent) : base(agent, SLGTurn.PLAYER_TURN) { }

        public override void OnEnter(IMessage param = null)
        {
            base.OnEnter(param);
            Debug.Log("player turn enter");
        }

        public override int OnUpdate()
        {
            UpdateCursor();
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

            Debug.Log(string.Format("down current input word:{0}  word1:{1}  word2:{2}", word, m_PressingWord1, m_PressingWord2));
        }

        private void HandleMultipleWordUp(EInputWord word)
        {
            if (m_PressingWord1 == word)
                m_PressingWord1 = EInputWord.NONE;
            if (m_PressingWord2 == word)
                m_PressingWord2 = EInputWord.NONE;

            Debug.Log(string.Format("up current input word:{0}  word1:{1}  word2:{2}", word, m_PressingWord1, m_PressingWord2));
        }

        private void UpdateCursor()
        {
            if (m_Agent.CursorIsMoving() && m_Agent.CursorGetMovingProgress() < GlobalDefines.CURSOR_MOVING_THRESHOLD)
                return;
            IPoint point = GlobalFunctions.GetPointByInputWord(m_PressingWord1, 1);
            point += GlobalFunctions.GetPointByInputWord(m_PressingWord2, 1);
            if (point.X == 0 && point.Y == 0)
                return;
            m_Agent.CursorMove(point);
            m_Agent.WorldCameraFollowCellPos(m_Agent.CursorGetCurPoint());
        }
    }
}