using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryJoiningGrid : MonoBehaviour
{
    [SerializeField] private Transform _joinGridparent;
    [SerializeField] private Transform _gridA;
    [SerializeField] private Transform _gridB;

    [SerializeField] private float _waitTime;

    [Header("X")]
    [SerializeField] private float _movementTime;
    [SerializeField] private float _movementStep;
    [SerializeField] private Ease _easing;

    private Vector3 _initialLocalPosition;

    private void Start()
    {
        StartCoroutine(Movement());
    }

    private IEnumerator Movement()
    {
        while(true)
        {
            _gridA.DOLocalMoveZ(-_movementStep, _movementTime).SetEase(_easing);
            _gridB.DOLocalMoveZ(_movementStep, _movementTime).SetEase(_easing);
            yield return new WaitForSeconds(_movementTime);

            _gridA.localPosition = _initialLocalPosition;
            _gridB.localPosition = _initialLocalPosition;
            _joinGridparent.localRotation = Quaternion.Euler(0, (_joinGridparent.localRotation.eulerAngles.y + 90) % 360, 0);
            yield return new WaitForSeconds(_waitTime);
        }
    }
}
