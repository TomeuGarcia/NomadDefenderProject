using System;
using System.Threading.Tasks;

public class TurretPassiveAbility_SpawnCardCopyInDeck : ATurretPassiveAbility
{
    private TurretBuilding _turretOwner;
    
    private readonly TPADataModel_SpawnCardCopyInDeck _abilityDataModel;

    
    public TurretPassiveAbility_SpawnCardCopyInDeck(TPADataModel_SpawnCardCopyInDeck originalModel) 
        : base(originalModel)
    {
        _abilityDataModel = originalModel;
        ApplyDescriptionCorrection(_abilityDataModel.CostIncrement);
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
        turretCardData.IncrementPlayCost(_abilityDataModel.CostIncrement.Value);
        
        ServiceLocator.GetInstance().CardDrawer.SpawnTurretCardInDeck(turretCardData);
    }
}