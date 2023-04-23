using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BaseNullPassive", menuName = "TurretPassives/BaseNullPassive")]
public class BaseNullPassive : BasePassive
{
    public override void ApplyEffects(TurretBuilding owner) { }
}
