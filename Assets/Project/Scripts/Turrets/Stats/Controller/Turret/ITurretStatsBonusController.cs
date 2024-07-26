using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITurretStatsBonusController
{
    void AddBonusBaseStatsMultiplication(TurretStatsMultiplicationSnapshot bonusStatsMultiplication);
    void RemoveBonusBaseStatsMultiplication(TurretStatsMultiplicationSnapshot bonusStatsMultiplication);
}
