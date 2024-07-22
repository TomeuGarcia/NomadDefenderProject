
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class GameTime
{
    static public float TimeScale { get; private set; } = 1f;
    static public float DeltaTime => Time.deltaTime * TimeScale;

    static public void SetTimeScale(float timeScale)
    {
        TimeScale = timeScale;
    }

    static public IEnumerator TweenTimeScale(float endTimeScale, float duration)
    {
        float startTimeScale = TimeScale;

        for (float t = 0f; t < duration; t += Time.deltaTime) 
        {
            TimeScale = Mathf.Lerp(startTimeScale, endTimeScale, t / duration);
            yield return null;
        }
        TimeScale = endTimeScale;
    }


    public static IEnumerator WaitForSeconds(float duration)
    {
        duration = Mathf.Max(duration, 0);
        float timer = 0;

        while (timer < duration)
        {
            timer += DeltaTime;
            yield return null;
        }
    }

}
