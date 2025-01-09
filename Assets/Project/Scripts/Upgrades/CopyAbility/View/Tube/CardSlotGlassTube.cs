using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSlotGlassTube : MachineMovablePart
{
    //TODO - GLASS COMING UP ANIMATION? and particles and stuff

    [SerializeField] private Transform _glassTube;
    [SerializeField] private MachineTweenConfig _closeTubeConfig;
    [SerializeField] private MachineTweenConfig _openTubeConfig;

    public override void Init()
    {

    }

    public override IEnumerator EnterAnimation()
    {
        StartCoroutine(OpenTube());
        yield return null;
    }

    public IEnumerator OpenTube()
    {
        _glassTube.DOLocalMoveY(_openTubeConfig.Value, _openTubeConfig.Duration).SetEase(_openTubeConfig.Ease);
        yield return null;
    }

    public IEnumerator CloseTube()
    {
        _glassTube.DOLocalMoveY(_closeTubeConfig.Value, _closeTubeConfig.Duration).SetEase(_closeTubeConfig.Ease);
        yield return null;
    }

    public override IEnumerator ExitAnimation()
    {
        yield return null;
    }
}
