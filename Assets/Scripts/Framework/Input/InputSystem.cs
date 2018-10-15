using Framework.Common;
using Framework.Common.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Input
{
    public class InputSystem : Singleton<InputSystem>
    {

        //按键的事件按优先级依次派发，若有处理结果返回true，则终止派发
        public delegate bool InputDelegate(InputMessage msg);

        private List<IInputDevice> m_InputDevices;

        private LinkedList<InputEvent> m_InputEvent;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            m_InputEvent = new LinkedList<InputEvent>();
            m_InputDevices = new List<IInputDevice>();
        }

        public void AddInputDevice(IInputDevice device)
        {
            m_InputDevices.Add(device);
        }

        public void UpdateInput()
        {
            if (m_InputDevices.Count == 0)
                return;
            m_InputDevices.ForEach((device) =>
            {
                device.UpdateInput();
            });
        }

        InputMessage inputMsg = new InputMessage();
        public void OnInput(EInputWord word, bool down)
        {
            inputMsg.Word = word;
            inputMsg.IsDown = down;
            foreach (InputEvent del in m_InputEvent)
            {
                if (del.Delegate(inputMsg)) break;
            }
        }

        public Vector2 GetAxis(EJoystick joystick)
        {
            Vector2 axis = Vector2.zero;
            for (int i = 0; i < m_InputDevices.Count; ++i)
            {
                if ((axis = m_InputDevices[i].GetAxis(joystick)) != Vector2.zero)
                    return axis;
            }
            return axis;
        }

        public bool GetInput(EInputWord word)
        {
            bool active = false;
            for (int i = 0; i < m_InputDevices.Count; ++i)
            {
                if ((active = m_InputDevices[i].GetInput(word)))
                    return active;
            }
            return active;
        }

        public void AddInputEvent(InputDelegate del, int priority)
        {
            LinkedListNode<InputEvent> node = m_InputEvent.First;
            InputEvent input = new InputEvent(del, priority);
            while (node != null)
            {
                if (priority < node.Value.Priority)
                {
                    node = node.Next;
                }
                else
                {
                    m_InputEvent.AddBefore(node, input);
                    return;
                }
            }
            m_InputEvent.AddLast(input);
        }

        public void RemoveInputEvent(InputDelegate del)
        {
            LinkedListNode<InputEvent> node = m_InputEvent.First;
            while (node != null)
            {
                if (node.Value.Delegate == del)
                {
                    m_InputEvent.Remove(node);
                    break;
                }
                node = node.Next;
            }
        }
    }

    public class InputEvent
    {
        public InputSystem.InputDelegate Delegate { set; get; }

        public int Priority { set; get; }

        public InputEvent() { }

        public InputEvent(InputSystem.InputDelegate _delegate, int priority)
        {
            Delegate = _delegate;
            Priority = priority;
        }
    }

    public abstract class IInputDevice
    {
        protected InputSystem m_InputSystem;

        protected Vector2 m_Axis;

        public IInputDevice()
        {
            m_InputSystem = InputSystem.Instance;
        }

        public abstract Vector2 GetAxis(EJoystick joystick);

        public abstract bool GetInput(EInputWord input);

        public abstract void UpdateInput();
    }

    public enum EJoystick
    {
        LEFT,
        RIGHT,
    }

    public enum EInputWord
    {
        NONE = 0,
        DPAD_UP,
        DPAD_DOWN,
        DPAD_LEFT,
        DPAD_RIGHT,
        BACK,
        START,
        A,
        B,
        X,
        Y,
        LB,
        LT,
        L3,
        RB,
        RT,
        R3,
    }
}