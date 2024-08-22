using UnityEngine;

[CreateAssetMenu(fileName = "TurretAbility_Berserker", 
    menuName = SOAssetPaths.CARDS_ABILITIES + "Berserker")]
public class TPADataModel_Berserker : ATurretPassiveAbilityDataModel
{
    [Header("ABILITY CONFIG")]
    [SerializeField] private BerserkerTurretBuildingVisuals _berserkerVisualsPrefab;
    [SerializeField] private AbilityDescriptionVariable _berserkerDuration;
    
    
    public AbilityDescriptionVariable BerserkerDuration => _berserkerDuration;
    public BerserkerTurretBuildingVisuals VisualsPrefab => _berserkerVisualsPrefab;
    
    
    public override ATurretPassiveAbility MakePassiveAbility()
    {
        return new TurretPassiveAbility_Berserker(this);
    }
}