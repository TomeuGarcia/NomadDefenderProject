using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("DECK DATA")]
    [SerializeField] DeckData originalStartDeck;
    [SerializeField] DeckData gameStartDeck;


    private void Awake()
    {
        gameStartDeck.ReplaceFor(originalStartDeck);
    }


}
