using System.Collections.Generic;

public class TurretPassiveAbilitiesController : ITurretPassiveAbilitiesNotifier
{
    private readonly List<ATurretPassiveAbility> _passiveAbilities;
    private readonly TurretCardData _cardDataOwner;

    public List<ATurretPassiveAbility> PassiveAbilities => _passiveAbilities;

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
        foreach (ATurretPassiveAbility passiveAbility in _passiveAbilities)
        {
            AddPassiveAbility(passiveAbility.OriginalModel);
        }
    }

    public void AddPassiveAbility(ATurretPassiveAbilityDataModel passiveAbilityModel)
    {
        ATurretPassiveAbility passiveAbility = passiveAbilityModel.MakePassiveAbility();
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