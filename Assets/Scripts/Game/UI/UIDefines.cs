using Framework.Common.Message;
using Game.Common;
using Game.UI.GameHud;

namespace Game.UI
{
    public static class UIDefines
    {
        // 显示操作菜单
        public const int ID_SHOW_ACTION_MENU = 1000;

        // 显示回合
        public const int ID_SHOW_TURN = 1001;

        // 取消
        public const int ID_MENU_SELECTED = 1002;
    }

    namespace Message
    {

        // ID_SHOW_ACTION_MENU
        public class ShowActionMenuMsg : IMessage
        {
            
        }

        // ID_SHOW_TURN
        public class ShowTurnMsg : IMessage
        {
            public ETurnType TurnType;
        }

        // ID_MENU_SELECTED
        public class MenuSelectedMsg : IMessage
        {
            public EActionMenuOption option;
        }
    }
}