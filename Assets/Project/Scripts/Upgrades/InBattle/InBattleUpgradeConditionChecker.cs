using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface InBattleUpgradeConditionChecker
{
    bool HasEnoughCurrencyToLevelUp();
}
