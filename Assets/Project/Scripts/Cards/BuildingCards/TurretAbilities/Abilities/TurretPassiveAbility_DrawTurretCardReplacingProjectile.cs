using System.Threading.Tasks;

public class TurretPassiveAbility_DrawTurretCardReplacingProjectile : ATurretPassiveAbility
{
    private TurretBuilding _turretOwner;
    
    public TurretPassiveAbility_DrawTurretCardReplacingProjectile(TurretPassiveAbilityDataModel originalModel) 
        : base(originalModel)
    {
    }

    public override void OnTurretCreated(TurretBuilding turretOwner)
    {
        _turretOwner = turretOwner;
    }

    public override void OnTurretPlaced()
    {
        DoDrawCard();
    }
    
    private async void DoDrawCard()
    {
        await Task.Delay(500);
        
        BuildingCard card = ServiceLocator.GetInstance().CardDrawer
            .UtilityTryDrawRandomCardOfType(BuildingCard.CardBuildingType.TURRET, 2.25f);

        if (card == null) return;

        TurretBuildingCard turretCard = card as TurretBuildingCard;
                
        turretCard.InBattleReplaceAttack(_turretOwner.TurretPartAttack, 1.5f);
    }
}