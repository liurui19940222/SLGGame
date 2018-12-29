using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Game.SLG.System;
using Game.Common;

namespace Game.SLG.Level.Action
{
    [Serializable]
    public class SpawnData
    {
        public int x;
        public int y;
        public int id;
        public Direction direction;
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
                Entity.Character ch = SLG.SLGGame.Instance.CS_CreateCharacterAtPoint(_param.relation, list[i].id, new Framework.AStar.IPoint(list[i].x, list[i].y));
                ch.LookAt(list[i].direction);
            }
        }
    }
}