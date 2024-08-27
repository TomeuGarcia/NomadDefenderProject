using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DeckLibrary_NAME", 
    menuName = SOAssetPaths.CARDS_LIBRARIES + "DeckLibrary")]
public class DecksLibrary : ScriptableObject
{
    [Header("STARTER DECK DATA")]
    [SerializeField] protected CardDeckAsset startingDeck;

    [Header("GAME DECK DATA")]
    [SerializeField] protected CardDeckInUseData gameDeckData;


    public void SetStarterDeck(CardDeckAsset newStartingDeck)
    {
        startingDeck = newStartingDeck;
    }
    
    public void InitGameDeck()
    {
        gameDeckData.InitializeForRun(startingDeck, ServiceLocator.GetInstance().CardSpawnService);
    }

}