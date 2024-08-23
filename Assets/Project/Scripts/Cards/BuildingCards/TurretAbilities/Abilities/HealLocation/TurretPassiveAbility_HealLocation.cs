using System;
using System.Threading.Tasks;

public class TurretPassiveAbility_HealLocation : ATurretPassiveAbility
{
    private TurretBuilding _turretOwner;
    private TurretBinder _binder;

    private readonly TPADataModel_HealLocation _abilityDataModel;
    private int _healAmount;
    
    public TurretPassiveAbility_HealLocation(TPADataModel_HealLocation originalModel) 
        : base(originalModel)
    {
        _abilityDataModel = originalModel;
        _healAmount = 1;
        ApplyDescriptionCorrection(_abilityDataModel.HealAmount.Name, _healAmount);
    }


    public override void OnAddedToTurretCard(TurretCardData cardData)
    {
        _healAmount = cardData.CardUpgradeLevel;
        ApplyDescriptionCorrection(_abilityDataModel.HealAmount.Name, _healAmount);
    }

    public override void OnTurretCardUpgraded(TurretCardData cardData)
    {
        _healAmount = cardData.CardUpgradeLevel;
        ApplyDescriptionCorrection(_abilityDataModel.HealAmount.Name, _healAmount);
    }

    public override void OnTurretCreated(TurretBuilding turretOwner)
    {
        _turretOwner = turretOwner;
        PathLocation.OnHealthChanged += OnPathLocationHealthChanged;
    }

    public override void OnTurretDestroyed()
    {
        PathLocation.OnHealthChanged -= OnPathLocationHealthChanged;
    }

    public override void OnTurretPlacingStart()
    {
        ShowNewBinder();
        ConnectBinderWithPathLocation();
    }

    public override void OnTurretPlacingFinish()
    {
        HideBinder();
    }

    public override void OnTurretPlacingMove()
    {
        ConnectBinderWithPathLocation();
    }

    public override void OnTurretPlaced()
    {
        HideBinder();
        DoHeal();
    }
    

    private async void DoHeal()
    {
        float delayBeforeHealing = 0.3f;
        await Task.Delay(TimeSpan.FromSeconds(delayBeforeHealing));

        if (ServiceLocator.GetInstance().TDLocationsUtils.
            GetMostDamagedLocation(_turretOwner.Position, out PathLocation mostDamagedLocation))
        {
            mostDamagedLocation.Heal(_healAmount);
        }        
    }




    private void ShowNewBinder()
    {
        _binder = ServiceLocator.GetInstance().TDTurretBinderHelper.TakeBinder(TDTurretBinderHelper.BinderType.HEAL_TARGET);
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
        if (ServiceLocator.GetInstance().TDLocationsUtils.GetMostDamagedLocation(_turretOwner.Position, 
                out PathLocation mostDamagedLocation))
        {
            TurretBinderUtils.UpdateTurretBinder(_binder.Transform, 
                mostDamagedLocation.transform, _turretOwner.BinderPointTransform);
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