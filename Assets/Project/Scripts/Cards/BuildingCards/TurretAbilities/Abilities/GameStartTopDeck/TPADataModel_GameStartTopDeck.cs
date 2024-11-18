using UnityEngine;

[CreateAssetMenu(fileName = "TurretAbility_GameStartTopDeck", 
    menuName = SOAssetPaths.CARDS_ABILITIES + "GameStartTopDeck")]
public class TPADataModel_GameStartTopDeck : ATurretPassiveAbilityDataModel
{

    public override ATurretPassiveAbility MakePassiveAbility()
    {
        return new TurretPassiveAbility_GameStartTopDeck(this);
    }
}