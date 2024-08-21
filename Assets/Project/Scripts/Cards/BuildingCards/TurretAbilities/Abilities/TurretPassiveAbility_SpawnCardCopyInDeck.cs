using System;
using System.Threading.Tasks;

public class TurretPassiveAbility_SpawnCardCopyInDeck : ATurretPassiveAbility
{
    private TurretBuilding _turretOwner;
    private readonly int _costIncrement = 50;
    
    
    public TurretPassiveAbility_SpawnCardCopyInDeck(TurretPassiveAbilityDataModel originalModel) 
        : base(originalModel)
    {
        ApplyDescriptionCorrection("COST_INCREMENT_VALUE", _costIncrement);
    }

    public override void OnTurretCreated(TurretBuilding turretOwner)
    {
        _turretOwner = turretOwner;
    }

    public override void OnTurretPlaced()
    {
        SpawnCardCopy();
    }
    
    private async void SpawnCardCopy()
    {
        float delayBeforeSpawningCard = 0.5f;
        await Task.Delay(TimeSpan.FromSeconds(delayBeforeSpawningCard));

        TurretCardData turretCardData = new TurretCardData(_turretOwner.CardData);
        turretCardData.IncrementPlayCost(_costIncrement);
        
        ServiceLocator.GetInstance().CardDrawer.SpawnTurretCardInDeck(turretCardData);
    }
}