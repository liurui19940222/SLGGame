using Framework.Common;
using UnityEngine;

namespace Game.Component
{
    public class GameCamera : Singleton<GameCamera>
    {
        private Camera m_Camera;

        public Camera Camera
        {
            get { return m_Camera; }
            set { m_Camera = value; }
        }
    }
}
