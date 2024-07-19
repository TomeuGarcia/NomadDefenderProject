using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacilityFlickeringLight : MonoBehaviour
{
    [Header("GENERAL")]
    [SerializeField] private Light _light;
    [SerializeField] private bool _flicker;
    [SerializeField] private MeshRenderer _lightMR;
    [SerializeField] private AnimationCurve _lightMatCurve;
    private Material _lightMat;

    [Header("INTENSITY FLICKER")]
    [SerializeField] private List<AnimationCurve> _startAnimationCurves = new();
    [SerializeField] private List<AnimationCurve> _endAnimationCurves = new();

    [SerializeField] private Vector2 _intensityRange;
    [SerializeField] private Vector2 _frequencyRange;
    [SerializeField] private Vector2 _delayRange;

    [Header("TURN OFF FLICKER")]
    [SerializeField] private bool _canTurnOffFlicker;
    [SerializeField] private float _turnOffChance;
    [SerializeField] private Vector2 _TODurationRange;
    [SerializeField] private Vector2 _TOBetweenSequenceRange;
    [SerializeField] private Vector2Int _TOSequenceRange;

    private float _defaultIntensity;

    private void Awake()
    {
        if(_lightMR != null)
        {
            _lightMat = _lightMR.materials[0];
        }

        _defaultIntensity = _light.intensity;
        _light.gameObject.SetActive(false);
    }

    public void Activate()
    {
        ChooseNextAction();
    }

    public void Deactivate()
    {
        StopAllCoroutines();
        _lightMat.DOKill();
        _light.DOKill();

        _light.intensity = _defaultIntensity;
        if (_lightMR != null)
        {
            _lightMat.SetFloat(MaterialProperties.Activate, 0.0f);
        }
    }

    private void ChooseNextAction()
    {
        if (!_flicker) return;

        if(!_canTurnOffFlicker)
        {
            StartCoroutine(Flicker());
        }
        else
        {
            if(_turnOffChance == 1.0f || (_turnOffChance > 0.0f && Random.Range(0.0f, 1.0f) <= _turnOffChance))
            {
                StartCoroutine(TurnOffFlicker());
            }
            else
            {
                StartCoroutine(Flicker());
            }
        }
    }

    private IEnumerator Flicker()
    {
        int startAnimCurve = Random.Range(0, _startAnimationCurves.Count);
        int endAnimCurve = Random.Range(0, _endAnimationCurves.Count);
        float halfFrequencyTime = Random.Range(_frequencyRange.x, _frequencyRange.y) / 2.0f;
        float randomIntensity = Random.Range(_intensityRange.x, _intensityRange.y);

        _light.DOIntensity(randomIntensity, halfFrequencyTime)
            .SetEase(_startAnimationCurves[startAnimCurve]);
        if (_lightMR != null)
        {
            float endValue = _lightMatCurve.Evaluate(randomIntensity / _defaultIntensity);
            _lightMat.DOFloat(endValue, MaterialProperties.Activate, halfFrequencyTime);
        }
        yield return new WaitForSeconds(halfFrequencyTime);

        _light.DOIntensity(_defaultIntensity, halfFrequencyTime)
            .SetEase(_endAnimationCurves[endAnimCurve]);
        if (_lightMR != null)
        {
            _lightMat.DOFloat(1.0f, MaterialProperties.Activate, halfFrequencyTime);
        }
        yield return new WaitForSeconds(halfFrequencyTime);
        yield return new WaitForSeconds(Random.Range(_delayRange.x, _delayRange.y));

        ChooseNextAction();
    }

    private IEnumerator TurnOffFlicker()
    {
        int totalSequences = Random.Range(_TOSequenceRange.x, _TOSequenceRange.y);
        
        for(int i = 0; i < totalSequences; i++)
        {
            _light.intensity = 0.0f;
            yield return new WaitForSeconds(Random.Range(_TODurationRange.x, _TODurationRange.y));
            _light.intensity = _defaultIntensity;

            yield return new WaitForSeconds(Random.Range(_TOBetweenSequenceRange.x, _TOBetweenSequenceRange.y));
        }

        yield return new WaitForSeconds(Random.Range(_delayRange.x, _delayRange.y));

        ChooseNextAction();
    }
}
