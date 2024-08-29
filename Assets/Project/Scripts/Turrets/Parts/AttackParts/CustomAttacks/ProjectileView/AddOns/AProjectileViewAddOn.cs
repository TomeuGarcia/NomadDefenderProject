using System;
using System.Threading.Tasks;
using Scripts.ObjectPooling;
using UnityEngine;

public abstract class AProjectileViewAddOn : RecyclableObject
{
    private Transform _originalParent;
    private float _spawnTime;

    private void Awake()
    {
        _originalParent = transform.parent;
    }
    
    public void OnProjectileSpawned(Transform parent)
    {
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        _spawnTime = GetCurrentLifetimeTime();
        
        DoOnProjectileSpawned();
    }
    
    public void OnProjectileDisappear()
    {
        DoOnProjectileDisappear();
        WaitForEffectsToFinish();
    }

    private async void WaitForEffectsToFinish()
    {
        transform.SetParent(_originalParent);
        while (!AllAffectsFinished())
        {
            await Task.Yield();
        }
        
        Recycle();
    }

    protected abstract bool AllAffectsFinished();
    
    
    protected virtual void DoOnProjectileSpawned() { }
    protected virtual void DoOnProjectileDisappear() { }
    public virtual void OnProjectileHitsTarget(Transform target) { }


    private float GetCurrentLifetimeTime()
    {
        return Time.time * GameTime.TimeScale;
    }

    protected bool HasPassedTimeSinceSpawned(float time)
    {
        return GetCurrentLifetimeTime() - _spawnTime > time;
    }
    
    
    internal sealed override void RecycledInit() { }
    internal sealed override void RecycledReleased() { }
}