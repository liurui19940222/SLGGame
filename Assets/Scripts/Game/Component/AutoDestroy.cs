using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public bool destroyOnGameStart;

    void Awake()
    {
        if (GameManager.Instance.IsGameRunning)
            Destroy(gameObject);
    }

}
