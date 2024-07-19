using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BaseNullPassive", 
    menuName = SOAssetPaths.TURRET_PARTS_BASEPASSIVES + "BaseNullPassive")]
public class BaseNullPassive : BasePassive
{
    public override void ApplyEffects(TurretBuilding owner) { }
}
