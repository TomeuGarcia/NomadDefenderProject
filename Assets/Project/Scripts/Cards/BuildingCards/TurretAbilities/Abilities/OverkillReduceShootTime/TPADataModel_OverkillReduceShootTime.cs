using UnityEngine;

[CreateAssetMenu(fileName = "TurretAbility_OverkillReduceShootTime", 
    menuName = SOAssetPaths.CARDS_ABILITIES + "OverkillReduceShootTime")]
public class TPADataModel_OverkillReduceShootTime : ATurretPassiveAbilityDataModel
{
    public override ATurretPassiveAbility MakePassiveAbility()
    {
        return new TurretPassiveAbility_OverkillReduceShootTime(this);
    }
}