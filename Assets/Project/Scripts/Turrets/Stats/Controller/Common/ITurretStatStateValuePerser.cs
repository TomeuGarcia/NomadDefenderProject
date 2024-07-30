using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITurretStatStateValuePerser
{
    float ParseValue(float rawValue);
    string ParseValueToString(float rawValue);
}
