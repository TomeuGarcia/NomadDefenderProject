using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningFromSound : MonoBehaviour
{
    [Header("COMPONENTS")]
    [SerializeField] RandomSoundsCollection randomSourceCollection;
    [SerializeField] Light lightSource;

    [Header("COMPONENTS")]
    [SerializeField] private Vector2 timeRange;
    [SerializeField] private Vector2 intensityRange;

    private IEnumerator lightningStrike;
    private bool striking = false;

    private void OnEnable()
    {
        randomSourceCollection.OnRandomSoundPlayed += ActivateLightning;
        lightSource.enabled = true;
    }
    private void OnDisable()
    {
        randomSourceCollection.OnRandomSoundPlayed -= ActivateLightning;
        lightSource.enabled = false;
    }

    private void ActivateLightning()
    {
        if (striking)
        {
            StopCoroutine(lightningStrike);
            lightSource.DOKill();
        }

        lightningStrike = LightningStrike();
        StartCoroutine(lightningStrike);
    }

    IEnumerator LightningStrike()
    {
        striking = true;
        float timeLerp = 0.1f;
        lightSource.DOIntensity(Random.Range(intensityRange.x, intensityRange.y), timeLerp);
        yield return new WaitForSeconds(Random.Range(timeRange.x, timeRange.y));

        lightSource.DOIntensity(0.0f, timeLerp);
        striking = true;
    }
}
