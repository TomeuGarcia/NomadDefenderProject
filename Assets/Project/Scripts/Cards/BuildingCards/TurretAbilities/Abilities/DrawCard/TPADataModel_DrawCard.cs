
using UnityEngine;

[CreateAssetMenu(fileName = "TurretAbility_DrawCard", 
    menuName = SOAssetPaths.CARDS_ABILITIES + "DrawCard")]
public class TPADataModel_DrawCard : ATurretPassiveAbilityDataModel
{
    public override ATurretPassiveAbility MakePassiveAbility()
    {
        return new TurretPassiveAbility_DrawCard(this);
    }
}