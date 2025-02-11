using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VictoryGrid : MonoBehaviour
{
    [SerializeField] private Transform _gridA;
    [SerializeField] private Transform _gridB;

    [SerializeField] private float _maxZ = -13.0f;
    [SerializeField] private float _minZ = 9.0f;
    private float _difZ;

    [SerializeField] private float _speed = 9.0f;

    private Vector3 _resetPosition;

    private void Awake()
    {
        _difZ = _maxZ - _minZ;
    }

    private void Update()
    {
        MoveGrid(_gridA);
        MoveGrid(_gridB);
    }

    private void MoveGrid(Transform transform)
    {
        transform.localPosition += Vector3.forward * (_speed * Time.deltaTime);

        if(transform.localPosition.z > _maxZ)
        {
            transform.localPosition -= Vector3.forward * (_difZ);
        }
    }
}
