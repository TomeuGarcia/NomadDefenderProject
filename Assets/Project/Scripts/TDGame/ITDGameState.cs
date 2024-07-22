using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITDGameState 
{
    bool FirstCardWasPlayed { get; }
    bool GameHasFinished { get; }
}
