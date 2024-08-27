using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TurretStatStateDecimalParseHelper
{
    public static string ApplyFormat(float parsedValue)
    {
        return parsedValue.ToString("0.0").Replace(',', '.');
    }
}
