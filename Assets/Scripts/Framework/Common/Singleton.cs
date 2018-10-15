using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Framework.Common
{
    public class Singleton<T> where T : Singleton<T>, new()
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                    _instance.OnInitialize();
                }
                return _instance;
            }
        }

        protected virtual void OnInitialize() { }
    }
}