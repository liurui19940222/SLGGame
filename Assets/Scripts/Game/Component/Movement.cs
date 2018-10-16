using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component
{
    public class Movement : MonoBehaviour
    {
        private Transform m_Transform;
        private Vector3 m_Velocity;
        private Vector3 m_BeginingPos;
        private bool m_Moving = false;
        private float m_MovingTimer = 0.0f;
        private float m_MovingTime = 0.0f;
        private float m_MovingProgress = 0.0f;

        public bool IsMoving { get { return m_Moving; } }

        public float MovingProgress { get { return m_MovingProgress; } }

        void Awake()
        {
            m_Transform = transform;
        }

        void Update()
        {
            UpdateMovement();
        }

        public void MoveWithSpeed(Vector3 endPos, float speed)
        {
            MoveWithTime(endPos, (endPos - m_Transform.position).magnitude / speed);
        }

        public void MoveWithTime(Vector3 endPos, float time)
        {
            m_Velocity = endPos - m_Transform.position;
            m_BeginingPos = m_Transform.position;
            m_MovingTime = time;
            m_MovingTimer = 0;
            m_Moving = true;
        }

        private void UpdateMovement()
        {
            if (!m_Moving)
                return;
            m_MovingTimer += Time.deltaTime;
            if (m_MovingTimer >= m_MovingTime)
            {
                m_MovingProgress = 1.0f;
                m_Transform.position = m_BeginingPos + m_Velocity;
                m_Moving = false;
            }
            else
            {
                m_MovingProgress = m_MovingTimer / m_MovingTime;
                m_Transform.position = m_BeginingPos + m_Velocity * m_MovingProgress;
            }
        }
    }
}