using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ExtraArmorDamagePassive", 
    menuName = SOAssetPaths.TURRET_PARTS_BASEPASSIVES + "ExtraArmorDamagePassive")]
public class ExtraArmorDamagePassive : BaseDamagePassive
{
    protected override int PassiveDamageModifier(int damage, HealthSystem healthSystem)
    {
        if (healthSystem.HasArmor())
        {
            damage *= 2;
        }

        return damage;
    }
}
