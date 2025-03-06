using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameProgressionStatus
{
    public class GameStatus
    {
        public int VictoriesCount { get; private set; }
        public bool BeatARunWithFullPerfectDefense { get; private set; }
        public bool UnlocksHardDifficulty => VictoriesCount == 1;
        public bool UnlocksStarterDeck => VictoriesCount is > 0 and <= 2;
        
        public StarterDecksSaveStatus StarterDecksSaveStatus { get; private set; }

        public GameStatus(CardDeckAsset[] possibleStarterDecks)
        {
            VictoriesCount = 0;
            BeatARunWithFullPerfectDefense = false;
            StarterDecksSaveStatus = new StarterDecksSaveStatus(possibleStarterDecks);
        }
        public GameStatus(int victoriesCount, bool beatARunWithFullPerfectDefense, StarterDecksSaveStatus starterDecksSaveStatus)
        {
            VictoriesCount = victoriesCount;
            BeatARunWithFullPerfectDefense = beatARunWithFullPerfectDefense;
            StarterDecksSaveStatus = starterDecksSaveStatus;
        }

        public void ValidateCorrectLoading(CardDeckAsset[] possibleStarterDecks)
        {
            if (StarterDecksSaveStatus == null ||
                StarterDecksSaveStatus.starterDecksCollectionSaveData == null ||
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

        public void SetBeatARunWithFullPerfectDefense()
        {
            BeatARunWithFullPerfectDefense = true;
        }
    }
    
    
    public GameStatus Game { get; }
    
    
}
