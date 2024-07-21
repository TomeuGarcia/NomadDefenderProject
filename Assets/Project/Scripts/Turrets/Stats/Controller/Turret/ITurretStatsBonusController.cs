using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITurretStatsBonusController
{
    void AddBonusStatsMultiplication(TurretStatsMultiplicationSnapshot bonusStatsMultiplication);
    void RemoveBonusStatsMultiplication(TurretStatsMultiplicationSnapshot bonusStatsMultiplication);
}
