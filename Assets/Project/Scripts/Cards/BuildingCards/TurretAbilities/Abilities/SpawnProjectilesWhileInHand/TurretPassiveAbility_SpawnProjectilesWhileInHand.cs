

using System;
using System.Threading.Tasks;
using UnityEngine;

public class TurretPassiveAbility_SpawnProjectilesWhileInHand : ATurretPassiveAbility
{
    private readonly TPADataModel_SpawnProjectilesWhileInHand _abilityDataModel;
    private bool _isSubscribed;
    private TurretBuildingCard _ownerCard;

    private RecyclableParticles _cardParticles;

    
    
    public TurretPassiveAbility_SpawnProjectilesWhileInHand(TPADataModel_SpawnProjectilesWhileInHand originalModel) 
        : base(originalModel)
    {
        _abilityDataModel = originalModel;
    }

    public override void OnCardInitialized(TurretBuildingCard ownerCard)
    {
        _ownerCard = ownerCard;
    }

    public override void OnTurretCreated(TurretBuilding turretOwner)
    {
        SubscribeEvents();
    }

    public override void OnDrawnToHandTwiceOrMore()
    {
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

        EnemyWaveManager.OnStartNewWaves += OnNewWaveStarted;
        Enemy.OnTakeDamageResult += OnEnemyTakesDamage;

        SpawnCardParticles();
    }

    private void UnsubscribeEvents()
    {
        if (!_isSubscribed) return;
        _isSubscribed = false;

        EnemyWaveManager.OnStartNewWaves -= OnNewWaveStarted;
        Enemy.OnTakeDamageResult -= OnEnemyTakesDamage;


        ClearCardParticles();
    }

    private void OnNewWaveStarted()
    {
        UnsubscribeEvents();
    }

    private void OnEnemyTakesDamage(TurretDamageAttackResult attackResult)
    {
        if (!attackResult.HitKilled && attackResult.DamageAttackSource.ProjectileSource != null)
        {
            return;
        }


        TurretBuilding killerTurret = attackResult.DamageAttackSource.ProjectileSource.TurretOwner;
        TurretPartProjectileDataModel projectileDataModel = _abilityDataModel.ProjectileDataModel;
        float delayBetweenSpawns = _abilityDataModel.DelayBetweenSpawns;

        SpawnProjectiles(killerTurret, projectileDataModel, 1, delayBetweenSpawns);
    }

    private async void SpawnProjectiles(TurretBuilding killerTurret, TurretPartProjectileDataModel projectileDataModel,
        int spawnAmount, float delayBetweenSpawns)
    {
        for (int i = 0; i < spawnAmount; ++i)
        {
            projectileDataModel.ShootingControllerCreator.SpawnDynamically(killerTurret, projectileDataModel);
            await Task.Delay(TimeSpan.FromSeconds(delayBetweenSpawns));
        }
    }


    private void SpawnCardParticles()
    {
        _cardParticles =
            ServiceLocator.GetInstance().ParticleFactory
                .Create(ParticleTypes.SpawnOrbitingsWhileInHand, _ownerCard.CardParticlesSpot.position, Quaternion.identity)
                .GetComponent<RecyclableParticles>();

        _cardParticles.transform.SetParent(_ownerCard.CardParticlesSpot);
    }
    private void ClearCardParticles()
    {
        _cardParticles.Stop();
        _cardParticles.transform.SetParent(ServiceLocator.GetInstance().ParticleFactory.ParticleParent);
    }
}