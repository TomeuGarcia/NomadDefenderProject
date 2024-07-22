using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ExtraHPDamagePassive", menuName = "TurretPassives/ExtraHPDamagePassive")]
public class ExtraHPDamagePassive : BaseDamagePassive
{
    public override int PassiveDamageModifier(int damage, HealthSystem healthSystem)
    {
        if(healthSystem.HasArmor())
        {
            return damage;
        }

        return (int)((float)damage * 1.25f);
    }
}
