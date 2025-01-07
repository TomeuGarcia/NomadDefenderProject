using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public class ReplacingAnimation : MonoBehaviour
{
    [SerializeField] private ReplacingAnimationTube _replacingAnimationTube;
    [SerializeField] private List<CardSlotTubeLid> _lids = new();
    [SerializeField] private Transform _slider;

    [Header("PARAMETERS")]
    [SerializeField] private float _sliderDelay;
    [SerializeField] private TweenConfig _sliderTweenConfig;

    private MachineManager _machineManager;

    public void Init(MachineManager machineManager)
    {
        _machineManager = machineManager;
    }

    public IEnumerator StartAnimation()
    {
        foreach(CardSlotTubeLid lid in _lids)
        {
            StartCoroutine(lid.ReplaceAnimation());
        }
        yield return new WaitForSeconds(_sliderDelay);

        _slider.DOBlendableLocalMoveBy(_sliderTweenConfig.Value, _sliderTweenConfig.Duration).SetEase(_sliderTweenConfig.Ease);
        _machineManager.LowerTubes();
        _replacingAnimationTube.ActivateLeftTube();
        yield return new WaitForSeconds(_sliderTweenConfig.Duration);

        StartCoroutine(_replacingAnimationTube.PlayTubeFlash());
    }
}
