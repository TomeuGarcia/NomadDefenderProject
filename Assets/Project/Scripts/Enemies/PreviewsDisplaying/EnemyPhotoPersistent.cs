using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPhotoPersistent : MonoBehaviour
{
    private static EnemyPhotoPersistent _instance;
    
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
