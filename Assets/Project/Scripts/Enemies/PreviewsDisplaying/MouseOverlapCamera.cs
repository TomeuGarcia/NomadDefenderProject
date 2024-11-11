using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOverlapCamera : MonoBehaviour
{
    public static Camera Camera { get; private set; }
    
    private void Awake()
    {
        Camera = gameObject.GetComponent<Camera>();
    }

    private void OnDestroy()
    {
        Camera = null;
    }
}
