using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;


public class DisableCannon : MonoBehaviour
{
    [Header("ANIMATIONS")]
    [SerializeField] private DisableCannonShootAnimator _shootAnimator;
    
    [Header("PARTICLES")]
    [SerializeField] private ParticleSystem _missileParticle;
    [SerializeField] private ParticleSystem _missileLandParticle;
    
    [Header("TESTING")]
    [SerializeField] private Transform _targetTest;
    [SerializeField, Min(0)] private Vector2Int _targetOffsetTest = new Vector2Int(8, 8);
    [SerializeField] private GameObject _missileTest;


    private void Awake()
    {
        _shootAnimator.Init();
    }


    [Button()]
    private void TestLaunchMissile()
    {
        Vector3 missileEndPosition = _targetTest.position + new Vector3(
            Random.Range(-_targetOffsetTest.x, _targetOffsetTest.x), 
            0, 
            Random.Range(-_targetOffsetTest.y, _targetOffsetTest.y));
        
        StartCoroutine(MissileTravel(missileEndPosition));
    }

    private IEnumerator MissileTravel(Vector3 missileEndPosition)
    {
        _shootAnimator.PlayAnimation();
        
        _missileTest.SetActive(false);
        
        Vector3 missileEndOffset = missileEndPosition - _missileParticle.transform.position;
        
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule = _missileParticle.velocityOverLifetime;
        velocityOverLifetimeModule.orbitalOffsetX = missileEndOffset.x;
        velocityOverLifetimeModule.orbitalOffsetY = missileEndOffset.y;
        velocityOverLifetimeModule.orbitalOffsetZ = missileEndOffset.z;
        
        _missileParticle.Play();
        
        yield return new WaitForSeconds(_missileParticle.main.startLifetime.constant);

        _missileTest.transform.position = missileEndPosition;
        _missileTest.SetActive(true);

        _missileLandParticle.transform.position = missileEndPosition;
        _missileLandParticle.Play();
    } 
    
    

}