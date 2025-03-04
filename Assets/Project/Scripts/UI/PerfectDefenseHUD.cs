using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PerfectDefenseHUD : MonoBehaviour
{
    [System.Serializable]
    private class GraphicMotion
    {
        [SerializeField] private Graphic _graphic;
        [SerializeField, Min(0f)] private float _duration;
        [SerializeField] private Vector3 _positionOffset;
        [SerializeField] private AnimationCurve _positionEase;
        [SerializeField] private AnimationCurve _fadeEase;

        public void Play()
        {
            _graphic.DOComplete();
            _graphic.rectTransform.DOComplete();

            Color startColor = _graphic.color;
            startColor.a = 0f;
            _graphic.color = startColor;

            _graphic.rectTransform.localPosition = -_positionOffset;

            _graphic.DOFade(1f, _duration).SetEase(_fadeEase);
            _graphic.rectTransform.DOLocalMove(_positionOffset, _duration).SetEase(_positionEase);
        }
    }


    [SerializeField, Min(0)] private float _delay = 0.5f;
    [SerializeField] private CanvasGroup _holderCG;
    [SerializeField] private GraphicMotion[] _graphicMotions;


    private void OnEnable()
    {
        TDGameManager.OnPerfectDefenseVictoryStart += DelayedPlayAnimation;
    }
    private void OnDisable()
    {
        TDGameManager.OnPerfectDefenseVictoryStart -= DelayedPlayAnimation;
    }

    private void Awake()
    {
        _holderCG.alpha = 0f;
    }

    private void DelayedPlayAnimation()
    {
        StartCoroutine(DoDelayedPlayAnimation());
    }
    private IEnumerator DoDelayedPlayAnimation()
    {
        yield return new WaitForSeconds(_delay);
        PlayAnimation();
    }
    
    [Button()]
    private void PlayAnimation()
    {
        _holderCG.alpha = 1f;
        
        foreach (GraphicMotion graphicMotion in _graphicMotions)
        {
            graphicMotion.Play();
        }
    }
}
