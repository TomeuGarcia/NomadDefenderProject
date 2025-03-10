

using UnityEngine;

public class TurretPassiveAbility_ReduceCostPerOthersKills : ATurretPassiveAbility
{
    private readonly TPADataModel_ReduceCostPerOthersKills _abilityDataModel;
    private bool _isSubscribed;
    private int _killsCount;
    private TurretBuildingCard _ownerCard;
    private int _originalCardPlayCost;
    
    public TurretPassiveAbility_ReduceCostPerOthersKills(TPADataModel_ReduceCostPerOthersKills originalModel) 
        : base(originalModel)
    {
        _abilityDataModel = originalModel;
        UpdateDescriptionVariable(_abilityDataModel.PlayCostDecrementAmount);
        UpdateDescriptionVariable(_abilityDataModel.KillsToDecrement);
        _killsCount = 0;
    }

    public override void OnTurretCreated(TurretBuilding turretOwner)
    {
        SubscribeEvents();
    }
    
    public override void OnTurretDestroyed()
    {
        UnsubscribeEvents();
        _ownerCard.UpdatePlayCost(_originalCardPlayCost);
    }

    protected override void OnTurretPlaced()
    {
        UnsubscribeEvents();
        _killsCount = 0;
        _ownerCard.UpdatePlayCost(_originalCardPlayCost);
    }


    public override void OnCardInitialized(TurretBuildingCard ownerCard)
    {
        _ownerCard = ownerCard;
        _originalCardPlayCost = _ownerCard.CardData.PlayCost;
    }

    private void SubscribeEvents()
    {
        if (_isSubscribed) return;
        _isSubscribed = true;

        Enemy.OnTakeDamageResult += OnEnemyTakesDamage;
    }

    private void UnsubscribeEvents()
    {
        if (!_isSubscribed) return;
        _isSubscribed = false;

        Enemy.OnTakeDamageResult -= OnEnemyTakesDamage;
    }


    private void OnEnemyTakesDamage(TurretDamageAttackResult attackResult)
    {
        if (!attackResult.HitKilled)
        {
            return;
        }
        
        ++_killsCount;
        if (_killsCount < _abilityDataModel.KillsToDecrement.Value)
        {
            return;
        }

        _killsCount = 0;
        DecrementCardPlayCost();
    }


    private void DecrementCardPlayCost()
    {
        _ownerCard.PlayUpdatePlayCostAnimation(-_abilityDataModel.PlayCostDecrementAmount.Value);
        AchievementDefinitions.NegativePlayCostCard.Check(_ownerCard.CardData.PlayCost);
        
        ServiceLocator.GetInstance().ParticleFactory
            .Create(ParticleTypes.ReduceCardCostWhileInHand, _ownerCard.CardParticlesSpot.position, Quaternion.identity)
            .GetComponent<RecyclableParticles>();
    }
    


}