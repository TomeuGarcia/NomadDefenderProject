using UnityEngine;

[CreateAssetMenu(fileName = "TurretAbility_StockGenerator", 
    menuName = SOAssetPaths.CARDS_ABILITIES + "StockGenerator")]
public class TPADataModel_WaveFinishSpawnCardCopyInHand : ATurretPassiveAbilityDataModel
{
    [Header("ABILITY CONFIG")] 
    [SerializeField] private AbilityDescriptionVariable _costIncrementPerCard;
    public AbilityDescriptionVariable CostIncrementPerCard => _costIncrementPerCard;


    public override ATurretPassiveAbility MakePassiveAbility()
    {
        return new TurretPassiveAbility_WaveFinishSpawnCardCopyInHand(this);
    }
}