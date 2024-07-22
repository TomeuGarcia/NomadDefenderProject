using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    public float Duration { get; set; }
    public float CurrentTime { get; private set; }
    
    public Timer(float duration)
    {
        Duration = duration;
        CurrentTime = 0f;
    }

    public void Update(float deltaTime)
    {
        CurrentTime += deltaTime;
    }

    public bool HasFinished()
    {
        return CurrentTime >= Duration;
    }

    public void Reset()
    {
        CurrentTime = 0f;    
    }

}
