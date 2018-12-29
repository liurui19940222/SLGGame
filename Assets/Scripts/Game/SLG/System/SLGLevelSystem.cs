using Framework.Common;
using Framework.Common.Message;
using Framework.FSM;
using Game.SLG.Turn;
using Game.SLG.Turn.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.SLG.Level;
using Game.SLG.Level.Condition;
using Game.SLG.Level.Action;
using Game.Config;

namespace Game.SLG.System
{
    public class SLGLevelSystem : IGameSystem
    {
        private LevelConfig m_Config;

        private Dictionary<ECondition, ConditionBase> m_Checker;

        private Dictionary<EAction, ActionBase> m_Worker;

        public SLGLevelSystem()
        {
            m_Checker = new Dictionary<ECondition, ConditionBase>();
            m_Worker = new Dictionary<EAction, ActionBase>();
        }

        public override void OnInitialize(IResourceLoader loader, params object[] pars)
        {
            m_Config = pars[0] as LevelConfig;
            RegisterChecker<ConditionOnStart>(ECondition.OnStart);
            RegisterChecker<ConditionOnTurnStart>(ECondition.OnTurnStart);
            RegisterChecker<ConditionOnSomeoneKilled>(ECondition.OnSomeoneKilled);
            RegisterChecker<ConditionOnEnd>(ECondition.OnEnd);

            RegisterWorker<SpawnAction>(EAction.Spawn);
            RegisterWorker<StoryAction>(EAction.Story);

            MessageCenter.Instance.AddListener(WorldMessage.ON_SLGGAME_START, OnGameStart);
        }

        public override void OnUpdate()
        {

        }

        public override void OnUninitialize()
        {
            m_Checker.Clear();
            m_Worker.Clear();

            MessageCenter.Instance.RemoveListener(WorldMessage.ON_SLGGAME_START, OnGameStart);
        }

        public override void OnInputMsg(InputMessage msg)
        {

        }

        public bool Check(ECondition condition, Param param)
        {
            if (!m_Checker.ContainsKey(condition))
            {
                Debug.LogError("the checker of handling condition " + condition + " is not registered");
                return false;
            }
            return m_Checker[condition].Check(param, SLG.SLGGame.Instance.Environment);
        }

        public void Trigger(EAction action, Param param)
        {
            if (!m_Worker.ContainsKey(action))
            {
                Debug.LogError("the worker of handling action " + action + " is not registered");
                return;
            }
            m_Worker[action].Trigger(param);
        }

        private void RegisterChecker<T>(ECondition condition) where T : ConditionBase, new()
        {
            if (m_Checker.ContainsKey(condition))
            {
                Debug.LogError("repeat to register condition " + condition);
                return;
            }
            m_Checker.Add(condition, new T());
        }

        private void RegisterWorker<T>(EAction action) where T : ActionBase, new()
        {
            if (m_Worker.ContainsKey(action))
            {
                Debug.LogError("repeat to register worker " + action);
                return;
            }
            m_Worker.Add(action, new T());
        }

        private void OnGameStart(IMessage msg)
        {
            foreach (LevelEvent _event in m_Config.Events)
            {
                if (_event.condition == ECondition.OnStart)
                {
                    if (Check(_event.condition, _event.conditionParam))
                    {
                        Trigger(_event.action, _event.actionParam);
                    }
                }
            }
        }
    }
}