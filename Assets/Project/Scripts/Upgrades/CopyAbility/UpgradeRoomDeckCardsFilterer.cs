using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class UpgradeRoomDeckCardsFilterer
{
    public static List<BuildingCard> GetRandomTurretCards(BuildingCard[] deckCards, int numberOfCards, Transform cardsHolder)
    {
        // Separate MAXed cards from NON-MAXed cards
        List<BuildingCard> maxLevelCards = new List<BuildingCard>();
        List<BuildingCard> notMaxLevelCards = new List<BuildingCard>();
        
        for (int cardI = 0; cardI < deckCards.Length; ++cardI)
        {
            if (deckCards[cardI].cardBuildingType == BuildingCard.CardBuildingType.TURRET)
            {
                
                if (deckCards[cardI].GetCardLevel() < 3)
                {
                    notMaxLevelCards.Add(deckCards[cardI]);
                }
                else
                {
                    maxLevelCards.Add(deckCards[cardI]);
                }
            }            
        }

        BuildingCard[] chosenCards = new BuildingCard[numberOfCards];
        int chosenCardI = 0;

        const int maximumCardsOfMaxLevel = 1;
        int numMaxedCardsToAdd = Mathf.Min(maximumCardsOfMaxLevel, maxLevelCards.Count);
        
        int undesiredRemainingMaxCards = numberOfCards - notMaxLevelCards.Count - numMaxedCardsToAdd;
        if (undesiredRemainingMaxCards > 0)
        {
            numMaxedCardsToAdd += undesiredRemainingMaxCards;
        }
        

        // If not enough NON-MAXed cards, add MAXed cards
        if (numMaxedCardsToAdd > 0)
        {            
            HashSet<int> randomMaxedCardsIndices = new HashSet<int>();

            while (randomMaxedCardsIndices.Count < numMaxedCardsToAdd && maxLevelCards.Count > 0)
            {
                int randomIndex = Random.Range(0, maxLevelCards.Count);
                randomMaxedCardsIndices.Add(randomIndex);
            }
            foreach (int index in randomMaxedCardsIndices)
            {
                chosenCards[chosenCardI] = maxLevelCards[index]; 
                ++chosenCardI;
            }
        }

        // Add NON-MAXed cards
        int numRemainingCards = numberOfCards - numMaxedCardsToAdd;
        HashSet<int> randomNotMaxedCardsIndices = new HashSet<int>();
        while (randomNotMaxedCardsIndices.Count < numRemainingCards && notMaxLevelCards.Count > 0)
        {
            int randomIndex = Random.Range(0, notMaxLevelCards.Count);
            randomNotMaxedCardsIndices.Add(randomIndex);
        }
        foreach (int index in randomNotMaxedCardsIndices)
        {
            chosenCards[chosenCardI] = notMaxLevelCards[index];
            ++chosenCardI;
        }

        // Set parent
        for (int cardI = 0; cardI < numberOfCards; ++cardI)
        {
            chosenCards[cardI].transform.SetParent(cardsHolder, false);
        }

        return chosenCards.ToList();
    }
    
    
    
    
    
    
    
    public static List<BuildingCard> GetRandomTurretCardsWithAtLeast1Ability(BuildingCard[] deckCards, int numberOfCards, Transform cardsHolder)
    {
        // Separate MAXed cards from NON-MAXed cards
        List<BuildingCard> maxLevelCards = new List<BuildingCard>();
        List<BuildingCard> notMaxLevelCards = new List<BuildingCard>();
        TurretBuildingCard firstCardWithAbility = null;

        for (int cardI = 0; cardI < deckCards.Length; ++cardI)
        {
            if (deckCards[cardI].cardBuildingType == BuildingCard.CardBuildingType.TURRET)
            {
                
                if (deckCards[cardI].GetCardLevel() < TurretCardDataModel.MAX_CARD_LEVEL)
                {
                    notMaxLevelCards.Add(deckCards[cardI]);
                }
                else
                {
                    maxLevelCards.Add(deckCards[cardI]);
                }
                
                TurretBuildingCard cardWithAbility = deckCards[cardI] as TurretBuildingCard;
                if (firstCardWithAbility == null &&
                    cardWithAbility.CardData.PassiveAbilitiesController.CurrentNumberOfPassives > 0)
                {
                    firstCardWithAbility = cardWithAbility;
                }
            }            
        }

        BuildingCard[] chosenCards = new BuildingCard[numberOfCards];
        int chosenCardI = 0;




        const int maximumCardsOfMaxLevel = 1;
        int numMaxedCardsToAdd = Mathf.Min(maximumCardsOfMaxLevel, maxLevelCards.Count);

        int undesiredRemainingMaxCards = numberOfCards - notMaxLevelCards.Count - numMaxedCardsToAdd;
        if (undesiredRemainingMaxCards > 0)
        {
            numMaxedCardsToAdd += undesiredRemainingMaxCards;
        }
        

        // If not enough NON-MAXed cards, add MAXed cards
        if (numMaxedCardsToAdd > 0)
        {            
            HashSet<int> randomMaxedCardsIndices = new HashSet<int>();

            while (randomMaxedCardsIndices.Count < numMaxedCardsToAdd && maxLevelCards.Count > 0)
            {
                int randomIndex = Random.Range(0, maxLevelCards.Count);
                randomMaxedCardsIndices.Add(randomIndex);
            }
            foreach (int index in randomMaxedCardsIndices)
            {
                chosenCards[chosenCardI] = maxLevelCards[index]; 
                ++chosenCardI;
            }
        }

        // Add NON-MAXed cards
        int numRemainingCards = numberOfCards - numMaxedCardsToAdd;
        HashSet<int> randomNotMaxedCardsIndices = new HashSet<int>();
        while (randomNotMaxedCardsIndices.Count < numRemainingCards && notMaxLevelCards.Count > 0)
        {
            int randomIndex = Random.Range(0, notMaxLevelCards.Count);
            randomNotMaxedCardsIndices.Add(randomIndex);
        }
        foreach (int index in randomNotMaxedCardsIndices)
        {
            chosenCards[chosenCardI] = notMaxLevelCards[index];
            ++chosenCardI;
        }



        bool noCardWithAbility = true;
        foreach (BuildingCard chosenCard in chosenCards)
        {
            TurretBuildingCard chosenTurretCard = chosenCard as TurretBuildingCard;
            if (chosenTurretCard.CardData.PassiveAbilitiesController.CurrentNumberOfPassives > 0)
            {
                noCardWithAbility = false;
                break;
            }
        }

        if (noCardWithAbility)
        {
            if (firstCardWithAbility == null)
            {
                Debug.Log("XD quit");
                Application.Quit();
                return null;
            }

            chosenCards[0] = firstCardWithAbility;
        }

        

        // Set parent
        for (int cardI = 0; cardI < numberOfCards; ++cardI)
        {
            chosenCards[cardI].transform.SetParent(cardsHolder, false);
        }
        return chosenCards.ToList();
    }
    
    


}