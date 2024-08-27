
using UnityEngine;

[CreateAssetMenu(fileName = "TurretAbility_Clone", 
    menuName = SOAssetPaths.CARDS_ABILITIES + "Clone")]
public class TPADataModel_SpawnCardCopyInDeck : ATurretPassiveAbilityDataModel
{
    [Header("ABILITY CONFIG")] 
    [SerializeField] private AbilityDescriptionVariable _costIncrement;
    
    public AbilityDescriptionVariable CostIncrement => _costIncrement;
    
    
    public override ATurretPassiveAbility MakePassiveAbility()
    {
        return new TurretPassiveAbility_SpawnCardCopyInDeck(this);
    }
}