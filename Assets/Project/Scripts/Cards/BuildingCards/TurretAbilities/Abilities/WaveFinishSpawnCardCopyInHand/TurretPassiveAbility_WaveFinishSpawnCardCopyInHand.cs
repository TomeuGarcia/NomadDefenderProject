

using System.Threading.Tasks;

public class TurretPassiveAbility_WaveFinishSpawnCardCopyInHand : ATurretPassiveAbility
{
    private TurretBuilding _turretOwner;
    private bool _isSubscribed;
    
    private readonly TPADataModel_WaveFinishSpawnCardCopyInHand _abilityDataModel;

    
    
    public TurretPassiveAbility_WaveFinishSpawnCardCopyInHand(TPADataModel_WaveFinishSpawnCardCopyInHand originalModel) 
        : base(originalModel)
    {
        _abilityDataModel = originalModel;
        ApplyDescriptionCorrection(_abilityDataModel.CostIncrementPerCard);
    }

    public override void OnTurretCreated(TurretBuilding turretOwner)
    {
        _turretOwner = turretOwner;
        SubscribeEvents();
    }

    public override void OnTurretDestroyed()
    {
        UnsubscribeEvents();
    }

    protected override void OnTurretPlaced()
    {
        UnsubscribeEvents();
    }


    private void SubscribeEvents()
    {
        if (_isSubscribed) return;
        _isSubscribed = true;

        EnemyWaveManager.OnStartNewWaves += SpawnCardCopy;
    }

    private void UnsubscribeEvents()
    {
        if (!_isSubscribed) return;
        _isSubscribed = false;

        EnemyWaveManager.OnStartNewWaves -= SpawnCardCopy;
    }


    private async void SpawnCardCopy()
    {
        await Task.Delay(System.TimeSpan.FromSeconds(0.1f));

        CardDrawer cardDrawer = ServiceLocator.GetInstance().CardDrawer;
        int numberOfCards = cardDrawer.GetCardsInHand().Length + 1;
        
        TurretCardData turretCardDataCopy = new TurretCardData(_turretOwner.CardData, true);
        turretCardDataCopy.RemovePassiveAbility(OriginalModel);
        turretCardDataCopy.IncrementPlayCost(_abilityDataModel.CostIncrementPerCard.Value * numberOfCards);
        
        ServiceLocator.GetInstance().CardDrawer.SpawnTurretCardInHand(turretCardDataCopy);
    }
}