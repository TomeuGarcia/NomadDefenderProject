using System;

namespace Project.Scripts.Upgrades.CopyAbility
{
    public class CopyFromCardAllowPlaceCondition : CardPlaceSpot.IAllowPlaceCondition
    {
        public Action OnTriedPlacingInvalidCard;
        
        
        public bool CardIsAllowed(BuildingCard card)
        {
            TurretBuildingCard copyFromCard = card as TurretBuildingCard;
            
            bool cardIsAllowed =
                copyFromCard.CardData.PassiveAbilitiesController.CurrentNumberOfPassives > 0;

            if (!cardIsAllowed)
            {
                OnTriedPlacingInvalidCard?.Invoke();
            }
            

            return cardIsAllowed;
        }
    }
}