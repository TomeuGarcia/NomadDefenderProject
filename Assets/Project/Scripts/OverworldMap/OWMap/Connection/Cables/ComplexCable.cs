using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplexCable : ConnectionCable
{
    [SerializeField] protected MaterialLerp.FloatData secondLerpData;

    public override void FillCable(bool destroyed)
    {
        base.FillCable(destroyed);

        StartCoroutine(ConsecutiveFill());
    }

    private IEnumerator ConsecutiveFill()
    {
        yield return StartCoroutine(MaterialLerp.FloatLerp(lerpData, new Material[2] { cableMaterials[0], cableMaterials[1] }));
        yield return StartCoroutine(MaterialLerp.FloatLerp(secondLerpData, new Material[2] { cableMaterials[2], cableMaterials[3] }));
    }
}
