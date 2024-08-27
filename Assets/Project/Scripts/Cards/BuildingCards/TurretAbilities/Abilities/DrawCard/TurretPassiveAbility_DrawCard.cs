using System;
using System.Threading.Tasks;

public class TurretPassiveAbility_DrawCard : ATurretPassiveAbility
{

    public TurretPassiveAbility_DrawCard(TPADataModel_DrawCard originalModel) 
        : base(originalModel)
    {
    }

    public override void OnTurretPlaced()
    {
        DoDrawCard();
    }

    private async void DoDrawCard()
    {
        float delayBeforeDrawing = 0.5f;
        await Task.Delay(TimeSpan.FromSeconds(delayBeforeDrawing));
        
        ServiceLocator.GetInstance().CardDrawer.UtilityTryDrawAnyRandomCard(1.5f);
    }


}