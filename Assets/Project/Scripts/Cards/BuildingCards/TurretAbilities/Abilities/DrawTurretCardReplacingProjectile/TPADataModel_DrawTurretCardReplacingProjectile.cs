
using UnityEngine;

[CreateAssetMenu(fileName = "TurretAbility_Reinforcements", 
    menuName = SOAssetPaths.CARDS_ABILITIES + "Reinforcements")]
public class TPADataModel_DrawTurretCardReplacingProjectile : ATurretPassiveAbilityDataModel
{
    
    public override ATurretPassiveAbility MakePassiveAbility()
    {
        return new TurretPassiveAbility_DrawTurretCardReplacingProjectile(this);
    }
}