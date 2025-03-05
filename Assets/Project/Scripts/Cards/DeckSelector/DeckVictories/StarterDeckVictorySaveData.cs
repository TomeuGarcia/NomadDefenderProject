
using System;
using UnityEngine;


[System.Serializable]
public class StarterDecksSaveStatus
{
    [SerializeField] public StarterDeckSaveData[] starterDecksCollectionSaveData;

    public StarterDecksSaveStatus(CardDeckAsset[] possibleStarterDecks)
    {
        starterDecksCollectionSaveData = new StarterDeckSaveData[possibleStarterDecks.Length];
        for (int i = 0; i < possibleStarterDecks.Length; ++i)
        {
            CardDeckAsset starterDeck = possibleStarterDecks[i];
            starterDecksCollectionSaveData[i] = new StarterDeckSaveData(starterDeck.DeckName);
        }
    }

    public StarterDeckSaveData GetDeckSaveDataByName(CardDeckAsset starterDeck)
    {
        foreach (StarterDeckSaveData starterDeckSaveData in starterDecksCollectionSaveData)
        {
            if (starterDeckSaveData.deckName == starterDeck.DeckName)
            {
                return starterDeckSaveData;
            }
        }

        return null;
    }

    public void IncrementDeckVictory(CardDeckAsset starterDeck, GameDifficultyType gameDifficulty)
    {
        GetDeckSaveDataByName(starterDeck).IncrementWin(gameDifficulty);
    }
    
    public void ResetAll()
    {
        foreach (StarterDeckSaveData starterDeckSaveData in starterDecksCollectionSaveData)
        {
            starterDeckSaveData.Reset();
        }
    }

    public void GetDeckVictoriesAndHighestDifficulty(CardDeckAsset starterDeck, 
        out int winCount, out GameDifficultyType highestDifficultyWin)
    {
        StarterDeckSaveData starterDeckSaveData = GetDeckSaveDataByName(starterDeck);
        winCount = starterDeckSaveData.winCount;
        highestDifficultyWin = starterDeckSaveData.highestDifficultyWin;
    }

    public void ValidateCorrectLoading()
    {
        foreach (StarterDeckSaveData starterDeckSaveData in starterDecksCollectionSaveData)
        {
            starterDeckSaveData.ValidateCorrectLoading();
        }
    }
}



[System.Serializable]
public class StarterDeckSaveData
{
    [SerializeField] public string deckName;
    [SerializeField] public int winCount;
    [SerializeField] public GameDifficultyType highestDifficultyWin;

    [SerializeField] public DeckHighscore[] highscoreByDifficultyIndex;

    public StarterDeckSaveData(string deckName)
    {
        this.deckName = deckName;
        Reset();
    }

    public void Reset()
    {
        winCount = 0;
        highestDifficultyWin = GameDifficultyType.Easy;

        int difficultyTypesCount = Enum.GetValues(typeof(GameDifficultyType)).Length;

        highscoreByDifficultyIndex = new DeckHighscore[difficultyTypesCount];
        for (int i = 0; i < difficultyTypesCount; ++i)
        {
            highscoreByDifficultyIndex[i] = new DeckHighscore();
        }
    }

    public void IncrementWin(GameDifficultyType gameDifficulty)
    {
        ++winCount;
        if (highestDifficultyWin < gameDifficulty)
        {
            highestDifficultyWin = gameDifficulty;
        }
    }

    public void UpdateHighscore(GameDifficultyType gameDifficulty, DeckHighscore newScore)
    {
        GetHighscoreByDifficulty(gameDifficulty).Update(newScore);
    }

    public DeckHighscore GetHighscoreByDifficulty(GameDifficultyType gameDifficulty)
    {
        return highscoreByDifficultyIndex[(int)gameDifficulty];
    }

    public void ValidateCorrectLoading()
    {
        if (highscoreByDifficultyIndex == null || highscoreByDifficultyIndex.Length < 1)
        {
            Reset();
            return;
        }
        
        int difficultyTypesCount = Enum.GetValues(typeof(GameDifficultyType)).Length;
        DeckHighscore[] fixedHighscoreByDifficultyIndex = new DeckHighscore[difficultyTypesCount];

        for (int i = 0; i < highscoreByDifficultyIndex.Length; ++i)
        {
            fixedHighscoreByDifficultyIndex[i] = highscoreByDifficultyIndex[i];
        }
        for (int i = highscoreByDifficultyIndex.Length; i < difficultyTypesCount; ++i)
        {
            fixedHighscoreByDifficultyIndex[i] = new DeckHighscore();
        }

        highscoreByDifficultyIndex = fixedHighscoreByDifficultyIndex;
    }
}

[System.Serializable]
public class DeckHighscore
{
    [SerializeField] public int totalDamageDealt;
    [SerializeField] public int highestDamageDealt;
    [SerializeField] public int perfectDefenseCount;
    [SerializeField] public int lostHealth;

    public DeckHighscore()
    {
        totalDamageDealt = 0;
        highestDamageDealt = 0;
        perfectDefenseCount = 0;
        lostHealth = 0;
    }
    
    public DeckHighscore(int totalDamageDealt, int highestDamageDealt, int perfectDefenseCount, int lostHealth)
    {
        this.totalDamageDealt = totalDamageDealt;
        this.highestDamageDealt = highestDamageDealt;
        this.perfectDefenseCount = perfectDefenseCount;
        this.lostHealth = lostHealth;
    }

    public void Update(DeckHighscore newScore)
    {
        totalDamageDealt    = Mathf.Max(totalDamageDealt, newScore.totalDamageDealt);
        highestDamageDealt  = Mathf.Max(highestDamageDealt, newScore.highestDamageDealt);
        perfectDefenseCount = Mathf.Max(perfectDefenseCount, newScore.perfectDefenseCount);
        lostHealth          = Mathf.Max(lostHealth, newScore.lostHealth);
    }
}