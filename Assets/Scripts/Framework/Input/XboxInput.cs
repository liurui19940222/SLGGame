using Framework.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Input
{
    public class XboxInput : IInputDevice
    {

        private Vector2 m_LastDPadAxis;
        private float m_LastLTrigger;
        private float m_LastRTrigger;

        //所有按键的状态
        private bool[] m_KeyStates;

        public XboxInput()
        {
            m_KeyStates = new bool[Util.GetEnumMaxValue(typeof(EInputWord)) + 1];
        }

        //得到摇杆输入
        public override Vector2 GetAxis(EJoystick joystick)
        {
            if (joystick == EJoystick.LEFT)
            {
                m_Axis.x = UnityEngine.Input.GetAxis(InputConst.XBOX_KEYWORD_LEFT_JOY_H);
                m_Axis.y = UnityEngine.Input.GetAxis(InputConst.XBOX_KEYWORD_LEFT_JOY_V);
            }
            else if (joystick == EJoystick.RIGHT)
            {
                m_Axis.x = UnityEngine.Input.GetAxis(InputConst.XBOX_KEYWORD_RIGHT_JOY_H);
                m_Axis.y = UnityEngine.Input.GetAxis(InputConst.XBOX_KEYWORD_RIGHT_JOY_V);
            }
            return m_Axis;
        }

        //得到DPad的输入
        public Vector2 GetDPadAxis()
        {
            Vector2 axis = default(Vector2);
            axis.x = UnityEngine.Input.GetAxis(InputConst.XBOX_KEYWORD_DPAD_H);
            axis.y = UnityEngine.Input.GetAxis(InputConst.XBOX_KEYWORD_DPAD_V);
            return axis;
        }

        public override bool GetInput(EInputWord input)
        {
            return m_KeyStates[(int)input];
        }

        public override void UpdateInput()
        {
            TestDPadWord();
            TestTrigger(EInputWord.LT);
            TestTrigger(EInputWord.RT);
            TestFunctionKeyWord(InputConst.XBOX_KEYWORD_BACK, EInputWord.BACK);
            TestFunctionKeyWord(InputConst.XBOX_KEYWORD_START, EInputWord.START);
            TestFunctionKeyWord(InputConst.XBOX_KEYWORD_A, EInputWord.A);
            TestFunctionKeyWord(InputConst.XBOX_KEYWORD_B, EInputWord.B);
            TestFunctionKeyWord(InputConst.XBOX_KEYWORD_X, EInputWord.X);
            TestFunctionKeyWord(InputConst.XBOX_KEYWORD_Y, EInputWord.Y);
            TestFunctionKeyWord(InputConst.XBOX_KEYWORD_LB, EInputWord.LB);
            TestFunctionKeyWord(InputConst.XBOX_KEYWORD_L3, EInputWord.L3);
            TestFunctionKeyWord(InputConst.XBOX_KEYWORD_RB, EInputWord.RB);
            TestFunctionKeyWord(InputConst.XBOX_KEYWORD_R3, EInputWord.R3);
        }

        //测试功能键的输入
        private void TestFunctionKeyWord(string button, EInputWord word)
        {
            if (UnityEngine.Input.GetButtonDown(button))
            {
                m_InputSystem.OnInput(word, true);
                m_KeyStates[(int)word] = true;
            }
            else if (UnityEngine.Input.GetButtonUp(button))
            {
                m_InputSystem.OnInput(word, false);
                m_KeyStates[(int)word] = false;
            }
        }

        //测试扳机键的输入
        private void TestTrigger(EInputWord trigger)
        {
            float value = 0;
            float lastValue = 0;
            if (trigger == EInputWord.LT)
            {
                value = UnityEngine.Input.GetAxis(InputConst.XBOX_KEYWORD_LT) != 0 ? 1 : 0;
                lastValue = m_LastLTrigger;
                m_LastLTrigger = value;
            }
            else if (trigger == EInputWord.RT)
            {
                value = UnityEngine.Input.GetAxis(InputConst.XBOX_KEYWORD_RT) != 0 ? 1 : 0;
                lastValue = m_LastRTrigger;
                m_LastRTrigger = value;
            }
            if (value > 0)
            {
                if (lastValue == 0)
                    m_InputSystem.OnInput(trigger, true);
                m_KeyStates[(int)trigger] = true;
            }
            else if (lastValue > 0)
            {
                m_InputSystem.OnInput(trigger, false);
                m_KeyStates[(int)trigger] = false;
            }
        }

        //测试DPad的输入，并转换成合适的键值传递
        private void TestDPadWord()
        {
            Vector2 axis = GetDPadAxis();
            if (axis != Vector2.zero)
            {
                EInputWord lastWord = GetDPadWord(m_LastDPadAxis);
                EInputWord word = GetDPadWord(axis);
                if (lastWord != word)
                {
                    if (lastWord != EInputWord.NONE)
                    {
                        m_InputSystem.OnInput(lastWord, false);
                        m_KeyStates[(int)lastWord] = false;
                    }
                    m_InputSystem.OnInput(word, true);
                    m_KeyStates[(int)word] = true;
                }
            }
            else if (m_LastDPadAxis != Vector2.zero)
            {
                EInputWord lastWord = GetDPadWord(m_LastDPadAxis);
                if (lastWord != EInputWord.NONE)
                {
                    m_InputSystem.OnInput(lastWord, false);
                    m_KeyStates[(int)lastWord] = false;
                }
            }
            m_LastDPadAxis = axis;
        }

        //将DPad的二维向量输入转换为4个方向的键值
        private EInputWord GetDPadWord(Vector2 axis)
        {
            if (axis == Vector2.zero)
                return EInputWord.NONE;
            if (Mathf.Abs(axis.x) > Mathf.Abs(axis.y))
            {
                return axis.x > 0 ? EInputWord.DPAD_RIGHT : EInputWord.DPAD_LEFT;
            }
            else
            {
                return axis.y > 0 ? EInputWord.DPAD_UP : EInputWord.DPAD_DOWN;
            }
        }

        //打印摇杆的输入
        private void DebugPrint()
        {
            Vector2 vec2 = GetAxis(EJoystick.LEFT);
            if (vec2 != Vector2.zero)
                Debug.Log("Left Joystick axis:" + vec2);

            vec2 = GetAxis(EJoystick.RIGHT);
            if (vec2 != Vector2.zero)
                Debug.Log("Right Joystick axis:" + vec2);

            vec2 = GetDPadAxis();
            if (vec2 != Vector2.zero)
                Debug.Log("DPad Joystick axis:" + vec2);
        }
    }
}