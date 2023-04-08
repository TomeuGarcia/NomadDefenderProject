using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeEnemy : Enemy
{
    private bool canBeTargeted = true;

    public void SetCanBeTargeted(bool canBeTargeted)
    {
        this.canBeTargeted = canBeTargeted;
    }

    public override bool CanBeTargeted()
    {
        return canBeTargeted;
    }
    public override float GetTargetPriorityBonus()
    {
        return 0f;
    }
}
