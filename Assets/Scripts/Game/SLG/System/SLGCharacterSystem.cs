using Framework.Common;
using Game.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.SLG.System
{
    public enum ECharacterRelation
    {
        OwnSide = 0,
        Opposed = 1,
        Friendly = 2,
        Nothing = 3,
    }

    public class SLGCharacterSystem : IGameSystem
    {
        private static int GameId;

#if UNITY_EDITOR
        private Transform m_Root;
        private Transform m_OwnSideRoot;
        private Transform m_OpposedRoot;
        private Transform m_FriendlyRoot;
#endif

        private List<Character> m_OwnSideCharacters;

        private List<Character> m_OpposedCharacters;

        private List<Character> m_FriendlyCharacters;

        public override void OnInitialize(IResourceLoader loader, params object[] pars)
        {
            m_OwnSideCharacters = new List<Character>();
            m_OpposedCharacters = new List<Character>();
            m_FriendlyCharacters = new List<Character>();

            Transform root = pars[0] as Transform;

#if UNITY_EDITOR
            m_Root = new GameObject("SLGCharacterSystem").transform;
            m_OwnSideRoot = new GameObject("OwnSide").transform;
            m_OpposedRoot = new GameObject("Opposed").transform;
            m_FriendlyRoot = new GameObject("Friendly").transform;
            m_Root.SetParent(root);
            m_OwnSideRoot.SetParent(m_Root);
            m_OpposedRoot.SetParent(m_Root);
            m_FriendlyRoot.SetParent(m_Root);
#endif

        }

        public override void OnUninitialize()
        {
           
        }

        public override void OnUpdate()
        {
            
        }

        public Character CreateCharacterAtPoint(ECharacterRelation relation, int id, Framework.AStar.IPoint point)
        {
            List<Character> list = null;
            Transform parent = null;
            switch (relation)
            {
                case ECharacterRelation.OwnSide:
                    list = m_OwnSideCharacters;
                    parent = m_OwnSideRoot;
                    break;
                case ECharacterRelation.Opposed:
                    list = m_OpposedCharacters;
                    parent = m_OpposedRoot;
                    break;
                case ECharacterRelation.Friendly:
                    list = m_FriendlyCharacters;
                    parent = m_FriendlyRoot;
                    break;
            }
            Character character = new Character(id, parent);
            character.GID = ++GameId;
            list.Add(character);
            character.SetCellPos(point);
            return character;
        }

        // 刷新所有角色的行动
        public void RefreshActions(ECharacterRelation relation)
        {
            List<Character> list = GetChList(relation);
            foreach (Character ch in list)
            {
                ch.RefreshAction();
            }
        }

        // 得到能行动的角色的数量
        public int GetCanActCount(ECharacterRelation relation)
        {
            List<Character> list = GetChList(relation);
            int count = 0;
            foreach (Character ch in list)
            {
                if (ch.HasAction)
                    count++;
            }
            return count;
        }

        private List<Character> GetChList(ECharacterRelation relation)
        {
            List<Character> list = null;
            switch (relation)
            {
                case ECharacterRelation.OwnSide:
                    list = m_OwnSideCharacters;
                    break;
                case ECharacterRelation.Opposed:
                    list = m_OpposedCharacters;
                    break;
                case ECharacterRelation.Friendly:
                    list = m_FriendlyCharacters;
                    break;
            }
            return list;
        }
    }
} 
