using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Scripts.ObjectPooling;
using UnityEngine;

public class BuildingDisableWave : RecyclableObject
{
    [SerializeField] private MeshRenderer _meshRenderer;
    private Material _mashMaterial;
    private BuildingDisableWaveConfig _config;
    
    private Transform MeshTransform => _meshRenderer.transform;


    
    private void Awake()
    {
        _mashMaterial = _meshRenderer.material;
    }

    internal override void RecycledInit()
    {
        _mashMaterial.DOComplete();
        _mashMaterial.SetFloat("_EmptyT", 0);
        _mashMaterial.SetFloat("_AlphaMultiplier", 1);

        MeshTransform.DOComplete();
    }

    internal override void RecycledReleased()
    {
        
    }

    public void Init(BuildingDisableWaveConfig config)
    {
        _config = config;
        StartCoroutine(Sequence());
    }

    private IEnumerator Sequence()
    {
        StartCoroutine(PlayAnimation());
        
        yield return new WaitForSeconds(_config.DelayBeforeApplying);
        ApplyWaveEffect();
    }

    private IEnumerator PlayAnimation()
    {
        MeshTransform.localScale = Vector3.zero;
        MeshTransform.DOScale(Vector3.one * (_config.Radius * 2), _config.Animation.Duration)
            .SetEase(Ease.OutQuart);

        _mashMaterial.DOFloat(1f, "_EmptyT", _config.Animation.Duration)
            .SetEase(_config.Animation.FadeEase);
        
        _mashMaterial.DOFloat(0f, "_AlphaMultiplier", _config.Animation.Duration)
            .SetEase(Ease.InExpo);
        
        yield return new WaitForSeconds(_config.Animation.Duration);
        Recycle();
    }

    private void ApplyWaveEffect()
    {
        Collider[] collidersInRange = Physics.OverlapSphere(transform.position, _config.Radius, 
            _config.BuildingsLayerMask, QueryTriggerInteraction.Collide);

        foreach (Collider colliderInRange in collidersInRange)
        {
            if (colliderInRange.TryGetComponent(out IDisableableBuilding disableableBuilding) &&
                disableableBuilding.CanBeDisabled())
            {
                BuildingDisableManager.Instance.HandleNewBuilding(disableableBuilding, _config.DisableDuration);
            }
        }
    }
}