using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Common.Message
{
    public class MessageCenter : Singleton<MessageCenter>
    {
        private Dictionary<int, List<Action<IMessage>>> m_Listeners;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            m_Listeners = new Dictionary<int, List<Action<IMessage>>>();
        }

        public void AddListener(int id, Action<IMessage> call)
        {
            List<Action<IMessage>> list = null;
            if (m_Listeners.ContainsKey(id))
            {
                list = m_Listeners[id];
            }
            else
            {
                list = new List<Action<IMessage>>();
                m_Listeners.Add(id, list);
            }
            list.Add(call);
        }

        public void RemoveListener(int id, Action<IMessage> call)
        {
            if (m_Listeners.ContainsKey(id))
            {
                m_Listeners[id].Remove(call);
            }
        }

        public void SendMessage(int id, IMessage msg)
        {
            List<Action<IMessage>> list = null;
            m_Listeners.TryGetValue(id, out list);
            if (list == null)
                return;
            for (int i = 0; i < list.Count; ++i)
            {
                list[i](msg);
            }
        }
    }
}