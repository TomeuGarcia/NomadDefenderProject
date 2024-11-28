using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Project.Scripts.Upgrades.CopyAbility
{
    public class CopyAbilityManagerCardHandAnimator : MonoBehaviour
    {
        
        
        
        public IEnumerator PlayInitShowCards(BuildingCard[] cards)
        {
            Vector3[] goalPositions = new Vector3[cards.Length];
            Vector3 startOffset = new Vector3(0, -3, -1);

            for (int i = 0; i < cards.Length; ++i)
            {
                BuildingCard card = cards[i];
                card.DisableMouseInteraction();
                Vector3 cardStartPosition = card.CardTransform.position;
                goalPositions[i] = cardStartPosition;

                card.CardTransform.position = cardStartPosition + startOffset;
            }

            
            
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < cards.Length; ++i)
            {
                BuildingCard card = cards[i];
                card.CardTransform.DOMove(goalPositions[i], 0.3f)
                    .SetEase(Ease.OutSine);
                
                yield return new WaitForSeconds(0.1f);
            }

            foreach (var card in cards)
            {
                card.EnableMouseInteraction();
            }
        }
        
        public IEnumerator PlayFinishHideCards(BuildingCard[] allCards, BuildingCard[] placedCards)
        {
            foreach (var card in placedCards)
            {
                card.StandardState(true, duration: 0.2f);
            }
            yield return new WaitForSeconds(0.4f);
        
            
            
            for (int i = 0; i < allCards.Length; ++i)
            {
                BuildingCard card = allCards[i];
                Vector3 goalPosition = card.CardTransform.position + new Vector3(0, -3, -1);
                
                card.CardTransform.DOMove(goalPosition, 0.3f)
                    .SetEase(Ease.OutSine);
                
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}