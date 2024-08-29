using System;
using UnityEngine;

public class TransformForwardRotator : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 10f;
    private float _currentRotation = 0f;
    
    private void Update()
    {
        _currentRotation = _currentRotation + (_rotationSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.AngleAxis(_currentRotation, Vector3.forward);
    }
    
}