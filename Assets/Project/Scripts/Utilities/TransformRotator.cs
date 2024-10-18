using System;
using UnityEngine;

public class TransformRotator : MonoBehaviour
{
    [SerializeField] private Vector3 _rotationAxis = Vector3.forward;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private float _currentRotation = 0f;

    private void Awake()
    {
        OnValidate();
    }

    private void OnValidate()
    {
        _rotationAxis.Normalize();
    }

    private void Update()
    {
        _currentRotation = _currentRotation + (_rotationSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.AngleAxis(_currentRotation, _rotationAxis);
    }
}