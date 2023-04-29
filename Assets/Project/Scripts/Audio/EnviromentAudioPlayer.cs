using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviromentAudioPlayer : MonoBehaviour
{
    [SerializeField] LoopingSound loopingSound;

    void Start()
    {
        if(loopingSound != null)
        {
            loopingSound.StartPlaying();
        }
    }
}
