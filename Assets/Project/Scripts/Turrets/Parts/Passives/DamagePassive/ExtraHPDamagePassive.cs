using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ExtraHPDamagePassive", 
    menuName = SOAssetPaths.TURRET_PARTS_BASEPASSIVES + "ExtraHPDamagePassive")]
public class ExtraHPDamagePassive : BaseDamagePassive
{
    protected override int PassiveDamageModifier(int damage, HealthSystem healthSystem)
    {
        if(healthSystem.HasArmor())
        {
            return damage;
        }

        return (int)((float)damage * 1.25f);
    }
}
