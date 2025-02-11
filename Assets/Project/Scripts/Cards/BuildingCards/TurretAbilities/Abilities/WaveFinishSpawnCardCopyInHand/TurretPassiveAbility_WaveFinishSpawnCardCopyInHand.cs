

using System.Threading.Tasks;
using UnityEngine;

public class TurretPassiveAbility_WaveFinishSpawnCardCopyInHand : ATurretPassiveAbility
{
    private bool _isSubscribed;
    
    private readonly TPADataModel_WaveFinishSpawnCardCopyInHand _abilityDataModel;
    private TurretBuildingCard _ownerCard;

    public TurretPassiveAbility_WaveFinishSpawnCardCopyInHand(TPADataModel_WaveFinishSpawnCardCopyInHand originalModel) 
        : base(originalModel)
    {
        _abilityDataModel = originalModel;
        UpdateDescriptionVariable(_abilityDataModel.CostIncrementPerCard);
    }

    public override void OnCardInitialized(TurretBuildingCard ownerCard)
    {
        _ownerCard = ownerCard;
    }
    
    public override void OnDrawnToHandTwiceOrMore()
    {
        SubscribeEvents();
    }

    public override void OnTurretCreated(TurretBuilding turretOwner)
    {
        SubscribeEvents();
    }

    protected override void OnTurretPlaced()
    {
        UnsubscribeEvents();
    }


    public override void OnCardDestroyed()
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


    private void SpawnCardCopy()
    {
        DoSpawnCardCopy();
    }
    private async void DoSpawnCardCopy()
    {
        if (_ownerCard.cardLocation != BuildingCard.CardLocation.HAND)
        {
            return;
        }
        
        
        CardDrawer cardDrawer = ServiceLocator.GetInstance().CardDrawer;
        int numberOfCards = cardDrawer.GetCardsInHand().Length + 1;
        
        await Task.Delay(System.TimeSpan.FromSeconds(0.1f));
        ServiceLocator.GetInstance().ParticleFactory
            .Create(ParticleTypes.SpawnCardCopyInHand_SourceCard, _ownerCard.CardParticlesSpot.position, Quaternion.identity)
            .SetParent(_ownerCard.CardParticlesSpot);


        await Task.Delay(System.TimeSpan.FromSeconds(0.5f));
        TurretCardData turretCardDataCopy = new TurretCardData(_ownerCard.CardData, true);
        turretCardDataCopy.RemovePassiveAbility(OriginalModel);
        turretCardDataCopy.IncrementPlayCost(_abilityDataModel.CostIncrementPerCard.Value * numberOfCards);
        
        TurretBuildingCard spawnedCard = ServiceLocator.GetInstance().CardDrawer.SpawnTurretCardInHand(turretCardDataCopy);
        
    }
}