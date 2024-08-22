
using UnityEngine;

[CreateAssetMenu(fileName = "TurretAbility_SharpShooter", 
    menuName = SOAssetPaths.CARDS_ABILITIES + "SharpShooter")]
public class TPADataModel_MostDistantEnemySorting : ATurretPassiveAbilityDataModel
{
    
    public override ATurretPassiveAbility MakePassiveAbility()
    {
        return new TurretPassiveAbility_MostDistantEnemySorting(this);
    }
}