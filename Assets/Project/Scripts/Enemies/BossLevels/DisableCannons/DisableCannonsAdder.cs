using System;
using NaughtyAttributes;
using UnityEngine;

public class DisableCannonsAdder : MonoBehaviour
{
    [System.Serializable]
    private class DisableCannonGroup
    {
        [SerializeField] private DisableCannon[] _disableCannons;
        public DisableCannon[] DisableCannons => _disableCannons;
    }
    
    [Header("CONTROLLER")] 
    [SerializeField] private DisableCannonsController _disableCannonsController;
    
    [Header("CANNON GROUPS")]
    [SerializeField] private DisableCannonGroup[] _disableCannonAdditionGroups;
    private int _nextGroupIndex;


    private void Awake()
    {
        _nextGroupIndex = 0;
        
        foreach (DisableCannonGroup disableCannonsGroup in _disableCannonAdditionGroups)
        {
            foreach (DisableCannon disableCannon in disableCannonsGroup.DisableCannons)
            {
                disableCannon.Init();
            }
        }
    }

    private bool CanKeepAdding()
    {
        return _nextGroupIndex < _disableCannonAdditionGroups.Length;
    }


    [Button()]
    public void AddNext()
    {
        if (!CanKeepAdding())
        {
            return;
        }

        DisableCannon[] cannonsToAdd = _disableCannonAdditionGroups[_nextGroupIndex].DisableCannons;
        StartCoroutine(_disableCannonsController.AddAvailableCannons(cannonsToAdd));
        ++_nextGroupIndex;
    }


    [Button()]
    public void RemoveAllAndReset()
    {
        StartCoroutine(_disableCannonsController.RemoveAllAvailableCannons());
        _nextGroupIndex = 0;
    }
    
}