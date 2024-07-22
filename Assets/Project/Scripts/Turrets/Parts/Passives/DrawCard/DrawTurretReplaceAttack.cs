using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


[CreateAssetMenu(fileName = "DeployDrawTurretCardReplaceAttack", menuName = "TurretPassives/DeployDrawTurretCardReplaceAttack")]
public class DrawTurretReplaceAttack : BasePassive
{
    private TurretBuilding owner;

    public override void ApplyEffects(TurretBuilding owner)
    {
        this.owner = owner;
        owner.OnGotPlaced += DrawCard;
        owner.OnDestroyed += UnsubscriveEvents;
    }

    private void UnsubscriveEvents()
    {
        owner.OnGotPlaced -= DrawCard;
        owner.OnDestroyed -= UnsubscriveEvents;
    }



    private void DrawCard()
    {
        DoDrawCard();
    }

    private async void DoDrawCard()
    {
        await Task.Delay(500);
        BuildingCard card = ServiceLocator.GetInstance().CardDrawer.UtilityTryDrawRandomCardOfType(BuildingCard.CardBuildingType.TURRET, 2.25f);

        if (card == null) return;

        TurretBuildingCard turretCard = card as TurretBuildingCard;
                
        turretCard.InBattleReplaceAttack(owner.TurretPartAttack, 1.5f);
    }

}
