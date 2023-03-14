
using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class GameTime
{
    static public float TimeScale { get; private set; } = 1f;

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

}
