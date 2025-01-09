using UnityEngine;

namespace Project.Scripts.CardCollection
{
    public class CardCollectionCardPartsGroup
    {
        private readonly CardPart[] _cards;

        public CardCollectionCardPartsGroup(CardPart[] cards)
        {
            _cards = cards;
        }


        public void StartCardsInteraction()
        {
            foreach (CardPart card in _cards)
            {
                card.InitPositions(card.transform.position);
                
                card.OnCardHovered += SetHoveredCard;
                card.ReenableMouseInteraction();
                card.canDisplayInfoIfNotInteractable = false;
                card.isInteractable = true;
                card.hideInfoWhenSelected = false;
            }
        }

        
            
        private void SetHoveredCard(CardPart card)
        {
            card.HoveredState();

            foreach (CardPart itCard in _cards)
            {
                itCard.OnCardHovered -= SetHoveredCard;
                itCard.OnCardUnhovered += SetStandardCard;
            }
            
            GameAudioManager.GetInstance().PlayCardHovered();
        }

        private void SetStandardCard(CardPart card)
        {
            card.StandardState(false);
            
            foreach (CardPart itCard in _cards)
            {
                itCard.OnCardHovered += SetHoveredCard;
                itCard.OnCardUnhovered -= SetStandardCard;
            }
            
            //GameAudioManager.GetInstance().PlayCardHoverExit();
        }
        
    }
}