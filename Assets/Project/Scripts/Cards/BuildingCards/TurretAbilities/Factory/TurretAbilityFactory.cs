
public class TurretAbilityFactory : ITurretAbilityFactory
{
    public ATurretPassiveAbility MakePassiveAbilityFromType(TurretPassiveAbilityDataModel abilityDataModel)
    {
        switch (abilityDataModel.AbilityType)
        {
            case TurretPassiveAbilityType      .None:
                return new TurretPassiveAbility_Test(abilityDataModel);
            
            case TurretPassiveAbilityType      .MostDistantEnemySorting:
                return new TurretPassiveAbility_MostDistantEnemySorting(abilityDataModel);
            
            case TurretPassiveAbilityType      .HealLocation:
                return new TurretPassiveAbility_HealLocation(abilityDataModel);
            
            case TurretPassiveAbilityType      .DrawCard:
                return new TurretPassiveAbility_DrawCard(abilityDataModel);
            case TurretPassiveAbilityType      .DrawTurretCardReplacingProjectile:
                return new TurretPassiveAbility_DrawTurretCardReplacingProjectile(abilityDataModel);
            case TurretPassiveAbilityType      .SpawnCardCopyInDeck:
                return new TurretPassiveAbility_SpawnCardCopyInDeck(abilityDataModel);
            case TurretPassiveAbilityType      .WaveFinishSpawnCardCopyInHand:
                return new TurretPassiveAbility_WaveFinishSpawnCardCopyInHand(abilityDataModel);
            case TurretPassiveAbilityType      .SelfHurtUpgradeStats:
                return new TurretPassiveAbility_SelfHurtUpgradeStats(abilityDataModel);
        }

        
        return null;
    }

    
}