using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.Rendering;

public class CardSlotTube : MachineMovablePart
{
    [Header("REFERENCES")]
    [SerializeField] private CardSlotTubeArm _arm;
    [SerializeField] private CardSlotHandle[] _handles;
    [SerializeField] private CardSlotGlassTube _glassTube;
    [SerializeField] private CardSlotTubeLid[] _tubeLid;

    [Header("PARAMETERS")]
    [SerializeField] private float _delayToHandle;
    [SerializeField] private float _handlePerDelay;
    [SerializeField] private float _glassTubeDelay;

    public override void Init()
    {

    }

    public override IEnumerator EnterAnimation()
    {
        StartCoroutine(_arm.EnterAnimation());
        yield return new WaitForSeconds(_delayToHandle);

        for(int i = 0; i < _handles.Length; i++)
        {
            StartCoroutine(_handles[i].EnterAnimation());
            yield return new WaitForSeconds(_handlePerDelay);
        }

        for (int i = 0; i < _tubeLid.Length; i++)
        {
            StartCoroutine(_tubeLid[i].EnterAnimation());
        }
        yield return new WaitForSeconds(_glassTubeDelay);
        StartCoroutine(_glassTube.EnterAnimation());
    }

    public override IEnumerator ExitAnimation()
    {
        yield return null;
    }
}
