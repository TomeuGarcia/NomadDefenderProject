using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SelectableDeck;

public class SelectableDeck : MonoBehaviour
{
    [SerializeField] private Transform cardsHolder;
    [SerializeField] private Collider interactionCollider;
    [SerializeField] private DeckData deckData;
    private BuildingCard[] cards;

    private DeckSelector deckSelector;

    public BuildingCard[] Cards => cards;
    public Transform CardsHolder => cardsHolder;



    [System.Serializable]
    public class ArrangeCardsData
    {
        [SerializeField] private Vector3 displacement;
        [SerializeField] private Vector3 maxRotationAngle;

        public Vector3 DisplacementBetweenCards => displacement;
        public Vector3 RandomRotationAngle => new Vector3(Random.Range(-maxRotationAngle.x, maxRotationAngle.x), 
                                                          Random.Range(-maxRotationAngle.y, maxRotationAngle.y), 
                                                          Random.Range(-maxRotationAngle.z, maxRotationAngle.z));
    }


    private void OnMouseDown()
    {
        deckSelector.OnDeckSelected(this);
    }


    public void InitReferences(DeckSelector deckSelector)
    {
        this.deckSelector = deckSelector;
    }

    public void InitSpawnCards(DeckCreator deckCreator)
    {
        deckCreator.SpawnCardsAndResetDeckData(deckData, out cards);
    }

    public void InitArrangeCards(ArrangeCardsData arrangeCardsData)
    {
        for (int i = 0; i < cards.Length; ++i)
        {
            BuildingCard card = cards[i];

            card.DisableMouseInteraction();

            card.RootCardTransform.SetParent(cardsHolder.transform);

            card.RootCardTransform.localRotation = Quaternion.Euler(arrangeCardsData.RandomRotationAngle + Vector3.right * 90f);
            card.RootCardTransform.localPosition = i * arrangeCardsData.DisplacementBetweenCards;
        }
    }


    public void SetEnabledInteraction(bool isEnabled)
    {
        interactionCollider.enabled = isEnabled;
    }


    public IEnumerator ArrangeCardsFromFirst(float motionDuration, float delayBetweenCards, ArrangeCardsData arrangeCardsData, Transform newParent)
    {
        for (int i = 0; i < cards.Length; ++i)
        {
            ArrangeCard(cards[i], i, motionDuration, arrangeCardsData, newParent);

            yield return new WaitForSeconds(delayBetweenCards);
        }
    }
    
    public IEnumerator ArrangeCardsFromLast(float motionDuration, float delayBetweenCards, ArrangeCardsData arrangeCardsData, Transform newParent)
    {
        for (int i = cards.Length - 1; i >= 0; --i)
        {           
            ArrangeCard(cards[i], cards.Length-1 - i, motionDuration, arrangeCardsData, newParent);

            yield return new WaitForSeconds(delayBetweenCards);
        }
    }

    private void ArrangeCard(BuildingCard card, int i, float motionDuration, ArrangeCardsData arrangeCardsData, Transform newParent)
    {
        card.RootCardTransform.SetParent(newParent);

        card.RootCardTransform.DOLocalRotateQuaternion(Quaternion.Euler(arrangeCardsData.RandomRotationAngle + Vector3.right * 90f), motionDuration);
        card.RootCardTransform.DOLocalMove(i * arrangeCardsData.DisplacementBetweenCards, motionDuration);
    }

    public void EnableCardsMouseInteraction()
    {
        foreach (BuildingCard card in cards)
        {
            card.ReenableMouseInteraction();
        }
    }

    public void DisableCardsMouseInteraction()
    {
        foreach (BuildingCard card in cards)
        {
            card.DisableMouseInteraction();
        }
    }




}
