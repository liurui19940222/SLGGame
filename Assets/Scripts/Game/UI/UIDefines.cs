using Framework.Common.Message;
using Game.Common;

namespace Game.UI
{
    public static class UIDefines
    {
        // 显示操作菜单
        public const int ID_SHOW_ACTION_MENU = 1000;

        // 显示回合
        public const int ID_SHOW_TURN = 1001;
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

    }
}