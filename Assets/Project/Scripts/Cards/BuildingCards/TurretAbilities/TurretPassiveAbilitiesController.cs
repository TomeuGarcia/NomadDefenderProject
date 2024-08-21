using System.Collections.Generic;

public class TurretPassiveAbilitiesController : ITurretPassiveAbilitiesNotifier
{
    private readonly List<ATurretPassiveAbility> _passiveAbilities;


    public TurretPassiveAbilitiesController()
    {
        _passiveAbilities = new List<ATurretPassiveAbility>(3);
    }
    
    public TurretPassiveAbilitiesController(TurretPassiveAbilitiesController other,
        ITurretAbilityFactory abilityFactory)
    {
        _passiveAbilities = new List<ATurretPassiveAbility>(other._passiveAbilities.Count);
        foreach (ATurretPassiveAbility passiveAbility in _passiveAbilities)
        {
            AddPassiveAbility(abilityFactory.MakePassiveAbilityFromType(passiveAbility.AbilityType));
        }
    }

    public void AddPassiveAbility(ATurretPassiveAbility passiveAbility)
    {
        _passiveAbilities.Add(passiveAbility);
        passiveAbility.OnAddedToTurretCard();
    }
    

    public void OnTurretPlaced()
    {
        foreach (var passiveAbility in _passiveAbilities)
        {
            passiveAbility.OnTurretPlaced();
        }
    }

    public void OnBeforeShootingEnemy()
    {
        foreach (var passiveAbility in _passiveAbilities)
        {
            passiveAbility.OnBeforeShootingEnemy();
        }
    }

    public void OnAfterShootingEnemy()
    {
        foreach (var passiveAbility in _passiveAbilities)
        {
            passiveAbility.OnAfterShootingEnemy();
        }
    } 
}