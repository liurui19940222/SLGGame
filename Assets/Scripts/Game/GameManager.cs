using Framework.Common;
using Framework.Common.Message;
using Framework.FSM;
using Framework.Input;
using Framework.UI;
using Game.Component;
using Game.Scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{
    public class GameManager : Singleton<GameManager>
    {
        private UIManager m_UIManager;

        private ResourceLoader m_ResLoader;

        private FSMMachine m_SceneFSM;

        private InputSystem m_InputSystem;

        public UIManager UIMgr { get { return m_UIManager; } }

        public ResourceLoader ResLoader { get { return m_ResLoader; } }

        public void Launch()
        {
            m_ResLoader = new ResourceLoader();
            m_UIManager = CreateSystem<UIManager>();
            m_InputSystem = InputSystem.Instance;

            m_SceneFSM = new FSMMachine();
            m_SceneFSM.AddState(new StartScene());
            m_SceneFSM.AddState(new FightingScene());
            m_SceneFSM.SetDefaultState(SceneDefines.SCENE_START);

            GameCamera.Instance.Camera = Camera.main;
        }

        public void OnUpdate()
        {
            m_InputSystem.UpdateInput();
            m_UIManager.OnUpdate();
            m_SceneFSM.OnUpdate();
        }

        public void Quit()
        {
            m_SceneFSM.Quit();
            m_UIManager.OnUninitialize();
        }

        public void SwitchScene(int sceneId, IMessage param = null)
        {
            m_SceneFSM.SwitchToState(sceneId, param);
        }

        public T CreateSystem<T>(params object[] pars) where T : IGameSystem, new()
        {
            IGameSystem system = new T();
            system.OnInitialize(m_ResLoader, pars);
            return (T)system;
        }

        public void AddInputEvent(InputSystem.InputDelegate del, int priority)
        {
            m_InputSystem.AddInputEvent(del, priority);
        }

        public void RemoveInputEvent(InputSystem.InputDelegate del)
        {
            m_InputSystem.RemoveInputEvent(del);
        }
    }
}
