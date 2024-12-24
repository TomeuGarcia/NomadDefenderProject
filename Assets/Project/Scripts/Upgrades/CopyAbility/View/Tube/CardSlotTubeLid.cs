using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSlotTubeLid : MachineMovablePart
{
    [Header("REFERENCES")]
    [SerializeField] private Transform _lid;

    [Header("PARAMETERS")]
    [SerializeField] private TweenConfig _spinConfig;

    public override void Init()
    {

    }

    public override IEnumerator EnterAnimation()
    {
        Spin();
        yield return null;
    }

    public void Spin(int spinCount = 2)
    {
        _lid.DOBlendableRotateBy(_spinConfig.Value * spinCount, _spinConfig.Duration, RotateMode.FastBeyond360).SetEase(_spinConfig.Ease);
    }

    public override IEnumerator ExitAnimation()
    {
        yield return null;
    }
}
