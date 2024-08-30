using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ShotgunBullet : MonoBehaviour, TurretMultipleProjectileView.ISource
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private TrailRenderer _trailRenderer;

    private IListener _listener;
    private bool _disappearing;

    
    public interface IListener
    {
        void OnEnemyHit(Enemy enemy);
        void OnDisappearCompleted();
    }

    

    public void Configure(IListener listener)
    {
        _listener = listener;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag("Enemy") || _disappearing)
        {
            return;
        }

        OnEnemyHit(other.gameObject.GetComponent<Enemy>());
    }
    
    private void OnEnemyHit(Enemy enemy)
    {
        if (_disappearing) return;
        
        _listener.OnEnemyHit(enemy);
        StartDisappearing();
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
            StartDisappearing();
        }
    }

    public Transform GetAddOnsParent()
    {
        return transform;
    }

    

    private void StartDisappearing()
    {
        StartCoroutine(WaitToDisable());
    }
    private IEnumerator WaitToDisable()
    {
        _disappearing = true;
        
        _rigidbody.DOKill(false);
        _trailRenderer.emitting = false;

        yield return new WaitForSeconds(0.2f);
        Disable();
    }
    private void Disable()
    {
        _disappearing = false;

        gameObject.SetActive(false);
        
        _listener.OnDisappearCompleted();
    }

}