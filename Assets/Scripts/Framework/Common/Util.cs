using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Framework.Common
{
    public class Util
    {
        //反射调用成员方法
        public static void Invoke(object obj, string method, params object[] param)
        {
            MethodInfo info = obj.GetType().GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (info != null)
            {
                info.Invoke(obj, param);
            }
        }

        //得到枚举最大值
        public static int GetEnumMaxValue(Type type)
        {
            int Max = int.MinValue;

            foreach (int i in Enum.GetValues(type))
            {
                if (i > Max) Max = i;
            }

            return Max;
        }

        //分割Rect
        public static List<Rect> SplitRect(Rect rect, List<float> weights)
        {
            List<Rect> list = new List<Rect>();
            float sum = 0;
            weights.ForEach((f) =>
            {
                sum += f;
            });
            float totalWidth = rect.width;
            float totalHeight = rect.height;
            float x = rect.xMin;
            float y = rect.y;
            for (int i = 0; i < weights.Count; ++i)
            {
                weights[i] /= sum;
                Rect r = new Rect();
                r.width = totalWidth * weights[i];
                r.height = totalHeight;
                r.x = x;
                r.y = y;
                x += r.width * 0.5f;
                x += r.width * 0.5f;
                list.Add(r);
            }

            return list;
        }

        //在指定位置创建一个cube
        private static List<GameObject> m_DebugGos = new List<GameObject>();
        public static void DebugCube(Vector3 position, Vector3 scale, Color color)
        {
            //GameObject go = GameObject.Instantiate(ResourceFactory.Instance.LoadAsset<UnityEngine.Object>("Debug/", "Cube")) as GameObject;
            //m_DebugGos.Add(go);
            //go.GetComponent<MeshRenderer>().material.color = color;
            //go.transform.localScale = scale;
            //go.transform.position = position;
        }

        //清除所有Debug生成的对象
        public static void ClearDebug()
        {
            foreach (GameObject go in m_DebugGos)
            {
                GameObject.Destroy(go);
            }
            m_DebugGos.Clear();
        }

        //深拷贝
        public static T DeepCopy<T>(T obj)
        {
            //如果是字符串或值类型则直接返回
            if (obj is string || obj.GetType().IsValueType) return obj;

            object retval = Activator.CreateInstance(obj.GetType());
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                try { field.SetValue(retval, DeepCopy(field.GetValue(obj))); }
                catch { }
            }
            return (T)retval;
        }
    }
}