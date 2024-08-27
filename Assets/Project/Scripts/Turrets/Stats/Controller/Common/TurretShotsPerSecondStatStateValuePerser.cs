using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretShotsPerSecondStatStateValuePerser : ITurretStatStateValuePerser
{
    public float ParseValue(float rawValue)
    {
        return 1f / rawValue;
    }

    public string ParseValueToString(float rawValue)
    {
        return TurretStatStateDecimalParseHelper.ApplyFormat(rawValue);
    }
}
