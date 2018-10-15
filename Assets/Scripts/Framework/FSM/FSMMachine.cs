using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Common.Message;

namespace Framework.FSM
{

    public class FSMMachine
    {

        private Dictionary<int, FSMState> m_States;

        private FSMState m_DefaultState;

        protected FSMState m_CurState;

        public FSMMachine()
        {
            m_States = new Dictionary<int, FSMState>();
            OnEnter();
        }

        public void AddState(FSMState state)
        {
            if (m_States.ContainsKey(state.Id))
                m_States[state.Id] = state;
            else
                m_States.Add(state.Id, state);
        }

        public FSMState GetState(int id)
        {
            if (m_States.ContainsKey(id))
                return m_States[id];
            return null;
        }

        public void RemoveState(int id)
        {
            if (m_States.ContainsKey(id))
                m_States.Remove(id);
        }

        public void SetAsDefaultState(FSMState state)
        {
            AddState(state);
            m_DefaultState = state;
        }

        public void SetDefaultState(int id)
        {
            m_DefaultState = GetState(id);
        }

        public void SwitchToState(int id, IMessage param)
        {
            if (m_CurState != null && m_CurState.Id != id)
            {
                m_CurState.OnExit();
            }
            m_CurState = GetState(id);
            m_CurState.OnEnter(param);
        }

        public void HandleInput(InputMessage msg)
        {
            if (m_CurState != null)
                m_CurState.HandleInput(msg);
        }

        public virtual void OnUpdate()
        {
            if (m_CurState == null)
            {
                m_CurState = m_DefaultState;
                m_CurState.OnEnter();
            }

            if (m_CurState != null)
            {
                int targetId = m_CurState.OnUpdate();
                if (targetId != m_CurState.Id)
                {
                    SwitchToState(targetId, null);
                }
            }
        }

        public void Quit()
        {
            if (m_CurState != null)
                m_CurState.OnExit();
            OnExit();
        }

        protected virtual void OnEnter() { }

        protected virtual void OnExit() { }
    }
}