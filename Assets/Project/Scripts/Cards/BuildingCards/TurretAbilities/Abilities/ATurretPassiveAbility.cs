public abstract class ATurretPassiveAbility
{
    public readonly TurretPassiveAbilityDataModel OriginalModel;
    public string Name => OriginalModel.Name;
    public string Description => OriginalModel.Description;


    protected ATurretPassiveAbility(TurretPassiveAbilityDataModel originalModel)
    {
        OriginalModel = originalModel;
    }
    
    
    public virtual void OnAddedToTurretCard() { } 
    
    public virtual void OnTurretPlaced() { } 
    
    public virtual void OnBeforeShootingEnemy() { } 
    public virtual void OnAfterShootingEnemy() { } 
}