using System.Collections.Generic;

namespace Project.Scripts.Upgrades.CopyAbility
{
    public static class UpgradeRoomCardSortingUtilities
    {
        public static void PutCardsWithAbilitiesInFront(List<BuildingCard> cards)
        {
            for (int i = 1; i < cards.Count; ++i)
            {
                TurretBuildingCard turretCard = cards[i] as TurretBuildingCard;
                if (turretCard == null)
                {
                    continue;
                }

                if (turretCard.CardData.PassiveAbilitiesController.CurrentNumberOfPassives > 0)
                {
                    cards.RemoveAt(i);
                    cards.Insert(0, turretCard);
                }
            }
        }
        
        
    }
}