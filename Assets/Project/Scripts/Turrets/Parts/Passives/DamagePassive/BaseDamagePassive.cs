using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseDamagePassive : BasePassive
{
    public override void ApplyEffects(TurretBuilding owner)
    {
        owner.baseDamagePassive = PassiveDamageModifier;
    }

    public abstract int PassiveDamageModifier(int damage, HealthSystem healthSystem);
}

