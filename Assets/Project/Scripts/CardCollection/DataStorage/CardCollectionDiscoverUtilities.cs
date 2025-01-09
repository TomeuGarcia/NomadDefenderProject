using System.Collections.Generic;

namespace Project.Scripts.CardCollection.DataStorage
{
    public static class CardCollectionDiscoverUtilities
    {
        public static void DiscoverTurretCardProjectilesAndAbilities(
            CardCollectionDataStorage cardCollectionDataStorage, TurretBuildingCard turretCard)
        {
            TurretPartProjectileDataModel projectile = turretCard.CardParts.Projectile;
            if (!cardCollectionDataStorage.WasDiscovered(projectile))
            {
                cardCollectionDataStorage.Discover(projectile);
            }

            List<ATurretPassiveAbility> passiveAbilities = turretCard.CardData.PassiveAbilitiesController.PassiveAbilities;
            foreach (var passiveAbility in passiveAbilities)
            {
                if (!cardCollectionDataStorage.WasDiscovered(passiveAbility.OriginalModel))
                {
                    cardCollectionDataStorage.Discover(passiveAbility.OriginalModel);
                }
            }
        }


        public static void DiscoverCardDeckProjectilesAndAbilities(
            CardCollectionDataStorage cardCollectionDataStorage, CardDeckAsset cardDeckAsset)
        {
            TurretCardDataModel[] turretCards = cardDeckAsset.TurretCards;

            foreach (TurretCardDataModel turretCard in turretCards)
            {
                TurretPartProjectileDataModel projectile = turretCard.SharedPartsGroup.Projectile;
                if (!cardCollectionDataStorage.WasDiscovered(projectile))
                {
                    cardCollectionDataStorage.Discover(projectile);
                }

                ATurretPassiveAbilityDataModel[] passiveAbilities = turretCard.PassiveAbilityModels;
                foreach (var passiveAbility in passiveAbilities)
                {
                    if (!cardCollectionDataStorage.WasDiscovered(passiveAbility))
                    {
                        cardCollectionDataStorage.Discover(passiveAbility);
                    }
                }
            }
        }
        
    }
}