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
    
    
    private void OnEnable()
    {
        StartCoroutine(DashAfterReachingNodeT());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }


    private IEnumerator DashAfterReachingNodeT()
    {
        while (true)
        {
            yield return StartCoroutine(MoveAndSetInvulnerable());
            yield return StartCoroutine(StopMovingAndSetVulnerable());
        }
    }

    private IEnumerator MoveAndSetInvulnerable()
    {
        //_enemy.CanBeTargetedFlag = false;
        _view.StartDashing();

        yield return StartCoroutine(WaitUntilTravelledDistance(_dashConfig.DashTravelDistance));
        
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
        _pathFollower.UpdateBaseMoveSpeed(0);
    }
    private void ResetMoving()
    {
        _pathFollower.UpdateBaseMoveSpeed(_dashConfig.BaseStats.MoveSpeed);
    }
    
    

}
