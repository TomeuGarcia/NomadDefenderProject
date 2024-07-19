using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITurretStatsBonusController
{
    void AddBonusStats(TurretStatsSnapshot bonusStats);
    void RemoveBonusStats(TurretStatsSnapshot bonusStats);
}
