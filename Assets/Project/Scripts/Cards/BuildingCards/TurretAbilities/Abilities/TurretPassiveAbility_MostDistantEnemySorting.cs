public abstract class ATurretPassiveAbility_AEnemySorting : ATurretPassiveAbility
{
    
    
    public override void ApplyEffects(TurretBuilding owner)
    {
        owner.SetEnemySortFunction(SortingFunction);
        turretOwner = owner;
    }

    protected abstract int SortingFunction(Enemy e1, Enemy e2);

    
}