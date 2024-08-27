using System;
using System.Threading.Tasks;

public class TurretPassiveAbility_SelfHurtUpgradeStats : ATurretPassiveAbility
{
    private TurretBuilding _turretOwner;
    private TurretBinder _binder;
    private bool _isSubscribed;

    private readonly TPADataModel_SelfHurtUpgradeStats _abilityDataModel;
    
    public TurretPassiveAbility_SelfHurtUpgradeStats(TPADataModel_SelfHurtUpgradeStats originalModel) 
        : base(originalModel)
    {
        _abilityDataModel = originalModel;
        ApplyDescriptionCorrection(_abilityDataModel.DamageAmount);
    }


    public override void OnTurretCreated(TurretBuilding turretOwner)
    {
        _turretOwner = turretOwner;
    }

    public override void OnTurretPlacingStart()
    {
        SubscribeEvents();
        
        ShowNewBinder();
        ConnectBinderWithPathLocation();
    }

    public override void OnTurretPlacingFinish()
    {
        UnsubscribeEvents();

        HideBinder();
    }

    public override void OnTurretPlacingMove()
    {
        ConnectBinderWithPathLocation();
    }


    public override void OnTurretPlaced()
    {
        UnsubscribeEvents();
        DoDamageAndUpgrade();
    }


    

    private void SubscribeEvents()
    {
        if (_isSubscribed) return;
        _isSubscribed = true;

        PathLocation.OnHealthChanged += OnPathLocationHealthChanged;
    }
    private void UnsubscribeEvents()
    {
        if (!_isSubscribed) return;
        _isSubscribed = false;
        
        PathLocation.OnHealthChanged -= OnPathLocationHealthChanged;
    }
    
    private async void DoDamageAndUpgrade()
    {
        await Task.Delay(TimeSpan.FromSeconds(0.3f));

        _turretOwner.Upgrader.FreeTurretUpgrade();

        if (ServiceLocator.GetInstance().TDLocationsUtils
            .GetHealthiestLocation(_turretOwner.Position, out PathLocation pathLocation))
        {
            pathLocation.TakeDamage(_abilityDataModel.DamageAmount.Value);
        }

        HideBinder();
    }




    private void ShowNewBinder()
    {
        _binder = ServiceLocator.GetInstance().TDTurretBinderHelper.TakeBinder(TDTurretBinderHelper.BinderType.HURT_TARGET);
        _binder.Show();
    }
    private void HideBinder()
    {
        if (_binder == null) return;

        _binder.Hide();
        ServiceLocator.GetInstance().TDTurretBinderHelper.GiveBackBinder(_binder);
        _binder = null;
    }

    private void ConnectBinderWithPathLocation()
    {
        if (ServiceLocator.GetInstance().TDLocationsUtils
            .GetHealthiestLocation(_turretOwner.Position, out PathLocation pathLocation))
        {
            TurretBinderUtils.UpdateTurretBinder(_binder.Transform, 
                pathLocation.transform, _turretOwner.BinderPointTransform);
        }        
    }

    private void OnPathLocationHealthChanged()
    {
        if (_binder != null)
        {
            ConnectBinderWithPathLocation();
        }
    }
}