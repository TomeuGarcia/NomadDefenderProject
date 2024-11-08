using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;


[CreateAssetMenu(fileName = "DeckLibrary_NAME", 
    menuName = SOAssetPaths.CARDS_LIBRARIES + "DeckLibrary")]
public class DecksLibrary : ScriptableObject
{
    [Header("STARTER DECK DATA")]
    [SerializeField] protected CardDeckAsset startingDeck;
    [SerializeField] private bool _manualTrophy;
    [ShowIf("_manualTrophy")] [SerializeField] private UnlockableTrophyModel _startingDeckVictoryTrophy;

    [Header("GAME DECK DATA")]
    [SerializeField] protected CardDeckInUseData gameDeckData;
    public CardDeckInUseData DeckInUse => gameDeckData;
    
    [Header("ALL DECKS")]
    [SerializeField] private CardDeckAsset _frostDeck;
    [SerializeField] private CardDeckAsset _repeaterDeck;
    [SerializeField] private CardDeckAsset _currencyDeck;
    [SerializeField] private CardDeckAsset _berserkerDeck;


    public void SetStarterDeck(CardDeckAsset newStartingDeck, UnlockableTrophyModel startingDeckVictoryTrophy)
    {
        startingDeck = newStartingDeck;
        _startingDeckVictoryTrophy = startingDeckVictoryTrophy;
    }
    
    public void InitGameDeck()
    {
        gameDeckData.InitializeForRun(startingDeck, ServiceLocator.GetInstance().CardSpawnService,
            _startingDeckVictoryTrophy);
    }


    private bool IsUsingDeck(CardDeckAsset deckToCompareTo)
    {
        if (deckToCompareTo == null)
        {
            throw new NullReferenceException("DeckToCompareTo is Null");
        }
        
        return startingDeck == deckToCompareTo;
    }
    
    public bool IsUsingFrostDeck()
    {
        return IsUsingDeck(_frostDeck);
    }
    public bool IsUsingRepeaterDeck()
    {
        return IsUsingDeck(_repeaterDeck);
    }
    public bool IsUsingCurrencyDeck()
    {
        return IsUsingDeck(_currencyDeck);
    }
    public bool IsUsingBerserkerDeck()
    {
        return IsUsingDeck(_berserkerDeck);
    }
}