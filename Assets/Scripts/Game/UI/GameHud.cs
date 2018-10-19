using Framework.Common.Message;
using Framework.UI;
using Game.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public enum EActionMenuOption
    {
        Attack,
        Skill,
        Standby,
    }

    public class GameHud : IUserInterface
    {
        private ConsoleMenu m_ActionMenu;

        private GameObject m_TurnGo;
        private Text m_TurnTextBack;
        private Text m_TurnText;

        public GameHud() : base("GameHud", EUIType.HUD)
        {

        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            m_ActionMenu = m_Transform.Find("ActionMenu").GetComponent<ConsoleMenu>();
            m_TurnGo = m_Transform.Find("Turn").gameObject;
            m_TurnText = m_Transform.Find("Turn/TurnText").GetComponent<Text>();
            m_TurnTextBack = m_Transform.Find("Turn/TurnTextBack").GetComponent<Text>();

            AddListener(UIDefines.ID_SHOW_ACTION_MENU, OnShowActionMenu);
        }

        public override bool OnInput(InputMessage msg)
        {
            return m_ActionMenu.OnInput(msg);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            RemoveListener(UIDefines.ID_SHOW_ACTION_MENU, OnShowActionMenu);
        }

        private void OnShowActionMenu(IMessage msg)
        {
            UI.Message.ShowActionMenuMsg menuMsg = msg as UI.Message.ShowActionMenuMsg;
            m_ActionMenu.Clear();
            m_ActionMenu.AddItem<ConsoleMenuItemText>((int)EActionMenuOption.Attack).SetText("攻击");
            m_ActionMenu.AddItem<ConsoleMenuItemText>((int)EActionMenuOption.Standby).SetText("待机");
            m_ActionMenu.ResetCursor();
            m_ActionMenu.Show(true);
            GameManager.Instance.UIMgr.SetActivate(this);
        }

        private void OnShowTurn(IMessage msg)
        {
            UI.Message.ShowTurnMsg menuMsg = msg as UI.Message.ShowTurnMsg;


        }

        private void ShowTurn(bool show, ETurnType type)
        {
            m_TurnGo.SetActive(show);
            if (!show)
                return;

            if (type == Common.ETurnType.OwnSide)
                m_TurnText.text = m_TurnTextBack.text = "我方回合";
            else if (type == Common.ETurnType.Friendly)
                m_TurnText.text = m_TurnTextBack.text = "友方回合";
            else if (type == Common.ETurnType.Friendly)
                m_TurnText.text = m_TurnTextBack.text = "敌方回合";


        }
    }
}