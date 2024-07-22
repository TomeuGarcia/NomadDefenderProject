using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuildingUpgradesController
{
    int CurrentUpgradeLevel { get; }
    void IncrementUpgradeLevel();
}
