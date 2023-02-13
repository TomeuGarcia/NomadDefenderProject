using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTargetingPassive : BasePassive
{
    protected TurretBuilding turretOwner;

    public override void ApplyEffects(TurretBuilding owner)
    {
        owner.SetEnemySortFunction(SortingFunction);
        turretOwner = owner;
    }

    protected abstract int SortingFunction(Enemy e1, Enemy e2);
}
