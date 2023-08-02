using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


[CreateAssetMenu(fileName = "DeployDrawCard", menuName = "TurretPassives/DeployDrawCard")]
public class BaseDrawCard : BasePassive
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
        ServiceLocator.GetInstance().CardDrawer.UtilityTryDrawAnyRandomCard(1.5f);
    }

}
