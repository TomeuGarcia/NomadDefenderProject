using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MaterialLerp
{
    //////// FLOAT
    [System.Serializable]
    public class FloatData
    {
        public float time;
        public float min;
        public float max;
        public AnimationCurve curve;
        public bool invert;
        public string variableReference;
        public float endGoal;
    }


    public static IEnumerator FloatLerp(FloatData lerpData, Material[] materials)
    {
        float tParam;
        float currentTime = 0.0f;
        float dif = lerpData.max - lerpData.min;

        while (currentTime < lerpData.time)
        {
            currentTime += Time.deltaTime;

            tParam = lerpData.curve.Evaluate(currentTime / lerpData.time);
            tParam = lerpData.invert == true ? 1 - tParam : tParam;

            tParam = lerpData.min + dif * tParam;


            foreach (Material material in materials)
            {
                material.SetFloat(lerpData.variableReference, tParam);
            }

            yield return null;
        }

        foreach (Material material in materials)
        {
            material.SetFloat(lerpData.variableReference, lerpData.endGoal);
        }
    }
    //////// FLOAT
}
