using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFlash : MonoBehaviour
{
    [SerializeField] private Image image;

    public void FadeIn(float duration)
    {
        image.DOFade(0.1f, duration);
    }
    public void FadeOut(float duration)
    {
        image.DOFade(0.0f, duration);
    }
}
