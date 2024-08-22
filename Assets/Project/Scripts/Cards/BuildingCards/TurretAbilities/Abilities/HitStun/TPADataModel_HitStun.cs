
using UnityEngine;

[CreateAssetMenu(fileName = "TurretAbility_HighVoltage", 
    menuName = SOAssetPaths.CARDS_ABILITIES + "HighVoltage")]
public class TPADataModel_HitStun : ATurretPassiveAbilityDataModel
{
    [Header("ABILITY CONFIG")] 
    [SerializeField] private AbilityDescriptionVariable _stunDuration;
    public AbilityDescriptionVariable StunDuration => _stunDuration;
    
    public override ATurretPassiveAbility MakePassiveAbility()
    {
        return new TurretPassiveAbility_HitStun(this);
    }
}