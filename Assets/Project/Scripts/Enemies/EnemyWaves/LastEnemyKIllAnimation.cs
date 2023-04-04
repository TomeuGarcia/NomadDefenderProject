using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastEnemyKIllAnimation : MonoBehaviour
{
    [SerializeField] float animationTime;
    [SerializeField] AnimationCurve animationCurve;

    public IEnumerator StartAnimation()
    {
        Time.timeScale = 0.0f;
        float currentTime = 0.0f;

        while(currentTime < animationTime)
        {
            Debug.Log(currentTime);
            currentTime += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(animationCurve.Evaluate(currentTime / animationTime), 0.1f, 1.0f);

            yield return null;
        }

        Time.timeScale = 1.0f;
    }
}
