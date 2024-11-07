using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Scripts.ObjectPooling;
using UnityEngine;

public class BuildingDisableWave : RecyclableObject
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Transform _EMPTransform;
    private Material _mashMaterial;
    private BuildingDisableWaveConfig _config;
    
    private Transform MeshTransform => _meshRenderer.transform;

    private Vector3 _EMPStartPosition;
    private Quaternion _EMPStartRotation;
    
    private void Awake()
    {
        _mashMaterial = _meshRenderer.material;

        _EMPStartPosition = _EMPTransform.position;
        _EMPStartRotation = _EMPTransform.localRotation;
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

        MeshTransform.localScale = Vector3.zero;
        _EMPTransform.localPosition = _EMPStartPosition;
        _EMPTransform.localRotation = _EMPStartRotation;
        _EMPTransform.gameObject.SetActive(true);

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
        _EMPTransform.DOLocalMove(_config.DropAnimation.EMPEndPosition, _config.DropAnimation.Duration).SetEase(_config.DropAnimation.Ease);
        _EMPTransform.DOLocalRotate(_config.DropAnimation.EMPEndRotation, _config.DropAnimation.Duration).SetEase(_config.DropAnimation.Ease);
        yield return new WaitForSeconds(_config.DropAnimation.Duration);
        _EMPTransform.gameObject.SetActive(false);

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