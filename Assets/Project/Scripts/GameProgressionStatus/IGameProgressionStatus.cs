using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameProgressionStatus
{
    public class GameStatus
    {
        public int VictoriesCount { get; private set; }
        public bool UnlocksHardDifficulty => VictoriesCount == 1;
        public bool UnlocksStarterDeck => VictoriesCount is > 0 and <= 2;
        
        public StarterDecksSaveStatus StarterDecksSaveStatus { get; private set; }

        public GameStatus(CardDeckAsset[] possibleStarterDecks)
        {
            VictoriesCount = 0;
            StarterDecksSaveStatus = new StarterDecksSaveStatus(possibleStarterDecks);
        }
        public GameStatus(int victoriesCount, StarterDecksSaveStatus starterDecksSaveStatus)
        {
            VictoriesCount = victoriesCount;
            StarterDecksSaveStatus = starterDecksSaveStatus;
        }

        public void ValidateCorrectLoading(CardDeckAsset[] possibleStarterDecks)
        {
            if (StarterDecksSaveStatus == null ||
                StarterDecksSaveStatus.starterDecksCollectionSaveData.Length < possibleStarterDecks.Length)
            {
                StarterDecksSaveStatus = new StarterDecksSaveStatus(possibleStarterDecks);
            }
            
            StarterDecksSaveStatus.ValidateCorrectLoading();
        }

        public void IncrementVictoriesCount(CardDeckAsset starterDeck, GameDifficultyType gameDifficulty)
        {
            ++VictoriesCount;
            StarterDecksSaveStatus.IncrementDeckVictory(starterDeck, gameDifficulty);
        }
    }
    
    
    public GameStatus Game { get; }
    
    
}
