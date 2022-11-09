using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDGameManager : MonoBehaviour
{
    [SerializeField] private PathLocation basePathLocation;


    private void OnEnable()
    {
        basePathLocation.OnDeath += GameOver;
        EnemyWaveManager.OnAllWavesFinished += CheckVictory;
    }

    private void OnDisable()
    {
        basePathLocation.OnDeath -= GameOver;
        EnemyWaveManager.OnAllWavesFinished -= CheckVictory;
    }

    private void GameOver()
    {
        Debug.Log("GameOver");
    }

    private void CheckVictory()
    {
        if (!basePathLocation.healthSystem.IsDead())
        {
            Victory();
        }
    }
    private void Victory()
    {
        Debug.Log("Victory");
    }


}
