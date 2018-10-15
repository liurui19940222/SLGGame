using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GameLoop : MonoBehaviour
    {

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            GameManager.Instance.Launch();
        }

        void Update()
        {
            GameManager.Instance.OnUpdate();
        }

        void OnDestroy()
        {
            GameManager.Instance.Quit();
        }
    }
}