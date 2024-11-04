using Project.Scripts.Turrets.Visuals;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretAbility_Drain", 
    menuName = SOAssetPaths.CARDS_ABILITIES + "Drain")]
public class TPADataModel_HealLocationPerKills : ATurretPassiveAbilityDataModel
{
    [Header("ABILITY CONFIG")] 
    [SerializeField] private AbilityDescriptionVariable _healAmount;
    [SerializeField] private AbilityDescriptionVariable _startingKills;
    [SerializeField] private AbilityDescriptionVariable _killsIncrease;

    [Header("VISUALS")] 
    [SerializeField] private HealLocationPerKillsTurretBuildingVisuals _visualsPrefab;
    
    public AbilityDescriptionVariable HealAmount => _healAmount;
    public AbilityDescriptionVariable StartingKills => _startingKills;
    public AbilityDescriptionVariable KillsIncrease => _killsIncrease;
    public HealLocationPerKillsTurretBuildingVisuals VisualsPrefab => _visualsPrefab;


    public override ATurretPassiveAbility MakePassiveAbility()
    {
        return new TurretPassiveAbility_HealLocationPerKills(this);
    }
}