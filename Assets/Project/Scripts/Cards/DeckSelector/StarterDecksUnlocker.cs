using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarterDecksUnlocker : MonoBehaviour
{
    private static StarterDecksUnlocker instance;
    private const string UNLOCKED_STARTER_DECKS_COUNT_KEY = "UnlockedStartDecks";
    private const int START_NUMBER_DECKS_UNLOCKED = 2;
    private const int TOTAL_NUMBER_STARTER_DECKS = 4;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    public static StarterDecksUnlocker GetInstance()
    {
        return instance;
    }

    public void ResetUnlockedCount()
    {
        PlayerPrefs.SetInt(UNLOCKED_STARTER_DECKS_COUNT_KEY, START_NUMBER_DECKS_UNLOCKED);
    }

    public void UnlockNextDeck()
    {
        int currentNumeberOfUnlockedDecks = GetNumberofUnlockedDecks();
        currentNumeberOfUnlockedDecks  = Mathf.Min(currentNumeberOfUnlockedDecks + 1, TOTAL_NUMBER_STARTER_DECKS);

        PlayerPrefs.SetInt(UNLOCKED_STARTER_DECKS_COUNT_KEY, currentNumeberOfUnlockedDecks);
    }
    public void UnlockRemainengDecks()
    {
        PlayerPrefs.SetInt(UNLOCKED_STARTER_DECKS_COUNT_KEY, TOTAL_NUMBER_STARTER_DECKS);
    }

    public int GetNumberofUnlockedDecks()
    {
        if (!PlayerPrefs.HasKey(UNLOCKED_STARTER_DECKS_COUNT_KEY))
        {
            ResetUnlockedCount();
        }

        int currentNumeberOfUnlockedDecks = PlayerPrefs.GetInt(UNLOCKED_STARTER_DECKS_COUNT_KEY);
        return currentNumeberOfUnlockedDecks;
    }

}
