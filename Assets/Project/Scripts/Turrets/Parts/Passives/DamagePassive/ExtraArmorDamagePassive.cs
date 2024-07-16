using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ExtraArmorDamagePassive", 
    menuName = SOAssetPaths.TURRET_PARTS_BASEPASSIVES + "ExtraArmorDamagePassive")]
public class ExtraArmorDamagePassive : BaseDamagePassive
{
    public override int PassiveDamageModifier(int damage, HealthSystem healthSystem)
    {
        if (!healthSystem.HasArmor())
        {
            return damage;
        }

        return (int)((float)damage * 2.0f);
    }
}
