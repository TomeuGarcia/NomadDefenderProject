

using System.Threading.Tasks;

public class TurretPassiveAbility_WaveFinishSpawnCardCopyInHand : ATurretPassiveAbility
{
    private readonly int _costIncrementPerCard = 10;
    private TurretBuilding _turretOwner;
    private bool _isSubscribed;
    
    public TurretPassiveAbility_WaveFinishSpawnCardCopyInHand(TurretPassiveAbilityDataModel originalModel) 
        : base(originalModel)
    {
        ApplyDescriptionCorrection("COST_INCREMENT_VALUE", _costIncrementPerCard);
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

    public override void OnTurretPlaced()
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
        
        TurretCardData turretCardDataCopy = new TurretCardData(_turretOwner.CardData);
        turretCardDataCopy.RemovePassiveAbility(OriginalModel);
        turretCardDataCopy.IncrementPlayCost(_costIncrementPerCard * numberOfCards);
        
        ServiceLocator.GetInstance().CardDrawer.SpawnTurretCardInHand(turretCardDataCopy);
    }
}