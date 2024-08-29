using System;
using DG.Tweening;
using UnityEngine;

public class ShotgunBullet : MonoBehaviour, TurretMultipleProjectileView.ISource
{
    [SerializeField] private Rigidbody _rigidbody;

    private IListener _listener;
    
    
    public interface IListener
    {
        void OnEnemyHit(Enemy enemy);
        void OnEndReached();
    }

    

    public void Configure(IListener listener)
    {
        _listener = listener;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag("Enemy"))
        {
            return;
        }

        OnEnemyHit(other.gameObject.GetComponent<Enemy>());
    }
    
    private void OnEnemyHit(Enemy enemy)
    {
        _listener.OnEnemyHit(enemy);
        _rigidbody.DOKill(false);
    }


    public void StartMoving(Quaternion rotation, float maxDistance, float duration)
    {
        transform.rotation = rotation;
        Vector3 goalPosition = transform.forward * maxDistance;
        
        _rigidbody.DOMove(goalPosition, duration)
            .OnComplete(OnEndReached);
    }
    
    private void OnEndReached()
    {
        _listener.OnEndReached();
    }

    public Transform GetAddOnsParent()
    {
        return transform;
    }
}