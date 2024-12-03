using System;
using UnityEngine;

namespace Project.Scripts.Upgrades.CopyAbility
{
    public class CardPlaceSpot : MonoBehaviour
    {
        [SerializeField] private Transform cardDragBoundsTargetTransform;
        [SerializeField] private Vector3 _boundsSize = new Vector3(2f, 1f, 4f);
        
        private Bounds _cardDragBoundsTarget;
        
        public BuildingCard PlacedCard { get; private set; }
        public IAllowPlaceCondition AllowPlaceCondition { get; set; }

        public Vector3 PlacePosition => transform.position;


        public Action<BuildingCard> OnCardPlaced;
        public Action<BuildingCard> OnCardRemoved;
        
        public interface IAllowPlaceCondition
        {
            bool CardIsAllowed(BuildingCard card);
        }
        

        private void OnValidate()
        {
            UpdateDragBounds();
        }
        private void Awake()
        {
            UpdateDragBounds();
        }

        private void UpdateDragBounds()
        {
            _cardDragBoundsTarget = new Bounds(cardDragBoundsTargetTransform.position, _boundsSize);
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(_cardDragBoundsTarget.center, _cardDragBoundsTarget.size);
        }
        
        
        public bool CheckSnapCardAtSelectedPosition(BuildingCard card)
        {
            return !HasAnyPlacedCard() && _cardDragBoundsTarget.Contains(card.CardTransform.position);
        }

        public bool AllowsPlacingCard(BuildingCard card)
        {
            if (AllowPlaceCondition == null)
            {
                return true;
            }
            else
            {
                return AllowPlaceCondition.CardIsAllowed(card);
            }
        }

        public void SetPlacedCard(BuildingCard card)
        {
            PlacedCard = card;
            OnCardPlaced?.Invoke(card);
        }
        
        public void RemovePlacedCard()
        {
            BuildingCard tempPlacedCard = PlacedCard;
            PlacedCard = null;
            OnCardRemoved?.Invoke(tempPlacedCard);
        }

        public bool HasAnyPlacedCard()
        {
            return PlacedCard != null;
        }
        
        public bool HasPlacedCard(BuildingCard card)
        {
            return PlacedCard == card;
        }
        
    }
}