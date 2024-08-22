
using UnityEngine;
using System.Threading.Tasks;

public class TurretPassiveAbility_Berserker : ATurretPassiveAbility
{
    private readonly TPADataModel_Berserker _abilityDataModel;
    private TurretBuilding _turretOwner;
    
    private readonly TurretStatsMultiplicationSnapshot _hyperStatsMultiplier = 
        new TurretStatsMultiplicationSnapshot(0.0f, 4f, 3.0f);


    private BerserkerTurretBuildingVisuals _berserkerVisuals;
    
    private bool _isTurretPlaced;
    
    private float _berserkTimer;
    private bool _isInBerserkerMode;
    
    
    public TurretPassiveAbility_Berserker(TPADataModel_Berserker originalModel) 
        : base(originalModel)
    {
        _abilityDataModel = originalModel;
        _isTurretPlaced = false;
        _isInBerserkerMode = false;
        
        ApplyDescriptionCorrection(_abilityDataModel.BerserkerDuration);
    }

    public override void OnTurretCreated(TurretBuilding turretOwner)
    {
        _turretOwner = turretOwner;
        
        PathLocation.OnTakeDamage += OnPathLocationTakesDamage;
    }

    public override void OnTurretDestroyed()
    {
        PathLocation.OnTakeDamage -= OnPathLocationTakesDamage;
    }

    public override void OnTurretPlaced()
    {
        _berserkerVisuals = GameObject.Instantiate(_abilityDataModel.VisualsPrefab, _turretOwner.transform);
        _berserkerVisuals.TurretPlacedInit(_turretOwner, _turretOwner.MaterialForTurret);

        _isTurretPlaced = true;
    }

    public override void OnBeforeDamagingEnemy(TurretDamageAttack damageAttack)
    {
        // TODO if _isInBerserkerMode true, add projectile visuals 
    }


    private void OnPathLocationTakesDamage(PathLocation pathLocation)
    {
        if (!_isTurretPlaced) return;
        
        EnterBerserkMode();
    }

    private void EnterBerserkMode()
    {
        _berserkTimer += _abilityDataModel.BerserkerDuration.Value;

        if (!_isInBerserkerMode)
        {
            BerserkMode();
        }
    }

    private async void BerserkMode()
    {
        _isInBerserkerMode = true;
        
        SetupHyperStats();
        _berserkerVisuals.StartBerserkVisuals();

        while (_berserkTimer > 0.0f)
        {
            _berserkTimer -= GameTime.DeltaTime;
            await Task.Yield();
        }
        _berserkTimer = 0.0f;

        ResetStats();
        _berserkerVisuals.StopBerserkVisuals();
        _isInBerserkerMode = false;
    }

    private void SetupHyperStats()
    {
        _turretOwner.StatsBonusController.AddBonusBaseStatsMultiplication(_hyperStatsMultiplier);
    }

    private void ResetStats()
    {
        _turretOwner.StatsBonusController.RemoveBonusBaseStatsMultiplication(_hyperStatsMultiplier);
    }
    
    
}