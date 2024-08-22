using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlowFireRateTurretBuildingVisuals : MonoBehaviour
{
    private TurretBuilding _owner;

    [SerializeField] private Image _durationFillImage;
    [SerializeField] private Image _insideImage;
    private bool _isMax;
    private float _maxTimeBetweenShots;

    private Color originalFillImageColor;
    private Color originalInsideImageColor;

    private void Awake()
    {
        originalFillImageColor = _durationFillImage.color;
        originalInsideImageColor = _insideImage.color;
    }


    private void Update()
    {
        UpdateFillAmount();
    }


    public void TurretPlacedInit(TurretBuilding owner, float maxTimeBetweenShots)
    {
        _owner = owner;
        _maxTimeBetweenShots = maxTimeBetweenShots;
    }


    private void UpdateFillAmount()
    {
        float timePer1 = _owner.TimeSinceLastShot / _maxTimeBetweenShots;

        bool reachedMax = timePer1 > 0.999f;
        if (reachedMax && !_isMax)
        {
            OnReachedMax();
        }

        if (timePer1 < 0.001f)
        {
            OnTimeReset();
        }

        _isMax = reachedMax;
        _durationFillImage.fillAmount = timePer1;
    }


    private void OnReachedMax()
    {
        _insideImage.DOColor(Color.green, 0.45f)
            .OnComplete(
            () => _insideImage.DOColor(originalInsideImageColor, 0.45f)
            )
            .SetLoops(-1);     
    }
    
    private void OnTimeReset()
    {
        _insideImage.DOKill();
        _insideImage.color = originalInsideImageColor;

        _durationFillImage.DOKill();
        _durationFillImage.color = Color.red;
        _durationFillImage.DOColor(originalFillImageColor, 0.8f);
    }

}
