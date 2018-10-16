using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Entity
{
    public abstract class EntityBase
    {

        protected Transform m_Transform;
        protected GameObject m_GameObject;

        public EntityBase() { }

        public EntityBase(Transform trans)
        {
            m_Transform = trans;
            m_GameObject = trans.gameObject;
        }

        public EntityBase(string path, string name, Transform parent)
        {
            CreateInstance(path, name, parent);
        }

        public EntityBase(Transform trans, Transform parent)
        {
            m_Transform = trans;
            m_GameObject = trans.gameObject;
            trans.SetParent(parent);
        }

        protected void CreateInstance(string path, string name, Transform parent)
        {
            LoadRes(path, name);
            if (m_Transform != null)
                m_Transform.SetParent(parent);
        }

        private void LoadRes(string path, string name)
        {
            m_GameObject = GameManager.Instance.ResLoader.LoadAndInstantiateAsset(path, name);
            if (m_GameObject == null)
                return;
            m_Transform = m_GameObject.transform;
        }

        public void Release()
        {
            GameObject.Destroy(m_GameObject);
            m_GameObject = null;
            m_Transform = null;
        }
    }
}