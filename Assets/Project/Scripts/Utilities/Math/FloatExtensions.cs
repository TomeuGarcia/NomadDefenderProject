using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FloatExtensions
{
    public static bool AlmostEquals(this float float1, float float2, float precision)
    {
        return (Mathf.Abs(float1 - float2) <= precision);
    }
    public static bool AlmostZero(this float float1)
    {
        return float1.AlmostEquals(0f, 0.0001f);
    }
}
