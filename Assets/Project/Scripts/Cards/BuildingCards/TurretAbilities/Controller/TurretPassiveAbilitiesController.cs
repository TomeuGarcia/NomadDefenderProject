using System.Collections.Generic;

public class TurretPassiveAbilitiesController : ITurretPassiveAbilitiesNotifier
{
    private readonly List<ATurretPassiveAbility> _passiveAbilities;
    private readonly ITurretAbilityFactory _abilityFactory;
    private readonly TurretCardData _cardDataOwner;


    public TurretPassiveAbilitiesController(TurretCardData cardDataOwner,
        ITurretAbilityFactory abilityFactory)
    {
        _passiveAbilities = new List<ATurretPassiveAbility>(3);
        _abilityFactory = abilityFactory;
        _cardDataOwner = cardDataOwner;
    }
    
    public TurretPassiveAbilitiesController(TurretCardData cardDataOwner,
        TurretPassiveAbilitiesController other, ITurretAbilityFactory abilityFactory)
    {
        _passiveAbilities = new List<ATurretPassiveAbility>(other._passiveAbilities.Count);
        _abilityFactory = abilityFactory;
        _cardDataOwner = cardDataOwner;

        foreach (ATurretPassiveAbility passiveAbility in _passiveAbilities)
        {
            AddPassiveAbility(passiveAbility.OriginalModel);
        }
    }

    public void AddPassiveAbility(TurretPassiveAbilityDataModel passiveAbilityModel)
    {
        ATurretPassiveAbility passiveAbility = _abilityFactory.MakePassiveAbilityFromType(passiveAbilityModel);
        
        _passiveAbilities.Add(passiveAbility);
        passiveAbility.OnAddedToTurretCard(_cardDataOwner);
    }
    public void RemovePassiveAbility(TurretPassiveAbilityDataModel passiveAbilityModel)
    {
        foreach (ATurretPassiveAbility passiveAbility in _passiveAbilities)
        {
            if (passiveAbility.OriginalModel.AbilityType == passiveAbilityModel.AbilityType)
            {
                _passiveAbilities.Remove(passiveAbility);
                passiveAbility.OnRemovedFromTurretCard();
                return;
            }
        }
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