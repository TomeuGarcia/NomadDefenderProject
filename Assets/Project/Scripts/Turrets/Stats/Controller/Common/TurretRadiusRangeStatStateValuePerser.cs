using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretRadiusRangeStatStateValuePerser : ITurretStatStateValuePerser
{
    public float ParseValue(float rawValue)
    {
        return rawValue;
    }

    public string ParseValueToString(float rawValue)
    {
        return TurretStatStateDecimalParseHelper.ApplyFormat(ParseValue(rawValue));
    }
}
