using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AmbienceAudio : MonoBehaviour
{
    [SerializeField] private LoopingSound[] loopingSounds;
    [SerializeField] private RandomSoundsCollection[] randomSoundsCollections;  



    public void Play()
    {
        foreach (LoopingSound loopingSound in loopingSounds)
        {
            loopingSound.StartPlaying();        
        }

        foreach (RandomSoundsCollection randomSoundsCollection in randomSoundsCollections)
        {
            randomSoundsCollection.StartPlaying();
        }
    }

    public void Stop()
    {
        foreach (LoopingSound loopingSound in loopingSounds)
        {
            loopingSound.StopPlaying();
        }

        foreach (RandomSoundsCollection randomSoundsCollection in randomSoundsCollections)
        {
            randomSoundsCollection.StopPlaying();
        }
    }
}
