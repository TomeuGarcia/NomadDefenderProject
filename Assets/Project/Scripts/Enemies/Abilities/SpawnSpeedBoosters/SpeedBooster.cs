using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.ObjectPooling;
using UnityEngine;

public class SpeedBooster : RecyclableObject
{
    [System.Serializable]
    public class Boost
    {
        [SerializeField, Min(0)] private float _speedMultiplier = 1.5f;
        [SerializeField, Min(0)] private float _duration = 0.25f;
        [SerializeField, Min(0)] private float _accelerateDuration = 0.2f;
        [SerializeField, Min(0)] private float _decelerateDuration = 0.2f;
        
        public float SpeedMultiplier => _speedMultiplier;
        public float Duration => _duration;   
        public float AccelerateDuration => _accelerateDuration;   
        public float DecelerateDuration => _decelerateDuration;   
    }

    
    private SpeedBoosterConfig _config;
    private readonly HashSet<ISpeedBoosterUser> _usersToIgnore = new (1);


    public void Init(SpeedBoosterConfig config)
    {
        _config = config;
        StartCoroutine(Lifetime());
    }

    internal override void RecycledInit()
    {
        _usersToIgnore.Clear();
    }

    internal override void RecycledReleased()
    {
        
    }

    public void AddUserToIgnore(ISpeedBoosterUser speedBoosterUser)
    {
        _usersToIgnore.Add(speedBoosterUser);
    }
    
    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ISpeedBoosterUser speedBoosterUser) && 
            !_usersToIgnore.Contains(speedBoosterUser))
        {
            speedBoosterUser.ApplySpeedBoosterMultiplier(_config.Boost);
        }
    }

    private IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(_config.SpeedBoosterLifetime);
        Recycle();
    }


}