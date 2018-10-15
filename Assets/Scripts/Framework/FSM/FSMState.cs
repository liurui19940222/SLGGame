using Framework.Common.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.FSM
{
    public class FSMState
    {

        private int m_Id;

        public int Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }

        public FSMState(int id) { m_Id = id; }

        public void HandleInput(InputMessage msg) { }

        public virtual void OnEnter(IMessage param = null) { }

        public virtual void OnExit() { }

        public virtual int OnUpdate()
        {
            return m_Id;
        }
    }

}