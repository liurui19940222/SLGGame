using Game.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Scene
{
    public class StartScene : BaseScene
    {
        public StartScene() : base(SceneDefines.SCENE_START)
        {
        }

        protected override string GetName()
        {
            return "StartScene";
        }

        protected override void OnSceneInitialized()
        {
            Debug.Log("Start Scene OnSceneInitialized");
            FightingSceneMsg msg = new FightingSceneMsg();
            msg.TargetLevelName = "Level_01";
            GameManager.Instance.SwitchScene(SceneDefines.SCENE_FIGHTING, msg);
        }

        protected override void OnSceneUnload()
        {
            Debug.Log("Start Scene OnSceneUnload");
        }

        protected override void OnSceneUpdate()
        {
            
        }
    }
}
