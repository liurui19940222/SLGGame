using Game.Common;
using Framework.Common.Message;
using Framework.FSM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Scene
{
    public abstract class BaseScene : FSMState
    {
        private bool m_HasLoaded = false;

        private Coroutine m_CurCoroutine;

        private Transform m_RootTf;
        private GameObject m_RootGo;

        public BaseScene(int id) : base(id)
        {
        }

        public override void OnEnter(IMessage param = null)
        {
            base.OnEnter();
            m_HasLoaded = false;
            m_RootGo = new GameObject(GetName());
            m_RootTf = m_RootGo.transform;
            m_CurCoroutine = GlobalMono.Instance.StartCoroutine(Initialize(param));
        }

        public override void OnExit()
        {
            base.OnExit();
            if (m_CurCoroutine != null)
            {
                if (GlobalMono.Instance != null)
                {
                    GlobalMono.Instance.StopCoroutine(m_CurCoroutine);
                }
                m_CurCoroutine = null;
            }
            m_HasLoaded = false;
            OnSceneUnload();
            GameObject.Destroy(m_RootGo);
            m_RootGo = null;
            m_RootTf = null;
        }

        public override int OnUpdate()
        {
            if (!m_HasLoaded)
                return base.OnUpdate();
            OnSceneUpdate();
            return base.OnUpdate();
        }

        private IEnumerator Initialize(IMessage param)
        {
            yield return OnInitialize(param);
            m_HasLoaded = true;
            OnSceneInitialized();
        }

        protected virtual IEnumerator OnInitialize(IMessage param) {
            yield return null;
        }

        protected Transform GetRootTf() { return m_RootTf; }

        protected GameObject GetRootGo() { return m_RootGo; }

        protected abstract void OnSceneInitialized();

        protected abstract void OnSceneUpdate();

        protected abstract void OnSceneUnload();

        protected abstract string GetName();
    }
}
