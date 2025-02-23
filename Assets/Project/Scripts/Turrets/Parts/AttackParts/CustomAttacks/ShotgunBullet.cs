using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ShotgunBullet : MonoBehaviour, TurretMultipleProjectileView.ISource
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] private ParticleSystem _hitParticles;

    private IListener _listener;
    private bool _disappearing;

    
    public interface IListener
    {
        bool DoCheckEnemyOnTriggerEnter(Collider other, out Enemy enemy);
        void OnEnemyHit(Enemy enemy);
        void OnDisappearCompleted();
    }

    

    public void Configure(IListener listener)
    {
        _listener = listener;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_listener.DoCheckEnemyOnTriggerEnter(other, out Enemy enemy))
        {
            OnEnemyHit(enemy);
        }
    }

    private void OnEnemyHit(Enemy enemy)
    {
        if (_disappearing) return;
        
        _listener.OnEnemyHit(enemy);
        StartDisappearing(true);
    }


    public void StartMoving(Quaternion rotation, float maxDistance, float duration)
    {
        transform.localPosition = Vector3.zero;
        transform.rotation = rotation;
        
        gameObject.SetActive(true);
        _disappearing = false;
        
        _trailRenderer.emitting = true;
        _trailRenderer.Clear();


        
        Vector3 goalPosition = transform.position + (transform.forward * maxDistance);
        
        _rigidbody.DOMove(goalPosition, duration)
            .OnComplete(OnEndReached);
    }
    
    private void OnEndReached()
    {
        if (!_disappearing)
        {
            StartDisappearing(false);
        }
    }

    public Transform GetAddOnsParent()
    {
        return transform;
    }

    

    private void StartDisappearing(bool hitEnemy)
    {
        StartCoroutine(WaitToDisable(hitEnemy));
    }
    private IEnumerator WaitToDisable(bool hitEnemy)
    {
        _disappearing = true;
        
        _rigidbody.DOKill(false);
        _trailRenderer.emitting = false;

        if (hitEnemy)
        {
            _hitParticles.Play();
            yield return new WaitUntil(() => !_hitParticles.isEmitting);
        }
        else
        {
            yield return new WaitForSeconds(0.2f);
        }

        Disable();
    }
    private void Disable()
    {
        _disappearing = false;

        gameObject.SetActive(false);
        
        _listener.OnDisappearCompleted();
    }

}