using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegularCable : ConnectionCable
{
    public override void FillCable(bool destroyed) 
    {
        base.FillCable(destroyed);
        StartCoroutine(MaterialLerp.FloatLerp(lerpData, cableMaterials.ToArray()));
    }
}
