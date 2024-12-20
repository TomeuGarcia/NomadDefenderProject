using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.Rendering;

public class CardSlotTube : MachineMovablePart
{
    [SerializeField] private CardSlotTubeArm arm;

    public override void Init()
    {

    }

    public override IEnumerator EnterAnimation()
    {
        StartCoroutine(arm.EnterAnimation());
        yield return null;
    }

    public override IEnumerator ExitAnimation()
    {
        StartCoroutine(arm.ExitAnimation());
        yield return null;
    }
}
