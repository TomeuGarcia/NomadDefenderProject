using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
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

    [SerializeField] private Transform _viewHolder;
    [SerializeField] private Transform _speedBoostApplyRotator;


    public void Init(SpeedBoosterConfig config)
    {
        _config = config;
        StartCoroutine(Lifetime());
        PlayAppearAnimation();
    }

    private void OnEnable()
    {
        TDGameManager.OnGameFinishStart += ForceRecycle;
    }
    private void OnDisable()
    {
        TDGameManager.OnGameFinishStart -= ForceRecycle;
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
            ApplySpeedBoost(speedBoosterUser);
        }
    }

    private void ApplySpeedBoost(ISpeedBoosterUser speedBoosterUser)
    {
        speedBoosterUser.ApplySpeedBoosterMultiplier(_config.Boost);

        _speedBoostApplyRotator.DOComplete();
        _speedBoostApplyRotator.DOBlendableLocalRotateBy(Vector3.up * 360f, 0.25f, RotateMode.LocalAxisAdd)
            .SetEase(Ease.InOutSine);
    }

    private IEnumerator Lifetime()
    {
        Timer lifetimeTimer = new Timer(_config.SpeedBoosterLifetime);
        while (!lifetimeTimer.HasFinished())
        {
            lifetimeTimer.Update(GameTime.DeltaTime);
            yield return null;
        }

        Task disappearTask = PlayDisappearAnimation();
        yield return new WaitUntil(() => disappearTask.IsCompleted);
        Recycle();
    }
    
    private void PlayAppearAnimation()
    {
        _viewHolder.DOComplete();
        _viewHolder.DOPunchScale(Vector3.one * 0.5f, 0.3f);
    }
    
    private async Task PlayDisappearAnimation()
    {
        _viewHolder.DOComplete();
        await _viewHolder.DOPunchScale(Vector3.one * -0.5f, 0.3f)
            .AsyncWaitForCompletion();
    }

    private void ForceRecycle()
    {
        StopAllCoroutines();
        _viewHolder.DOComplete();
        Recycle();
    }
}