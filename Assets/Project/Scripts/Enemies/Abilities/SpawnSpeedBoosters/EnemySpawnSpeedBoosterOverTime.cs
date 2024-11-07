using System;
using System.Collections;
using UnityEngine;

public class EnemySpawnSpeedBoosterOverTime : MonoBehaviour
{
    [SerializeField] private EnemyTypeConfig_SpeedBoosterSpawner _config;
    [SerializeField] private Enemy _enemy;
    [SerializeField] private PathFollower _pathFollower;

    private void OnEnable()
    {
        StartCoroutine(StartSpawning());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator StartSpawning()
    {
        yield return new WaitForSeconds(_config.StartSpawningWaitDuration);
        while (true)
        {
            SpawnSpeedBooster();
            yield return StartCoroutine(WaitUntilTravelledDistance(_config.SpawnTravelDistance));
        }
    }
    
    
    private IEnumerator WaitUntilTravelledDistance(float distanceToTravel)
    {
        float startingTravelledDistance = _pathFollower.TravelledDistance;
        
        while (_pathFollower.TravelledDistance - startingTravelledDistance < distanceToTravel)
        {
            yield return null;
        }
    }

    private void SpawnSpeedBooster()
    {
        Vector3 speedBoosterPosition = _pathFollower.PositionBetweenNodes + new Vector3(0,0.16f,0);
        Quaternion speedBoosterRotation = Quaternion.LookRotation(_pathFollower.CurrentNode.GetDirectionToNextNode(), Vector3.up);

        SpeedBooster speedBooster = SpeedBoosterFactory.Instance.Create(
            _config.SpeedBoosterConfig, speedBoosterPosition, speedBoosterRotation);
        
        speedBooster.AddUserToIgnore(_enemy);
    }
    
}