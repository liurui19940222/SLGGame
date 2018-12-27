

using System;
using System.Collections;
using Framework.Common.Message;
using Game.Common;
using UnityEngine;
using Game.Component;
using UnityEngine.SceneManagement;
using Game.Config;

namespace Game.Scene
{
    public class FightingScene : BaseScene
    {
        private MapRes m_MapRes;
        private MapRenderer m_MapRenderer;

        public FightingScene() : base(SceneDefines.SCENE_FIGHTING)
        {
        }

        protected override IEnumerator OnInitialize(IMessage param)
        {
            FightingSceneMsg msg = param as FightingSceneMsg;
            if (msg == null)
            {
                yield break;
            }
            yield return SceneManager.LoadSceneAsync(msg.TargetLevelName);

            var mapRoot = GameObject.FindWithTag(GameTag.MapRoot);
            m_MapRes = mapRoot.GetComponent<MapRes>();
            m_MapRenderer = GameManager.Instance.ResLoader.LoadToolComponent<MapRenderer>("MapRenderer");
            m_MapRenderer.InitWithMapData(m_MapRes.mapData);
            m_MapRenderer.EnableGridDrawing();
            m_MapRenderer.transform.SetParent(GetRootTf());
        }

        protected override string GetName()
        {
            return "FightScene";
        }

        protected override void OnSceneInitialized()
        {
            SLG.SLGGame.Instance.Load(m_MapRes, GetRootTf());
            Debug.Log("Fighting Scene OnSceneInitialized");
            
        }

        protected override void OnSceneUnload()
        {
            SLG.SLGGame.Instance.Unload();
            Debug.Log("Fighting Scene OnSceneUnload");
        }

        protected override void OnSceneUpdate()
        {
            SLG.SLGGame.Instance.OnUpdate();
        }
    }
}
