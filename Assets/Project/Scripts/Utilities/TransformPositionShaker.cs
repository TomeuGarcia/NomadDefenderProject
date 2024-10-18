using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformPositionShaker : MonoBehaviour
{
    [SerializeField] private float _speed = 10;
    [SerializeField] private float _amplitude = 10;
    [SerializeField] private Vector3 _direction = Vector3.right;
    [SerializeField] private bool _backAndForth = true;
    [SerializeField] private bool _addStartingLocalPosition = false;
    [SerializeField] private float _time = 0f;

    private Vector3 _startingLocalPosition;
    
    
    private void OnValidate()
    {
        _direction.Normalize();
    }

    private void Awake()
    {
        _startingLocalPosition = transform.localPosition;
    }

    private void Update()
    {
        _time += Time.deltaTime * _speed;
        float sine = _backAndForth
            ? Mathf.Sin(_time)
            : (Mathf.Sin(_time) + 1) * 0.5f;
        
        
        Vector3 newPosition = _direction * (sine * _amplitude);
        if (_addStartingLocalPosition)
        {
            newPosition += _startingLocalPosition;
        }

        
        transform.localPosition = newPosition;
    }
    
}
