using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Project.Scripts.Upgrades.CopyAbility
{
    public class CopyAbilityManagerCardHandAnimator : MonoBehaviour
    {
        private BuildingCard[] _cards;
        private Vector3[] _initShowGoalPositions;
        
        public void InitShowCards(BuildingCard[] cards)
        {
            _cards = cards;
            _initShowGoalPositions = new Vector3[cards.Length];
            Vector3 startOffset = new Vector3(0, -3, -1);

            for (int i = 0; i < cards.Length; ++i)
            {
                BuildingCard card = cards[i];
                card.DisableMouseInteraction();
                Vector3 cardStartPosition = card.CardTransform.position;
                _initShowGoalPositions[i] = cardStartPosition;

                card.CardTransform.position = cardStartPosition + startOffset;
                card.canBeHovered = false;
            }
        }
        
        public IEnumerator PlayInitShowCards()
        {
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < _cards.Length; ++i)
            {
                BuildingCard card = _cards[i];
                card.CardTransform.DOMove(_initShowGoalPositions[i], 0.3f)
                    .SetEase(Ease.OutSine);
                
                yield return new WaitForSeconds(0.1f);
            }

            foreach (var card in _cards)
            {
                card.canBeHovered = true;
                card.ReenableMouseInteraction();
            }
        }
        
        
        
        
        public IEnumerator PlayFinishHideCards(BuildingCard[] allCards, BuildingCard[] placedCards)
        {
            foreach (var card in placedCards)
            {
                card.StandardState(true, duration: 2.0f);
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForSeconds(0.2f);
        
            
            
            for (int i = 0; i < allCards.Length; ++i)
            {
                BuildingCard card = allCards[i];
                Vector3 goalPosition = card.CardTransform.position + new Vector3(0, -3, -1);
                
                card.CardTransform.DOMove(goalPosition, 0.3f)
                    .SetEase(Ease.OutSine);
                
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
}