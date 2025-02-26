using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameProgressionStatus
{
    public class GameStatus
    {
        public int VictoriesCount { get; }
        public bool UnlocksHardDifficulty => VictoriesCount == 1;
        public bool UnlocksStarterDeck => VictoriesCount is > 0 and <= 2;

        public GameStatus(int victoriesCount)
        {
            VictoriesCount = victoriesCount;
        }
    }
    
    
    
    public GameStatus Game { get; }
    
}
