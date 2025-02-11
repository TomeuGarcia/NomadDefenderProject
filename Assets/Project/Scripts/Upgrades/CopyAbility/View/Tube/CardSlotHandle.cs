using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSlotHandle : MachineMovablePart
{
    [Header("TWEENS")]
    [SerializeField] private new Vector3 _enterRotation;
    [SerializeField] private float _enterRotationDuration = 1.0f;
    [SerializeField] private Ease _enterRotationEase;

    [SerializeField] private Transform _plugPivot;
    [SerializeField] private Vector3 _plugEndRotation;
    [SerializeField] private float _plugEndRotationDuration;
    [SerializeField] private Ease _plugRotationEase;
    [SerializeField] private float _plugWaitTime;

    public override void Init()
    {

    }

    public override IEnumerator EnterAnimation()
    {
        transform.DOLocalRotate(_enterRotation, _enterRotationDuration, RotateMode.Fast).SetEase(_enterRotationEase);
        yield return new WaitForSeconds(_plugWaitTime);

        _plugPivot.DOLocalRotate(_plugEndRotation, _plugEndRotationDuration, RotateMode.FastBeyond360).SetEase(_plugRotationEase);
    }

    public override IEnumerator ExitAnimation()
    {
        yield return null;
    }
}
