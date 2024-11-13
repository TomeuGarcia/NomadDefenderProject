using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EnemyRedirectedDamage : MonoBehaviour
{
    [SerializeField] private AnimationCurve _movementEase = AnimationCurve.Linear(0,0, 1, 1);
    [SerializeField] private ParticleSystem _reachTargetParticles;
    [SerializeField] private TrailRenderer _trailRenderer;
    
    private Enemy _enemySource;
    private PathLocation _originLocation;
    private PathLocation _targetLocation;

    private const float MOVE_SPEED = 10.0f;
    private static Vector3 CONTROL_POINTS_OFFSET = Vector3.up * 3.0f;

    private Timer _moveTimer;
    private Vector3 _startPoint;
    private Vector3 _endPoint;

    private float _defaultTrailTime;

    private void Awake()
    {
        _defaultTrailTime = _trailRenderer.time;
    }


    public void Init(Enemy enemySource, PathLocation originLocation, PathLocation targetLocation)
    {
        _enemySource = enemySource;
        _originLocation = originLocation;
        _targetLocation = targetLocation;
        _startPoint = transform.position;
        _endPoint = _targetLocation.Position + Vector3.up;
        
        float moveDuration = Vector3.Distance(_originLocation.Position, _targetLocation.Position) / MOVE_SPEED;
        _moveTimer = new Timer(moveDuration);

        transform.forward = Vector3.up;
        StartCoroutine(MoveToTarget());
    }

    
    private IEnumerator MoveToTarget()
    {
        while (!_moveTimer.HasFinished())
        {
            _moveTimer.Update(Time.unscaledDeltaTime * Mathf.Min(Time.timeScale, 2.0f));
            _trailRenderer.time = _defaultTrailTime / Mathf.Max(Time.timeScale, 0.1f);
            UpdatePosition();
            yield return null;
        }

        OnTargetReached();
    }

    private void UpdatePosition()
    {
        Vector3 startControlPoint = _startPoint + CONTROL_POINTS_OFFSET;
        Vector3 endControlPoint = _endPoint + CONTROL_POINTS_OFFSET;

        Vector3 previousPosition = transform.position;
        Vector3 currentPosition = DOCurve.CubicBezier.GetPointOnSegment(
            _startPoint, startControlPoint, _endPoint, endControlPoint, 
            _movementEase.Evaluate(_moveTimer.Ratio01)
            );
        transform.position = currentPosition;

        transform.forward = (currentPosition - previousPosition).normalized;
    }

    private void OnTargetReached()
    {
        _enemySource.AttackPathLocation(_targetLocation);
        StartCoroutine(PlayEndAnimation());
    }

    private IEnumerator PlayEndAnimation()
    {
        _reachTargetParticles.Play();
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
