using Framework.Common;
using Game.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.SLG
{
    public enum ECharacterRelation
    {
        OwnSide = 0,
        Opposed = 1,
        Friendly = 2,
    }

    public class SLGCharacterSystem : IGameSystem
    {
        private List<ICharacter> m_OwnSideCharacters;

        private List<ICharacter> m_OpposedCharacters;

        private List<ICharacter> m_FriendlyCharacters;

        public override void OnInitialize(IResourceLoader loader)
        {
            m_OwnSideCharacters = new List<ICharacter>();
            m_OpposedCharacters = new List<ICharacter>();
            m_FriendlyCharacters = new List<ICharacter>();
        }

        public override void OnUninitialize()
        {
           
        }

        public override void OnUpdate()
        {
            
        }

        public void CreateCharacterAtPoint(ECharacterRelation relation)
        {

        }
    }
}
