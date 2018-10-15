

using System;
using System.Collections;
using Framework.Common.Message;
using Game.Common;
using UnityEngine;
using Game.Component;

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
            m_MapRes = GameManager.Instance.ResLoader.LoadMapAsset(msg.TargetMapName);
            m_MapRes.transform.SetParent(GetRootTf());
            m_MapRenderer = GameManager.Instance.ResLoader.LoadToolComponent<MapRenderer>("MapRenderer");
            m_MapRenderer.MapData = m_MapRes.MapData;
            m_MapRenderer.Camera = GameCamera.Instance.Camera;
            m_MapRenderer.EnableGridDrawing();
            m_MapRenderer.transform.SetParent(GetRootTf());
        }

        protected override string GetName()
        {
            return "FightScene";
        }

        protected override void OnSceneInitialized()
        {
            SLG.SLGGame.Instance.Load(m_MapRes.MapData, GetRootTf());
            Debug.Log("Fighting Scene OnSceneInitialized");
            
        }

        protected override void OnSceneUnload()
        {
            SLG.SLGGame.Instance.UnLoad();
            Debug.Log("Fighting Scene OnSceneUnload");
        }

        protected override void OnSceneUpdate()
        {
            SLG.SLGGame.Instance.OnUpdate();
        }
    }
}
