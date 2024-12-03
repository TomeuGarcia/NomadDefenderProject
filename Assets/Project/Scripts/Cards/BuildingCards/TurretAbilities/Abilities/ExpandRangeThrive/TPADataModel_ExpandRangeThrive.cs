
using UnityEngine;

[CreateAssetMenu(fileName = "TurretAbility_ExpandRangeThrive", 
    menuName = SOAssetPaths.CARDS_ABILITIES + "ExpandRangeThrive")]
public class TPADataModel_ExpandRangeThrive : ATurretPassiveAbilityDataModel
{
    [Header("ABILITY CONFIG")] 
    [SerializeField] private AbilityDescriptionVariable _radiusRangeIncrement;
    
    public AbilityDescriptionVariable RadiusRangeIncrement => _radiusRangeIncrement;
    
    public override ATurretPassiveAbility MakePassiveAbility()
    {
        return new TurretPassiveAbility_ExpandRangeThrive(this);
    }
    
}