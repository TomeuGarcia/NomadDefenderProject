using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardSlotTubePlug;

public class CardSlotTubePlug : MachineMovablePart
{
    [Serializable]
    public class PlugSegment
    {
        public List<Transform> _segments = new();
    }

    [Header("TWEENS")]
    [SerializeField] private float _delayBetweenSegments;
    [SerializeField] private float _sdStep;
    [SerializeField] private float _sdStepDuration;
    [SerializeField] private Ease _sdStepEase;
    [SerializeField] private float _middleSegmentPause;
    [SerializeField] private List<PlugSegment> _plugSegments = new();

    public override void Init()
    {

    }

    public override IEnumerator EnterAnimation()
    {
        for(int i = 0; i < _plugSegments.Count; i++)
        {
            StartCoroutine(Plug(_plugSegments[i]));

            if (i == 0)
            {
                yield return new WaitForSeconds(_middleSegmentPause);
            }
        }
    }

    private IEnumerator Plug(PlugSegment plugSegment)
    {
        foreach (Transform segment in plugSegment._segments)
        {
            segment.DOLocalMoveX(segment.localPosition.x - _sdStep, _sdStepDuration)
                .SetEase(_sdStepEase);

            yield return new WaitForSeconds(_sdStepDuration + _delayBetweenSegments);
        }
    }

    public override IEnumerator ExitAnimation()
    {
        yield return null;
    }
}
