using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITurretStatState
{
    float BaseValue { get; }
    string BaseValueText { get; }

    float GetValueByLevel(int upgradeLevel);
    string GetValueTextByLevel(int upgradeLevel);
}
