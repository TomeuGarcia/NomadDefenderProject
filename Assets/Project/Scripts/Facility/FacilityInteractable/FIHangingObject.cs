using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FIHangingObject : AFacilityInteractable
{
    [Header("REFERENCES")]
    [SerializeField] private Transform _transformForceA;
    [SerializeField] private Transform _transformForceB;

    [Header("PARAMETERS")]
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Vector2 _strenghtRange;

    private Vector3 _positionDifference;

    private void Awake()
    {
        _positionDifference = _transformForceB.position - _transformForceA.position;
    }

    protected override IEnumerator DoInteract()
    {
        float randomCoef = Random.Range(0.0f, 1.0f);
        _rb.AddForceAtPosition(new Vector3(0, Random.Range(_strenghtRange.x, _strenghtRange.y), 0), _transformForceA.position + _positionDifference * randomCoef, ForceMode.Impulse);

        yield return _duration;
    }
}
