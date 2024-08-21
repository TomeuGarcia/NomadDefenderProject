using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Popeye.Modules.VFX.Generic.ParticleBehaviours;
using Scripts.ObjectPooling;
using UnityEngine;

public class InterpolatorRecycleParticle : RecyclableObject
{
    [SerializeField] internal bool _interpolateOnInit;
    [SerializeField] private float _despawnDelay = 0.0f;
    [SerializeField] internal InterpolatorRecycleParticleData[] _interpolations;

    [Header("LIGHT")]
    [SerializeField] private Light _light;
    [SerializeField] private float _duration;
    [SerializeField] private float _intensityGoal;
    [SerializeField] private Ease _lightEase = Ease.Linear;
    private float _initialIntensity;

    internal List<TrailRenderer> _trailRenderers = new();

    private int _completedInterpolations;
    private int _totalInterpolations;

    private bool _waitingForLight;


    private void Awake()
    {
        if (_light != null)
        {
            _totalInterpolations = _interpolations.Length + 1;

            _initialIntensity = _light.intensity;
            _waitingForLight = true;
        }
        else
        {
            _totalInterpolations = _interpolations.Length;
            _waitingForLight = false;
        }

        foreach (var interpolation in _interpolations)
        {
            interpolation.Awake();
            if (interpolation.TrailRenderers.Count > 0)
            {
                _trailRenderers.AddRange(interpolation.TrailRenderers);
            }
        }

        TrailEmission(false);
    }

    internal override void RecycledInit()
    {
        _completedInterpolations = 0;

        foreach (var interpolation in _interpolations)
        {
            foreach (var material in interpolation.Materials)
            {
                Setup(material, interpolation.FloatSetupDatas);
            }
        }

        if (_interpolateOnInit)
        {
            Play();
        }

        TrailInit();
    }

    private IEnumerator TrailInit()
    {
        yield return new WaitForEndOfFrame();
        TrailEmission(true);
    }

    private void TrailEmission(bool emission)
    {
        foreach (var trail in _trailRenderers)
        {
            trail.Clear();
            trail.emitting = emission;
        }
    }

    private void Setup(Material material, MaterialFloatSetupConfig[] setupConfigs)
    {
        MaterialInterpolator.Setup(material, setupConfigs);
    }

    public void Play()
    {
        if (_light != null)
        {
            _light.DOIntensity(_intensityGoal, _duration).SetEase(_lightEase).OnComplete(LightCompleted);
        }

        foreach (var interpolation in _interpolations)
        {
            foreach (var material in interpolation.Materials)
            {
                StartCoroutine(ApplyInterpolations(material, interpolation.FloatInterpolationDatas));
            }
        }
    }

    public void ForceStop()
    {
        Reset();
    }

    private IEnumerator ApplyInterpolations(Material material, MaterialFloatInterpolationConfig[] interpolationConfigs)
    {
        yield return MaterialInterpolator.ApplyInterpolations(material, interpolationConfigs);

        StartCoroutine(FinishedInterpolation());
    }

    private void LightCompleted()
    {
        DoLightCompleted();
    }

    private void DoLightCompleted()
    {
        _light.intensity = 0.0f;

        StartCoroutine(FinishedInterpolation());
    }

    private IEnumerator FinishedInterpolation()
    {
        _completedInterpolations++;

        if (_completedInterpolations >= _totalInterpolations)
        {
            yield return new WaitForSeconds(_despawnDelay);
            Reset();
        }
    }

    internal virtual void Reset()
    {
        if (_light != null)
        {
            _light.intensity = _initialIntensity;
        }

        TrailEmission(false);
        Recycle();
    }

    internal override void RecycledReleased() { }
}