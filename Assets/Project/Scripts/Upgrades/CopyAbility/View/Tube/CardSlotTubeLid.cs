using DG.Tweening;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSlotTubeLid : MachineMovablePart
{
    [Header("REFERENCES")]
    [SerializeField] private Transform _lid;
    [SerializeField] private bool _hasButtons;
    [SerializeField, ShowIf("_hasButtons")] private float _delayUntilButtons;
    [SerializeField, ShowIf("_hasButtons")] private Transform _lidButtons;
    [SerializeField, ShowIf("_hasButtons")] private TweenConfig _buttonsTweenConfig;
    [SerializeField, ShowIf("_hasButtons")] private RotateMode _rotationMode;

    [Header("PARAMETERS")]
    [SerializeField] private TweenConfig _spinConfig;

    public override void Init()
    {

    }

    public override IEnumerator EnterAnimation()
    {
        Spin();
        if(_hasButtons)
        {
            yield return new WaitForSeconds(_delayUntilButtons);
            _lidButtons.DOLocalRotate(_buttonsTweenConfig.Value, _buttonsTweenConfig.Duration, _rotationMode).SetEase(_buttonsTweenConfig.Ease);
        }
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
