//#define ENABLE_TEST

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component
{
    public class CharacterMovement : MonoBehaviour
    {
#if ENABLE_TEST
        public Transform[] transforms;
        public float MoveSpeed = 5.0f;
#else
        private float MoveSpeed = 5.0f;
#endif

        private Transform m_Transform;
        private Animator m_Animator;
        private Vector3[] m_Path;
        private float[] m_Distances;
        private float m_TotalDistance;

        private float m_MovingTimer;
        private float m_MovingTime;
        private bool m_IsMoving = false;

        void Awake()
        {
            m_Transform = transform;
            m_Animator = GetComponentInChildren<Animator>();

#if ENABLE_TEST
            List<Vector3> path = new List<Vector3>();
            foreach (Transform trans in transforms)
            {
                path.Add(trans.position);
            }
            MoveAlongPath(path.ToArray(), MoveSpeed);
#endif
        }

        void Update()
        {
            if (!m_IsMoving)
                return;
            m_MovingTimer += Time.deltaTime;
            if (m_MovingTimer >= m_MovingTime)
            {
                m_Transform.position = GetPosInPath(1.0f);
                m_Animator.SetFloat("Speed", 0.0f);
                m_IsMoving = false;
            }
            else
            {
                Vector3 oldPos = m_Transform.position;
                m_Transform.position = GetPosInPath(m_MovingTimer / m_MovingTime);
                Quaternion targetRotation = Quaternion.LookRotation(m_Transform.position - oldPos);
                m_Transform.rotation = Quaternion.RotateTowards(m_Transform.rotation, targetRotation, 10);
                m_Animator.SetFloat("Speed", 1.0f);
            }
        }

        // 移动到指定位置
        public void MoveTo(Vector3 targetPos, float speed)
        {
            Vector3[] path = new Vector3[2];
            path[0] = m_Transform.position;
            path[1] = targetPos;
            MoveAlongPath(path, speed);
        }

        // 沿着路径移动
        public void MoveAlongPath(Vector3[] path, float speed)
        {
            MoveSpeed = speed;
            if (path.Length < 2 || MoveSpeed == 0.0f)
                return;
            m_Path = path;
            CalcDistance(path);
            m_MovingTimer = 0.0f;
            m_MovingTime = m_TotalDistance / MoveSpeed;
            m_IsMoving = true;
        }

        // 计算每一个点到下一个点的距离
        private void CalcDistance(Vector3[] path)
        {
            m_TotalDistance = 0.0f;
            m_Distances = new float[path.Length - 1];
            for (int i = 0; i < path.Length - 1; ++i)
            {
                m_Distances[i] = Vector3.Distance(path[i], path[i + 1]);
                m_TotalDistance += m_Distances[i];
            }
        }

        // 根据归一化的时间参数获得位置
        private Vector3 GetPosInPath(float t)
        {
            if (t == 0.0f)
                return m_Path[0];
            else if (t >= 1.0f)
                return m_Path[m_Path.Length - 1];
            float dist = t * m_TotalDistance;
            float temp = 0.0f;
            int index = 0;
            for (index = 0; index < m_Distances.Length; ++index)
            {
                temp += m_Distances[index];
                if (temp >= dist)
                {
                    break;
                }
            }
            float t_betweenAB = (dist - (temp - m_Distances[index])) / m_Distances[index];
            return Vector3.Lerp(m_Path[index], m_Path[index + 1], t_betweenAB);
        }
    }
}
