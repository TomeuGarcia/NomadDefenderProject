using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTargetingPassives : BasePassive
{
    public override void Init(RangeBuilding owner)
    {
        owner.SetEnemySortFunction(SortingFunction);
        passiveType = PassiveType.TARGETING;
    }

    protected abstract int SortingFunction(Enemy e1, Enemy e2);
}
