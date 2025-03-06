using UnityEngine;

public class TurretPassiveAbility_ElectricWire : ATurretPassiveAbility,
    ElectricWireSegment.IEnemyDamageData
{
    private readonly TPADataModel_ElectricWire _originalModel;
    private TurretBuilding _turretOwner;
    private bool _isSubscribed; 

    public TurretPassiveAbility_ElectricWire(TPADataModel_ElectricWire originalModel) : 
        base(originalModel)
    {
        _originalModel = originalModel;
        _isSubscribed = false;
        
        UpdateDescriptionVariable(_originalModel.NumberOfPassives);
        UpdateDescriptionVariable(_originalModel.WireDamage);
        UpdateDescriptionVariable(_originalModel.WireStunDuration);
        UpdateDescriptionVariable(_originalModel.StackedWireDamage);
        UpdateDescriptionVariable(_originalModel.StackedWireStunDuration);
    }

    public int GetDamage()
    {
        return _originalModel.WireDamage.Value;
    }

    public float GetStunDuration()
    {
        return _originalModel.WireStunDuration.FloatValue;
    }

    public Gradient GetColorOverLifetimeGradient()
    {
        return _originalModel.WireColorGradient;
    }

    public int GetStackedDamage()
    {
        return _originalModel.StackedWireDamage.Value;
    }

    public float GetStackedStunDuration()
    {
        return _originalModel.StackedWireStunDuration.FloatValue;
    }

    public Gradient GetStackedColorOverLifetimeGradient()
    {
        return _originalModel.StackedWireColorGradient;
    }


    public Vector3 GetOriginPosition()
    {
        return _turretOwner.Position;
    }


    
    public override void OnTurretCreated(TurretBuilding turretOwner)
    {
        _turretOwner = turretOwner;
    }

    public override void OnCardDestroyed()
    {
        UnsubscribeEventsWhenPlaced();
    }

    protected override void OnTurretPlaced()
    {
        Building[] placedBuildings = BuildingPlacer.GetCurrentPlacedBuildings();
        foreach (Building placedBuilding in placedBuildings)
        {
            TryCreateSegmentWithBuilding(placedBuilding);
        }
        
        SubscribeEventsWhenPlaced();
    }

    public override void OnTurretUnplaced()
    {
        ElectricWiresManager.Instance.RemoveAllSegments(_turretOwner);
        
        UnsubscribeEventsWhenPlaced();
    }


    private void TryCreateSegmentWithBuilding(Building placedBuilding)
    {
        if (placedBuilding is TurretBuilding placedTurret && placedTurret != _turretOwner)
        {
            bool hasEnoughPassives = placedTurret.CardData.PassiveAbilitiesController.CurrentNumberOfPassives 
                                     >= _originalModel.NumberOfPassives.Value;

            if (hasEnoughPassives)
            {
                ElectricWiresManager.Instance.CreateSegment(this, _turretOwner, placedTurret);
            }
        }
    }
    
    private void TryRemoveSegmentWithBuilding(Building placedBuilding)
    {
        if (placedBuilding is TurretBuilding placedTurret && placedTurret != _turretOwner)
        {
            ElectricWiresManager.Instance.RemoveSegment(_turretOwner, placedTurret);
        }
    }
    
    private void SubscribeEventsWhenPlaced()
    {
        if (_isSubscribed) return;
        
        _isSubscribed = true;
        BuildingPlacer.OnTurretBuildingPlaced += TryCreateSegmentWithBuilding;
        BuildingPlacer.OnBuildingUnplaced += TryRemoveSegmentWithBuilding;
    }
    private void UnsubscribeEventsWhenPlaced()
    {
        if (!_isSubscribed) return;

        _isSubscribed = false;
        BuildingPlacer.OnTurretBuildingPlaced -= TryCreateSegmentWithBuilding;
        BuildingPlacer.OnBuildingUnplaced -= TryRemoveSegmentWithBuilding;
    }
    
}