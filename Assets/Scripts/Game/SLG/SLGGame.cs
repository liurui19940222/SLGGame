using Framework.AStar;
using Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.SLG
{
    public class SLGGame : Singleton<SLGGame>
    {
        private Transform m_RootTf;
        private GridMap2D m_MapData;
        private SLGCharacterSystem m_CharacterSystem;

        public GridMap2D MapData { get { return m_MapData; } }

        public void Load(GridMap2D mapData, Transform rootTf)
        {
            m_RootTf = rootTf;
            m_MapData = mapData;
            m_CharacterSystem = GameManager.Instance.CreateSystem<SLGCharacterSystem>();

            Game.Data.ActionRangeData rangeData = new Data.ActionRangeData();

            rangeData.MovingList.Add(new Data.ActionCellData(10, 10));
            rangeData.MovingList.Add(new Data.ActionCellData(11, 10));
            rangeData.MovingList.Add(new Data.ActionCellData(12, 10));

            rangeData.AttackingList.Add(new Data.ActionCellData(10, 11));
            rangeData.AttackingList.Add(new Data.ActionCellData(11, 11));
            rangeData.AttackingList.Add(new Data.ActionCellData(12, 11));

            Entity.ActionRangeView view = new Entity.ActionRangeView();
            view.Create(rangeData, m_RootTf);
        }

        public void UnLoad()
        {

        }

        public void OnUpdate()
        {

        }
    }
}
