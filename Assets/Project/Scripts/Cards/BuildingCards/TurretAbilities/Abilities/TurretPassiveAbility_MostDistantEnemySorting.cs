using UnityEngine;

public class TurretPassiveAbility_MostDistantEnemySorting : ATurretPassiveAbility
{
    private TurretBuilding _turretOwner;
    
    
    public TurretPassiveAbility_MostDistantEnemySorting(TurretPassiveAbilityDataModel originalModel) 
        : base(originalModel)
    {
    }

    public override void OnTurretCreated(TurretBuilding turretOwner)
    {
        _turretOwner = turretOwner;
        _turretOwner.SetEnemySortFunction(SortEnemiesPrioritizingMostDistant);
    }

    private int SortEnemiesPrioritizingMostDistant(Enemy e1, Enemy e2)
    {
        Vector3 turretPosition = _turretOwner.Position;
        
        float distanceE1 = Vector3.Distance(e1.Position, turretPosition);
        distanceE1 += e1.GetTargetPriorityBonus();

        float distanceE2 = Vector3.Distance(e2.Position, turretPosition);
        distanceE2 += e2.GetTargetPriorityBonus();

        return distanceE1.CompareTo(distanceE2);
    }


    
}