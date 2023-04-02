using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DialogueBackgroundDisplayer : MonoBehaviour
{
    [Header("CONSOLE DIALOG SYSTEM")]
    [SerializeField] private ConsoleDialogSystem consoleDialogSystem;


    [Header("BACKGROUND")]
    [SerializeField] private Image backgroundFadeImage;
    private Material backgroundMaterial;
    private Coroutine backgroundFadeInCoroutine = null;
    private Coroutine backgroundFadeOutCoroutine = null;
    private const float BACKGROUND_FADE_MIN = 0.00001f;
    private const float BACKGROUND_FADE_MAX = 1.5f;
    private const float BACKGROUND_FADE_RATE = 1.0f;
    private const string FADE_STRENGTH_PROPERTY = "_FadeStrength";


    [Header("PREDEFINED BACKGROUND")]
    [SerializeField] private Image predefinedBackgroundFadeImage;


    private void Awake()
    {
        backgroundMaterial = backgroundFadeImage.material;
        backgroundMaterial.SetFloat(FADE_STRENGTH_PROPERTY, BACKGROUND_FADE_MIN);


        predefinedBackgroundFadeImage.DOFade(0f, 0.01f);
    }

    private void OnEnable()
    {
        consoleDialogSystem.OnQueryBackgroundFadeIn += StartBackgroundFadeIn;
        consoleDialogSystem.OnQueryBackgroundFadeOut += StartBackgroundFadeOut;
    }

    private void OnDisable()
    {
        consoleDialogSystem.OnQueryBackgroundFadeIn -= StartBackgroundFadeIn;
        consoleDialogSystem.OnQueryBackgroundFadeOut -= StartBackgroundFadeOut;
    }



    private void StartBackgroundFadeIn()
    {
        // Do it the scuffed way for now
        //predefinedBackgroundFadeImage.DOComplete();
        predefinedBackgroundFadeImage.DOFade(1f, 0.2f);
        return;

        if (backgroundFadeOutCoroutine != null)
        {
            StopCoroutine(backgroundFadeOutCoroutine);
        }

        backgroundFadeInCoroutine = StartCoroutine(BackgroundFadeInCoroutine());
    }

    private void StartBackgroundFadeOut()
    {
        // Do it the scuffed way for now
        //predefinedBackgroundFadeImage.DOComplete();
        predefinedBackgroundFadeImage.DOFade(0f, 0.2f);
        return;

        if (backgroundFadeInCoroutine != null)
        {
            StopCoroutine(backgroundFadeInCoroutine);
        }

        backgroundFadeOutCoroutine = StartCoroutine(BackgroundFadeOutCoroutine());
    }


    private IEnumerator BackgroundFadeInCoroutine()
    {
        float value = backgroundMaterial.GetFloat(FADE_STRENGTH_PROPERTY);

        while (value < BACKGROUND_FADE_MAX)
        {
            value += Time.deltaTime * BACKGROUND_FADE_RATE;

            backgroundMaterial.SetFloat(FADE_STRENGTH_PROPERTY, value);
            yield return null;
            Debug.Log(value);
        }

        backgroundMaterial.SetFloat(FADE_STRENGTH_PROPERTY, BACKGROUND_FADE_MAX);

        backgroundFadeInCoroutine = null;
    }

    private IEnumerator BackgroundFadeOutCoroutine()
    {
        float value = backgroundMaterial.GetFloat(FADE_STRENGTH_PROPERTY);

        while (value > BACKGROUND_FADE_MIN)
        {
            value -= Time.deltaTime * BACKGROUND_FADE_RATE;

            backgroundMaterial.SetFloat(FADE_STRENGTH_PROPERTY, value);
            yield return null;
            Debug.Log(value);
        }

        backgroundMaterial.SetFloat(FADE_STRENGTH_PROPERTY, BACKGROUND_FADE_MIN);

        backgroundFadeOutCoroutine = null;

    }



}
