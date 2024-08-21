using System.Collections.Generic;

public abstract class ATurretPassiveAbility
{
    public readonly TurretPassiveAbilityDataModel OriginalModel;
    public string AbilityName => _description.Name;
    public string AbilityDescription
    {
        get
        {
            _description.ApplyDescriptionModifications(_descriptionCorrections);
            _descriptionCorrections.Clear();
            return _description.Description;
        }
    }

    private readonly TurretAbilityDescription _description;
    private readonly Dictionary<string, int> _descriptionCorrections;


    protected ATurretPassiveAbility(TurretPassiveAbilityDataModel originalModel)
    {
        OriginalModel = originalModel;
        
        _description = new TurretAbilityDescription(OriginalModel.Name, OriginalModel.Description);
        _descriptionCorrections = new Dictionary<string, int>();
    }

    protected void ApplyDescriptionCorrection(string keyword, int value)
    {
        if (_descriptionCorrections.ContainsKey(keyword))
        {
            _descriptionCorrections[keyword] = value;
            return;
        }
        
        _descriptionCorrections.Add(keyword, value);
    }
    
    
    public virtual void OnAddedToTurretCard(TurretCardData cardData) { } 
    public virtual void OnRemovedFromTurretCard() { } 
    public virtual void OnTurretCardUpgraded(TurretCardData cardData) { } 
    
    public virtual void OnTurretCreated(TurretBuilding turretOwner) { } 
    public virtual void OnTurretDestroyed() { } 
    
    public virtual void OnTurretPlacingStart() { } 
    public virtual void OnTurretPlacingFinish() { } 
    public virtual void OnTurretPlacingMove() { } 
    public virtual void OnTurretPlaced() { } 
    
    public virtual void OnBeforeShootingEnemy(TurretDamageAttack damageAttack) { } 
    public virtual void OnAfterShootingEnemy(TurretDamageAttackResult damageAttackResult) { } 
}