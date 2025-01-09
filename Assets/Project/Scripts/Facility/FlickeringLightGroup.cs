using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLightGroup : MonoBehaviour
{
    [SerializeField] private List<FacilityFlickeringLight> _flickeringLights = new();
    [SerializeField] private GameObject[] _otherLights;

    private bool _isActive = false;

    private void Awake()
    {
        UpdateOtherLights();
    }

    public void Activate()
    {
        if(_isActive)
        {
            return;
        }
        
        _isActive = true;

        foreach (FacilityFlickeringLight light in _flickeringLights)
        {
            light.gameObject.SetActive(true);
            light.Activate();
        }

        UpdateOtherLights();
    }

    public void Deactivate()
    {
        if (!_isActive)
        {
           return;
        }
        
        _isActive = false;

        foreach (FacilityFlickeringLight light in _flickeringLights)
        {
            light.Deactivate();
            light.gameObject.SetActive(false);
        }

        UpdateOtherLights();
    }

    private void UpdateOtherLights()
    {
        foreach (GameObject otherLight in _otherLights)
        {
            otherLight.SetActive(_isActive);
        }
    }
}
