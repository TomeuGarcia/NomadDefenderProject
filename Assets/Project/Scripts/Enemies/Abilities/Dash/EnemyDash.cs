using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDash : MonoBehaviour
{
    [SerializeField] private EnemyTypeConfig_Dasher _dashConfig;
    [SerializeField] private Enemy _enemy;
    [SerializeField] private PathFollower _pathFollower;
    [SerializeField] private EnemyDashView _view;

    private float _extraDashTime = 0f;
    
    
    private void OnEnable()
    {
        _enemy.OnSpeedBoostReceived += RestartDashLoopFromSpeedBoost;        
        DashLoop();
    }

    private void OnDisable()
    {
        _enemy.OnSpeedBoostReceived -= RestartDashLoopFromSpeedBoost;        
        StopAllCoroutines();
    }




    private void RestartDashLoopFromSpeedBoost()
    {
        _extraDashTime = _dashConfig.ExtraDashTimeWhenBoosted;
        //RestartDashLoop();
    }
    private void RestartDashLoop()
    {
        StopAllCoroutines();
        DashLoop();
    }
    private void DashLoop()
    {
        StartCoroutine(DashAfterReachingNodeT());
    }
    
    private IEnumerator DashAfterReachingNodeT()
    {
        while (true)
        {
            yield return StartCoroutine(MoveAndSetInvulnerable());
            _extraDashTime = 0f;
            yield return StartCoroutine(StopMovingAndSetVulnerable());
        }
    }

    private IEnumerator MoveAndSetInvulnerable()
    {
        //_enemy.CanBeTargetedFlag = false;
        _view.StartDashing();

        yield return StartCoroutine(WaitUntilTravelledDistance(_dashConfig.DashTravelDistance));
        yield return new WaitForSeconds(_extraDashTime);

        //_enemy.CanBeTargetedFlag = true;
        _view.StopDashing();
    }

    private IEnumerator StopMovingAndSetVulnerable()
    {
        StopMoving();
        yield return new WaitForSeconds(_dashConfig.StopDuration);
        ResetMoving();
    }
    
    private IEnumerator WaitUntilTravelledDistance(float distanceToTravel)
    {
        float startingTravelledDistance = _pathFollower.TravelledDistance;
        
        while (_pathFollower.TravelledDistance - startingTravelledDistance < distanceToTravel)
        {
            yield return null;
        }
    }
    
    
    private void StopMoving()
    {
        _pathFollower.UpdateBaseMoveSpeedDash(0f);
    }
    private void ResetMoving()
    {
        _pathFollower.UpdateBaseMoveSpeedDash(1f);
    }
    
    

}
