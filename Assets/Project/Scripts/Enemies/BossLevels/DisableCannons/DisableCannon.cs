using System;
using System.Collections;
using UnityEngine;


public class DisableCannon : MonoBehaviour
{
    [Header("ANIMATIONS")]
    [SerializeField] private DisableCannonShootAnimator _shootAnimator;
    
    [Header("PARTICLES")]
    [SerializeField] private ParticleSystem _missileParticle;
    [SerializeField] private ParticleSystem _missileLandParticle;
    

    private DisableMineFactory _disableMineFactory;
    

    public void Init(DisableMineFactory disableMineFactory)
    {
        _disableMineFactory = disableMineFactory;
        _shootAnimator.Init();
    }



    public void LaunchMissile(Vector3 missileEndPosition)
    {
        StartCoroutine(MissileTravel(missileEndPosition));
    }

    private IEnumerator MissileTravel(Vector3 missileEndPosition)
    {
        _shootAnimator.PlayAnimation();
        
        Vector3 missileEndOffset = missileEndPosition - _missileParticle.transform.position;
        
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule = _missileParticle.velocityOverLifetime;
        velocityOverLifetimeModule.orbitalOffsetX = missileEndOffset.x;
        velocityOverLifetimeModule.orbitalOffsetY = missileEndOffset.y;
        velocityOverLifetimeModule.orbitalOffsetZ = missileEndOffset.z;
        
        _missileParticle.Play();
        
        yield return new WaitForSeconds(_missileParticle.main.startLifetime.constant);


        _missileLandParticle.transform.position = missileEndPosition;
        _missileLandParticle.Play();
        
        _disableMineFactory.Create(missileEndPosition);
    } 
    
    

}