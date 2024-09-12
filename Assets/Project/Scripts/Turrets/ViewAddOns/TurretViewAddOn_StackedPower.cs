using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretViewAddOn_StackedPower : ATurretViewAddOn
{
    [SerializeField] private List<Transform> _meshes = new();
    private int _numberOfCards = 0;


    public ProjectileViewAddOn_StackedPower.IConfigurationSource ConfigurationSource { get; set; }
    

    internal override void RecycledInit()
    {

    }

    internal override void RecycledReleased()
    {

    }

    protected override void StartPlayingEffects()
    {
        _numberOfCards = Mathf.Clamp(ConfigurationSource.GetNumberOfCards(), 0, 10);
        float angleStep = 360.0f / _numberOfCards;

        for (int i = 0; i < _numberOfCards; i++)
        {
            _meshes[i].localRotation = Quaternion.Euler(Vector3.up * (angleStep * i));
            _meshes[i].gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if(_numberOfCards != ConfigurationSource.GetNumberOfCards())
        {
            StartPlayingEffects();
        }
    }

    protected override void StopPlayingEffects()
    {
        for (int i = 0; i < _meshes.Count; i++)
        {
            _meshes[i].localRotation = Quaternion.identity;
            _meshes[i].gameObject.SetActive(false);
        }
    }
}