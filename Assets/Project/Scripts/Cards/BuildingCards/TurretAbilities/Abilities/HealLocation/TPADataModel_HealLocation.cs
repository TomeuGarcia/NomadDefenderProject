using UnityEngine;

[CreateAssetMenu(fileName = "TurretAbility_Restore", 
    menuName = SOAssetPaths.CARDS_ABILITIES + "Restore")]
public class TPADataModel_HealLocation : ATurretPassiveAbilityDataModel
{
    [Header("ABILITY CONFIG")] 
    [SerializeField] private AbilityDescriptionVariable _healAmount;
    
    public AbilityDescriptionVariable HealAmount => _healAmount;


    public override ATurretPassiveAbility MakePassiveAbility()
    {
        return new TurretPassiveAbility_HealLocation(this);
    }
}