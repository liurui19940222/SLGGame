using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Game.SLG.System;

namespace Game.SLG.Level.Action
{
    [Serializable]
    public class SpawnData
    {
        public int x;
        public int y;
        public int id;
    }

    [CreateAssetMenu(menuName = "SLGGame/Action/SpawnActionParam")]
    public class SpawnActionParam : Param
    {
        public ECharacterRelation relation;

        public List<SpawnData> list;
    }

    public class SpawnAction : ActionBase
    {
        public override void Trigger(Param param)
        {
            SpawnActionParam _param = param as SpawnActionParam;
            List<SpawnData> list = _param.list;
            for (int i = 0; i < list.Count; ++i)
            {
                SLG.SLGGame.Instance.CS_CreateCharacterAtPoint(_param.relation, list[i].id, new Framework.AStar.IPoint(list[i].x, list[i].y));
            }
        }
    }
}