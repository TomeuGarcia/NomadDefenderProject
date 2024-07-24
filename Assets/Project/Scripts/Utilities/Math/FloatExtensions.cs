using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FloatExtensions
{
    public static bool AlmostEquals(this float float1, float float2, float precision = 0.0001f)
    {
        return (Mathf.Abs(float1 - float2) <= precision);
    }
    public static bool AlmostZero(this float value)
    {
        return value.AlmostEquals(0f);
    }
    
    public static float ToSine01(this float value)
    {
        return (Mathf.Sin(value * Mathf.PI - (Mathf.PI / 2)) + 1) / 2;
    }
}
