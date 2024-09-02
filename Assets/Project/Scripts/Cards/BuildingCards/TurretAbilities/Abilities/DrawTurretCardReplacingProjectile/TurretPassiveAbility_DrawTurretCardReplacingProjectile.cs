using System.Threading.Tasks;

public class TurretPassiveAbility_DrawTurretCardReplacingProjectile : ATurretPassiveAbility
{
    private TurretBuilding _turretOwner;
    
    public TurretPassiveAbility_DrawTurretCardReplacingProjectile(TPADataModel_DrawTurretCardReplacingProjectile originalModel) 
        : base(originalModel)
    {
    }

    public override void OnTurretCreated(TurretBuilding turretOwner)
    {
        _turretOwner = turretOwner;
    }

    protected override void OnTurretPlaced()
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
                
        turretCard.InBattleReplaceAttack(_turretOwner.ProjectileDataModel, 1.5f);
    }
}