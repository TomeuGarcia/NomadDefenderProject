using UnityEngine;

[CreateAssetMenu(fileName = "TurretAbility_ReduceCostPerOthersKills", 
    menuName = SOAssetPaths.CARDS_ABILITIES + "ReduceCostPerOthersKills")]
public class TPADataModel_ReduceCostPerOthersKills : ATurretPassiveAbilityDataModel
{
    [Header("ABILITY CONFIG")] 
    [SerializeField] private AbilityDescriptionVariable _killsToDecrement;
    [SerializeField] private AbilityDescriptionVariable _playCostDecrementAmount;
    
    public AbilityDescriptionVariable PlayCostDecrementAmount => _playCostDecrementAmount;
    public AbilityDescriptionVariable KillsToDecrement => _killsToDecrement;

    public override ATurretPassiveAbility MakePassiveAbility()
    {
        return new TurretPassiveAbility_ReduceCostPerOthersKills(this);
    }
}