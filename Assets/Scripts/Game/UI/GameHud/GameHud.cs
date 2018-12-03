using Framework.Common.Message;
using Framework.UI;
using Game.Common;
using Game.UI.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.GameHud
{
    public enum EActionMenuOption
    {
        Cancel = -1,
        Attack = 0,
        Skill,
        Standby,
    }

    public class GameHud : IUserInterface
    {
        private ConsoleMenu m_ActionMenu;
        private GameHudTurn m_Turn;

        public GameHud() : base("GameHud", EUIType.HUD)
        {

        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            m_ActionMenu = m_Transform.Find("ActionMenu").GetComponent<ConsoleMenu>();
            m_Turn = new GameHudTurn(m_Transform.Find("Turn"));

            m_ActionMenu.SetOnSelectedDelegate(OnActionMenuItemSelected);

            AddListener(UIDefines.ID_SHOW_ACTION_MENU, OnShowActionMenu);
            AddListener(UIDefines.ID_SHOW_TURN, OnShowTurn);
        }

        public override bool OnInput(InputMessage msg)
        {
            return m_ActionMenu.OnInput(msg);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            RemoveListener(UIDefines.ID_SHOW_ACTION_MENU, OnShowActionMenu);
            RemoveListener(UIDefines.ID_SHOW_TURN, OnShowTurn);
        }

        private void OnShowActionMenu(IMessage msg)
        {
            UI.Message.ShowActionMenuMsg menuMsg = msg as UI.Message.ShowActionMenuMsg;
            m_ActionMenu.Clear();
            m_ActionMenu.AddItem<ConsoleMenuItemText>((int)EActionMenuOption.Attack).SetText("攻击");
            m_ActionMenu.AddItem<ConsoleMenuItemText>((int)EActionMenuOption.Standby).SetText("待机");
            m_ActionMenu.ResetCursor();
            m_ActionMenu.Show(true);
            GameManager.Instance.UIMgr.SetActivate(this, true);
        }

        private void OnShowTurn(IMessage msg)
        {
            UI.Message.ShowTurnMsg menuMsg = msg as UI.Message.ShowTurnMsg;

            m_Turn.ShowTurn(true, menuMsg.TurnType);
        }

        private void OnActionMenuItemSelected(int id)
        {
            MenuSelectedMsg msg = new MenuSelectedMsg();
            msg.option = (EActionMenuOption)id;
            this.m_ActionMenu.Show(false);
            GameManager.Instance.UIMgr.SetActivate(this, false);
            MessageCenter.Instance.SendMessage(UIDefines.ID_MENU_SELECTED, msg);
        }
    }
}