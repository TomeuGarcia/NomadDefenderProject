public abstract class ATurretPassiveAbility
{
    public TurretPassiveAbilityType AbilityType { get; protected set; }
    public virtual void OnAddedToTurretCard() { } 
    
    public virtual void OnTurretPlaced() { } 
    
    public virtual void OnBeforeShootingEnemy() { } 
    public virtual void OnAfterShootingEnemy() { } 
}