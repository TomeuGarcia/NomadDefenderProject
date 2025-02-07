
using UnityEngine;
using System.Threading.Tasks;

public class TurretPassiveAbility_Berserker : ATurretPassiveAbility
{
    private readonly TPADataModel_Berserker _abilityDataModel;
    private TurretBuilding _turretOwner;
    
    private readonly TurretStatsMultiplicationSnapshot _hyperStatsMultiplier = 
        new TurretStatsMultiplicationSnapshot(-0.25f, 3f, 3.0f);


    private BerserkerTurretBuildingVisuals _berserkerVisuals = null;
    
    private bool _isTurretPlaced;
    
    private float _berserkCountdownTimer;
    private bool _isInBerserkerMode;
    
    
    public TurretPassiveAbility_Berserker(TPADataModel_Berserker originalModel) 
        : base(originalModel)
    {
        _abilityDataModel = originalModel;
        _isTurretPlaced = false;
        _isInBerserkerMode = false;
        
        UpdateDescriptionVariable(_abilityDataModel.BerserkerDuration);
    }

    public override void OnTurretCreated(TurretBuilding turretOwner)
    {
        _turretOwner = turretOwner;
        
        PathLocation.OnTakeDamage += OnPathLocationTakesDamage;
    }

    public override void OnTurretDestroyed()
    {
        PathLocation.OnTakeDamage -= OnPathLocationTakesDamage;
        if (_isInBerserkerMode)
        {
            ResetStats();
            _isInBerserkerMode = false;
        }
    }

    protected override void OnTurretPlaced()
    {
        if (_berserkerVisuals != null)
        {
            return;
        }
        
        _berserkerVisuals = GameObject.Instantiate(_abilityDataModel.VisualsPrefab, _turretOwner.transform);
        _berserkerVisuals.TurretPlacedInit(_turretOwner, _turretOwner.MaterialForTurret);

        _isTurretPlaced = true;
    }

    public override void OnTurretUnplaced()
    {
        _berserkCountdownTimer = 0f;
    }


    protected override void DoOnBeforeShootingEnemyEnd(ATurretProjectileBehaviour projectile)
    {
        if (_isInBerserkerMode)
        {
            AddViewAddOnToProjectile(_abilityDataModel.BerserkerActiveAddOn, projectile);
        }
    }

    private void OnPathLocationTakesDamage(PathLocation pathLocation)
    {
        if (!_isTurretPlaced) return;
        
        EnterBerserkMode();
    }

    private void EnterBerserkMode()
    {
        _berserkCountdownTimer += _abilityDataModel.BerserkerDuration.Value;
        _berserkCountdownTimer = Mathf.Min(_berserkCountdownTimer, _abilityDataModel.MaxBerserkerDuration);

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

        while (_berserkCountdownTimer > 0.0f)
        {
            _berserkCountdownTimer -= GameTime.DeltaTime;
            await Task.Yield();
        }
        _berserkCountdownTimer = 0.0f;

        _berserkerVisuals.StopBerserkVisuals();

        if (_isInBerserkerMode)
        {
            ResetStats();
        }
        
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