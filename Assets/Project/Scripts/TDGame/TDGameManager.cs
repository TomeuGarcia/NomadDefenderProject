using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TDGameManager : MonoBehaviour
{
    [SerializeField] private PathLocation basePathLocation;

    [Header("Canvas")]
    [SerializeField] private GameObject victoryHolder;
    [SerializeField] private Transform victoryTextTransform;
    [SerializeField] private GameObject defeatHolder;
    [SerializeField] private Transform defeatTextTransform;


    private void Awake()
    {
        victoryHolder.SetActive(false);
        defeatHolder.SetActive(false);
    }


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
        StartCoroutine(GameOverAnimation());
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
        StartCoroutine(VictoryAnimation());
    }


    private IEnumerator GameOverAnimation()
    {
        defeatHolder.SetActive(true);
        yield return null;
    }

    private IEnumerator VictoryAnimation()
    {
        victoryHolder.SetActive(true);
        yield return null;
    }

}
