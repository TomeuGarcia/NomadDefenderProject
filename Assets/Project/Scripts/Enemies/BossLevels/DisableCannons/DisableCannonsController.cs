using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class DisableCannonsController : MonoBehaviour
{
    [Header("FACTORY")]
    [SerializeField] private DisableMineFactory _disableMineFactory;

    [Header("CANNONS")] 
    [SerializeField] private DisableCannon[] _disableCannons;

    [Header("TESTING")] 
    [SerializeField] private Transform _targetTest;
    [SerializeField, Min(0)] private Vector2Int _targetOffsetTest = new Vector2Int(8, 8);
    [SerializeField, Min(0)] private float _delayBetweenCannons = 0.2f;

    
    
    private void Awake()
    {
        _disableMineFactory.Init();
    
        foreach (DisableCannon disableCannon in _disableCannons)
        {
            disableCannon.Init(_disableMineFactory);
        }
    }
    
    
    
    [Button()]
    private void TestLaunchMissile()
    {
        StartCoroutine(DoTestLaunchMissile());
    }
    
    private IEnumerator DoTestLaunchMissile()
    {
        for (int i = 0; i < _disableCannons.Length; ++i)
        {
            Vector3 missileEndPosition = _targetTest.position + new Vector3(
                Random.Range(-_targetOffsetTest.x, _targetOffsetTest.x), 
                0, 
                Random.Range(-_targetOffsetTest.y, _targetOffsetTest.y));
        
        
            _disableCannons[i].LaunchMissile(missileEndPosition);
            
            yield return new WaitForSeconds(_delayBetweenCannons);
        }
    }
}
