using System.Collections;
using DG.Tweening;
using Project.Scripts.Cards;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Scripts.CardCollection
{
    public class CardCollectionPositioner : MonoBehaviour
    {
        [Header("CONFIG")]
        [SerializeField] private Transform _cardsHolder;
        [SerializeField] private Transform _cardsStartPosition;
        [SerializeField, Min(1)] private int _cardsPerRow = 7;
        [SerializeField, Min(0)] private float _cardHeight = 1.5f;
        [SerializeField, Min(0)] private float _cardWidth = 1.0f;
        [SerializeField, Min(0)] private float _spacingBetweenCards = 0.25f;
        [SerializeField, Min(0)] private float _spacingBetweenRows = 0.25f;
        
        [Header("DEBUG")]
        [SerializeField, Min(1)] private int _debugCardsCount = 9;


        public void SetupCards(Transform[] cards)
        {
            foreach (Transform card in cards)
            {
                card.position = _cardsStartPosition.position;
                card.rotation = Quaternion.Euler(90f, 0f, 0f);
                card.SetParent(_cardsHolder);
            }
        }
        
        public IEnumerator PositionCards(Transform[] cards)
        {
            Vector3[] cardsEndPositions = ComputeCardsEndPositions(cards.Length); 
            
            float cardMoveDelay = 0.1f;
            float cardMoveDuration = 0.5f;
            float cardMoveSoundPitch = 1.2f;
        
            for (int i = 0; i < cards.Length; ++i)
            {
                yield return new WaitForSecondsRealtime(cardMoveDelay);

                GameAudioManager.GetInstance().PlayCardInfoMoveShown(cardMoveSoundPitch);
                
                cards[i].DOMove(cardsEndPositions[i], cardMoveDuration).SetEase(Ease.OutQuart);

                cardMoveDelay *= 0.98f;
                cardMoveDuration *= 0.98f;
                cardMoveSoundPitch *= 1.02f;
            }
            yield return new WaitForSecondsRealtime(0.3f);
        }
        
        private Vector3[] ComputeCardsEndPositions(int cardsCount)
        {
            return CardArrangingUtilities.GetCenteredCards(_cardsHolder, cardsCount, _cardsPerRow,
                _spacingBetweenRows, _spacingBetweenCards,
                _cardWidth, _cardHeight);
        }
        
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Vector3 cardSize = new Vector3(_cardWidth, 0.2f, _cardHeight);        
            Vector3[] cardPositions = ComputeCardsEndPositions(_debugCardsCount);

            for (int i = 0; i < cardPositions.Length; ++i)
            {
                Vector3 position = cardPositions[i];

                Gizmos.DrawSphere(position, 0.1f);
                Gizmos.DrawCube(position, cardSize);
            }
        }

    }
}