using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameProgressionStatus
{
    public class GameStatus
    {
        public readonly int VictoriesCount;

        public GameStatus(int victoriesCount)
        {
            VictoriesCount = victoriesCount;
        }
    }
    
    
    
    public GameStatus Game { get; }
    
}
