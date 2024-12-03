using Project.Scripts.Turrets.Visuals;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "TurretAbility_ExtraSellValuePerHit", 
    menuName = SOAssetPaths.CARDS_ABILITIES + "ExtraSellValuePerHit")]
public class TPADataModel_ExtraSellValuePerHit : ATurretPassiveAbilityDataModel
{
    [Header("ABILITY CONFIG")] 
    [SerializeField] private AbilityDescriptionVariable _hitsToIncrement;
    [SerializeField] private AbilityDescriptionVariable _sellValueIncrementAmount;

    
    public AbilityDescriptionVariable HitsToIncrement => _hitsToIncrement;
    public AbilityDescriptionVariable SellValueIncrementAmount => _sellValueIncrementAmount;


    public override ATurretPassiveAbility MakePassiveAbility()
    {
        return new TurretPassiveAbility_ExtraSellValuePerHit(this);
    }
    
}