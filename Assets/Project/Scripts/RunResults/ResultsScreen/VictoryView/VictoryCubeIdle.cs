using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryCubeIdle : MonoBehaviour
{
    [SerializeField] private Transform _parentTransform;
    [SerializeField] private Transform _outerTransform;
    [SerializeField] private Transform _innerTransform;

    [SerializeField] private float _sinSpeed;
    [SerializeField] private float _sinOffset;
    [SerializeField] private Vector3 _sinAxis;
    [SerializeField] private Vector3 _innerRotationSpeed;
    [SerializeField] private Vector3 _outerRotationSpeed;

    private Vector3 _innerRotation = Vector3.zero;
    private Vector3 _outerRotation = Vector3.zero;

    private Vector3 _initPos;

    private float _timer = 0.0f;

    private void Awake()
    {
        _initPos = _parentTransform.position;
        _timer += _sinOffset;
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        _parentTransform.position = _initPos + _sinAxis * Mathf.Sin(_timer * _sinSpeed);

        _innerRotation += _innerRotationSpeed * Time.deltaTime;
        _outerRotation += _outerRotationSpeed * Time.deltaTime;

        _innerTransform.rotation = Quaternion.Euler(_innerRotation);
        _outerTransform.rotation = Quaternion.Euler(_outerRotation);
    }
}
