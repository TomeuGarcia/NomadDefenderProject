using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSlotTubeArm : MachineMovablePart
{
    [Header("REFERENCES")]
    [SerializeField] private CardSlotTubePlug _plug;

    [Header("TWEENS")]
    [SerializeField] private new Vector3 _enterRotation;
    [SerializeField] private float _enterRotationDuration = 1.0f;
    [SerializeField] private Ease _enterRotationEase;
    [SerializeField] private float _plugWaitTime;

    public override void Init()
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator EnterAnimation()
    {
        transform.DOLocalRotate(_enterRotation, _enterRotationDuration, RotateMode.Fast).SetEase(_enterRotationEase);
        yield return new WaitForSeconds(_plugWaitTime);

        StartCoroutine(_plug.EnterAnimation());
    }

    public override IEnumerator ExitAnimation()
    {
        throw new System.NotImplementedException();
    }
}
