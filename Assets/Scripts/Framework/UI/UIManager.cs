using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Framework.Common;
using Framework.Common.Message;
using Framework.Input;

namespace Framework.UI
{
    public class UIManager : IGameSystem
    {
        private Transform m_RootTrans;
        private GameObject m_RootGo;
        private Camera m_Camera;
        private Canvas m_Canvas;

        private List<IUserInterface> m_GroupUIs;
        private List<IUserInterface> m_SingleUIs;
        private List<IUserInterface> m_HUDs;

        private IUserInterface m_ActivatedUI;

        private IResourceLoader m_ResLoader;

        public Camera Camera
        {
            get { return m_Camera; }
        }

        public override void OnInitialize(IResourceLoader loader)
        {
            m_ResLoader = loader;
            m_RootGo = GameObject.Instantiate(loader.LoadAsset("UI/", "UIRoot")) as GameObject;
            m_RootTrans = m_RootGo.transform;
            m_Camera = m_RootGo.GetComponentInChildren<Camera>();
            m_Canvas = m_RootGo.GetComponentInChildren<Canvas>();

            m_GroupUIs = new List<IUserInterface>();
            m_SingleUIs = new List<IUserInterface>();
            m_HUDs = new List<IUserInterface>();

            //注册监听按钮事件
            InputSystem.Instance.AddInputEvent(HandleInput, InputConst.INPUT_PRIORITY_UI);
        }

        public override void OnUninitialize()
        {
            InputSystem.Instance.RemoveInputEvent(HandleInput);
        }

        public override void OnUpdate()
        {
            m_GroupUIs.ForEach((ui) => { ui.OnUpdate(); });
            m_SingleUIs.ForEach((ui) => { ui.OnUpdate(); });
            m_HUDs.ForEach((ui) => { ui.OnUpdate(); });
        }

        //打开UI
        public T OpenUI<T>() where T : IUserInterface, new()
        {
            IUserInterface ui = new T();
            GameObject go = GameObject.Instantiate(m_ResLoader.LoadUIAsset(ui.PrefabName)) as GameObject;
            Transform trans = go.transform;
            trans.SetParent(m_RootTrans);
            if (ui.Type == EUIType.GroupUI)
            {
                trans.SetAsLastSibling();
                m_GroupUIs.Add(ui);
                SetActivate(ui);
            }
            else if (ui.Type == EUIType.SingleUI)
            {
                m_SingleUIs.Add(ui);
            }
            else if (ui.Type == EUIType.HUD)
            {
                m_HUDs.Add(ui);
            }
            Util.Invoke(ui, "Initialize", go);
            return ui as T;
        }

        //关闭UI
        public void CloseUI(IUserInterface ui)
        {
            if (ui.Type == EUIType.GroupUI)
            {
                m_GroupUIs.Remove(ui);
            }
            else if (ui.Type == EUIType.SingleUI)
            {
                m_SingleUIs.Remove(ui);
            }
            else if (ui.Type == EUIType.HUD)
            {
                m_HUDs.Remove(ui);
            }
            Util.Invoke(ui, "OnDestory");
            if (m_ActivatedUI == ui)
                m_ActivatedUI = null;
            GameObject.Destroy(ui.GameObject);
        }

        //关闭所有GroupUI
        public void CloseAllGroupUI()
        {
            while (m_GroupUIs.Count > 0)
            {
                CloseUI(m_GroupUIs[m_GroupUIs.Count - 1]);
            }
        }

        //设置一个UI为当前激活
        public void SetActivate(IUserInterface ui)
        {
            m_ActivatedUI = ui;
        }

        //处理按钮输入的回调
        public bool HandleInput(InputMessage msg)
        {
            if (m_ActivatedUI != null)
                return m_ActivatedUI.OnInput(msg);
            return false;
        }
    }

    public abstract class IUserInterface
    {
        protected RectTransform m_Transform;
        protected GameObject m_GameObject;

        private string m_PrefabName;
        private EUIType m_Type;

        public string PrefabName { get { return m_PrefabName; } }

        public EUIType Type { get { return m_Type; } }

        public GameObject GameObject { get { return m_GameObject; } }

        public RectTransform Transform { get { return m_Transform; } }


        public IUserInterface(string prefabName, EUIType type)
        {
            m_PrefabName = prefabName;
            m_Type = type;
        }

        private void Initialize(GameObject go)
        {
            m_GameObject = go;
            m_Transform = go.GetComponent<RectTransform>();
            m_Transform.localScale = Vector3.one;
            m_Transform.offsetMin = Vector3.zero;
            m_Transform.offsetMax = Vector3.zero;
            OnInitialize();
        }

        public virtual void OnUpdate() { }

        //处理输入
        public virtual bool OnInput(InputMessage msg) { return false; }

        //在IUserInterface初始化时调用
        protected virtual void OnInitialize() { }

        //当IUserInterface被销毁时调用
        protected virtual void OnDestroy() { }
    }

    public enum EUIType
    {
        SingleUI,
        GroupUI,
        HUD,
    }

}