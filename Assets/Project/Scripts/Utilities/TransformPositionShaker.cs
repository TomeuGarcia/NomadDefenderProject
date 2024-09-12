using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformPositionShaker : MonoBehaviour
{
    [SerializeField] private float _speed = 10;
    [SerializeField] private float _amplitude = 10;
    [SerializeField] private Vector3 _direction = Vector3.right;

    private void OnValidate()
    {
        _direction.Normalize();
    }

    private void Update()
    {
        transform.localPosition = _direction * (Mathf.Sin(Time.time * _speed) * _amplitude);
    }
    
}
