using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretDamageStatStateValuePerser : ITurretStatStateValuePerser
{
    public float ParseValue(float rawValue)
    {
        return Mathf.Floor(rawValue);
    }

    public string ParseValueToString(float rawValue)
    {
        return ParseValue(rawValue).ToString("G");
    }
}
