using System.Collections.Generic;

public abstract class ATurretPassiveAbility
{
    public const int MAX_AMOUNT_FOR_TURRET = 3;
    
    
    public readonly ATurretPassiveAbilityDataModel OriginalModel;
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
    private readonly Dictionary<string, string> _descriptionCorrections;


    protected ATurretPassiveAbility(ATurretPassiveAbilityDataModel originalModel)
    {
        OriginalModel = originalModel;
        
        _description = new TurretAbilityDescription(OriginalModel.Name, OriginalModel.Description);
        foreach (CardAbilityKeyword descriptionKeyword in OriginalModel.DescriptionKeywords)
        {
            descriptionKeyword.ApplyDescriptionModifications(_description);
        }
        
        _descriptionCorrections = new Dictionary<string, string>();
    }

    protected void ApplyDescriptionCorrection(string name, int value)
    {
        ApplyDescriptionCorrection(name, value.ToString());
    }
    protected void ApplyDescriptionCorrection(AbilityDescriptionVariable constantVariable)
    {
        ApplyDescriptionCorrection(constantVariable.Name, constantVariable.ValueAsString());
    }
    private void ApplyDescriptionCorrection(string name, string value)
    {
        if (_descriptionCorrections.ContainsKey(name))
        {
            _descriptionCorrections[name] = value;
            return;
        }
        
        _descriptionCorrections.Add(name, value);
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
    
    public virtual void OnBeforeShootingEnemy() { } 
    public virtual void OnBeforeDamagingEnemy(TurretDamageAttack damageAttack) { } 
    public virtual void OnAfterDamagingEnemy(TurretDamageAttackResult damageAttackResult) { } 
}