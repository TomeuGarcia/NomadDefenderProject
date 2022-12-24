using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ExtraHPDamagePassive", menuName = "TurretPassives/ExtraHPDamagePassive")]
public class ExtraHPDamagePassive : BaseDamagePassive
{
    public override int PassiveDamageModifier(int damage, HealthSystem healthSystem)
    {
        return (int)((float)damage * 1.2f); //no armor yet so we can just multiply all incoming damage
    }
}
