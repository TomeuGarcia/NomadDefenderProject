using System.Collections.Generic;

public class TurretPassiveAbilitiesController : ITurretPassiveAbilitiesNotifier
{
    private readonly List<ATurretPassiveAbility> _passiveAbilities;
    private readonly TurretCardData _cardDataOwner;

    public List<ATurretPassiveAbility> PassiveAbilities => _passiveAbilities;
    public int CurrentNumberOfPassives => _passiveAbilities.Count;

    public TurretPassiveAbilitiesController(TurretCardData cardDataOwner,
        ATurretPassiveAbilityDataModel[] startingPassiveAbilities)
    {
        _cardDataOwner = cardDataOwner;
        
        _passiveAbilities = new List<ATurretPassiveAbility>(startingPassiveAbilities.Length);
        foreach (ATurretPassiveAbilityDataModel passiveAbilityModel in startingPassiveAbilities)
        {
            AddPassiveAbility(passiveAbilityModel);
        }
    }
    
    public TurretPassiveAbilitiesController(TurretCardData cardDataOwner,
        TurretPassiveAbilitiesController other)
    {
        _cardDataOwner = cardDataOwner;

        _passiveAbilities = new List<ATurretPassiveAbility>(other._passiveAbilities.Count);
        foreach (ATurretPassiveAbility othersPassiveAbility in other._passiveAbilities)
        {
            AddPassiveAbility(othersPassiveAbility.OriginalModel);
        }
    }

    public void AddPassiveAbility(ATurretPassiveAbilityDataModel passiveAbilityModel)
    {
        ATurretPassiveAbility passiveAbility = passiveAbilityModel.MakePassiveAbility();
        if (HasReachedPassiveAmountLimit())
        {
            RemovePassiveAbility(_passiveAbilities[0].OriginalModel);
        }
        
        _passiveAbilities.Add(passiveAbility);
        passiveAbility.OnAddedToTurretCard(_cardDataOwner);
    }
    public void RemovePassiveAbility(ATurretPassiveAbilityDataModel passiveAbilityModel)
    {
        foreach (ATurretPassiveAbility passiveAbility in _passiveAbilities)
        {
            if (passiveAbility.OriginalModel == passiveAbilityModel)
            {
                _passiveAbilities.Remove(passiveAbility);
                passiveAbility.OnRemovedFromTurretCard();
                return;
            }
        }
    }
    public void OnTurretCardUpgraded(TurretCardData cardData)
    {
        foreach (ATurretPassiveAbility passiveAbility in _passiveAbilities)
        {
            passiveAbility.OnTurretCardUpgraded(cardData);
        }
    }
    

    public bool AlreadyContainsPassive(ATurretPassiveAbilityDataModel passiveAbilityDataModel)
    {
        foreach (ATurretPassiveAbility passiveAbility in _passiveAbilities)
        {
            if (passiveAbility.OriginalModel == passiveAbilityDataModel)
            {
                return true;
            }
        }

        return false;
    }

    public bool HasPassiveAbilities()
    {
        return _passiveAbilities.Count > 0;
    }
    public bool HasReachedPassiveAmountLimit()
    {
        return _passiveAbilities.Count == ATurretPassiveAbility.MAX_AMOUNT_FOR_TURRET;
    }
    


    public void OnTurretCreated(TurretBuilding turretOwner)
    {
        foreach (var passiveAbility in _passiveAbilities)
        {
            passiveAbility.OnTurretCreated(turretOwner);
        }
    }
    
    public void OnTurretDestroyed()
    {
        foreach (var passiveAbility in _passiveAbilities)
        {
            passiveAbility.OnTurretDestroyed();
        }
    }

    public void OnTurretPlacingStart()
    {
        foreach (var passiveAbility in _passiveAbilities)
        {
            passiveAbility.OnTurretPlacingStart();
        }
    }

    public void OnTurretPlacingFinish()
    {
        foreach (var passiveAbility in _passiveAbilities)
        {
            passiveAbility.OnTurretPlacingFinish();
        }
    }

    public void OnTurretPlacingMove()
    {
        foreach (var passiveAbility in _passiveAbilities)
        {
            passiveAbility.OnTurretPlacingMove();
        }
    }

    public void OnTurretPlaced(TurretBuilding turretOwner)
    {
        foreach (var passiveAbility in _passiveAbilities)
        {
            passiveAbility.OnTurretPlaced(turretOwner);
        }
    }


    public void OnBeforeShootingEnemy(ATurretProjectileBehaviour projectile)
    {
        foreach (var passiveAbility in _passiveAbilities)
        {
            passiveAbility.OnBeforeShootingEnemy(projectile);
        }  
    }
    
    public void OnBeforeDamagingEnemy(TurretDamageAttack damageAttack)
    {
        foreach (var passiveAbility in _passiveAbilities)
        {
            passiveAbility.OnBeforeDamagingEnemy(damageAttack);
        }
    }

    public void OnAfterDamagingEnemy(TurretDamageAttackResult damageAttackResult)
    {
        foreach (var passiveAbility in _passiveAbilities)
        {
            passiveAbility.OnAfterDamagingEnemy(damageAttackResult);
        }
    } 
}