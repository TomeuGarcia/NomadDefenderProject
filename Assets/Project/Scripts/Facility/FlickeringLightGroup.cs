using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLightGroup : MonoBehaviour
{
    [SerializeField] private List<FacilityFlickeringLight> _flickeringLights = new();

    private bool _isActive = false;

    public void Activate()
    {
        if(!_isActive)
        {
            _isActive = true;

            foreach (FacilityFlickeringLight light in _flickeringLights)
            {
                light.gameObject.SetActive(true);
                light.Activate();
            }
        }
    }

    public void Deactivate()
    {
        if (_isActive)
        {
            _isActive = false;

            foreach (FacilityFlickeringLight light in _flickeringLights)
            {
                light.Deactivate();
                light.gameObject.SetActive(false);
            }
        }
    }
}
