using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegularCable : ConnectionCable
{
    public override void FillCable(bool destroyed) 
    {
        if(destroyed) { foreach (Material mat in cableMaterials) { mat.SetFloat("_Broken", 1.0f); } }
        StartCoroutine(MaterialLerp.FloatLerp(lerpData, cableMaterials.ToArray()));
    }
}
