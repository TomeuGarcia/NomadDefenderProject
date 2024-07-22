using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DeckLibrary", menuName = "Cards/DeckLibrary")]
public class DecksLibrary : ScriptableObject
{
    [Header("STARTER DECK DATA")]
    [SerializeField] protected DeckData starterDeckData;

    [Header("GAME DECK DATA")]
    [SerializeField] protected DeckData gameDeckData;


    public void SetStarterDeck(DeckData newStarterDeckData)
    {
        starterDeckData = newStarterDeckData;
    }
    
    public void InitGameDeck()
    {
        gameDeckData.ReplaceFor(starterDeckData);
    }

}