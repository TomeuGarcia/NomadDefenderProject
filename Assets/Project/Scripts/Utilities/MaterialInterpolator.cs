using System;
using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public static class MaterialInterpolator
{
    public static void Setup(Material material, MaterialFloatSetupConfig[] setupDatas)
    {
        foreach (var data in setupDatas)
        {
            material.SetFloat(data.Name, data.InitialValue);
        }
    }

    public static IEnumerator ApplyInterpolations(Material material, MaterialFloatInterpolationConfig[] interpolationDatas)
    {
        int i = 0;
        foreach (var data in interpolationDatas)
        {
            yield return new WaitForSeconds(data.Delay);
            material.DOFloat(data.EndValue, data.Name, data.Duration).SetEase(data.Ease);
            i++;

            if (data.WaitForCompletion || i == (interpolationDatas.Length))
            {
                yield return new WaitForSeconds(data.Duration);
            }
        }
    }
}