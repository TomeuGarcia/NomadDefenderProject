using System;
using System.Threading.Tasks;
using UnityEngine;

public class TurretPassiveAbility_ExpandRangeThrive : ATurretPassiveAbility
{
    private TurretBuilding _turretOwner;
    private TPADataModel_ExpandRangeThrive _originalModel;
    
    public TurretPassiveAbility_ExpandRangeThrive(TPADataModel_ExpandRangeThrive originalModel) 
        : base(originalModel)
    {
        _originalModel = originalModel;
        ApplyDescriptionCorrection(originalModel.RadiusRangeIncrement);
    }

    public override void OnTurretCreated(TurretBuilding turretOwner)
    {
        _turretOwner = turretOwner;
        BuildingPlacer.OnRangedBuildingPlaced += OnRangedBuildingPlaced;
    }

    public override void OnTurretDestroyed()
    {
        BuildingPlacer.OnRangedBuildingPlaced -= OnRangedBuildingPlaced;
    }
    
    private void OnRangedBuildingPlaced(RangeBuilding rangeBuilding)
    {
        if (!_turretOwner.IsPlaced || _turretOwner == rangeBuilding)
        {
            return;
        }

        if (rangeBuilding.CurrentRadiusRange > _turretOwner.CurrentRadiusRange)
        {
            ExpandOwnerRange(rangeBuilding);
        }
    }
    

    private void ExpandOwnerRange(RangeBuilding otherPlacedBuilding)
    {
        _turretOwner.AddExtraRadiusRangeAndUpdate(_originalModel.RadiusRangeIncrement.FloatValue);

        ExpandRangeThriveParticles particles =
        ServiceLocator.GetInstance().ParticleFactory
            .Create(ParticleTypes.ExpandTurretRadiusAbility, _turretOwner.Position, Quaternion.identity)
            .GetComponent<ExpandRangeThriveParticles>();
        
        particles.Play(otherPlacedBuilding.Position, _turretOwner.Position, _turretOwner.CurrentRadiusRange);
    }


}