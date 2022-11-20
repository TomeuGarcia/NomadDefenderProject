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


    public delegate void TDGameManagerAction();
    public static event TDGameManagerAction OnVictoryComplete;
    public static event TDGameManagerAction OnGameOverStart;
    public static event TDGameManagerAction OnGameOverComplete;
    public static event TDGameManagerAction OnEndGameResetPools;



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

        if (OnGameOverStart != null) OnGameOverStart();
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
        yield return new WaitForSeconds(5f);

        if (OnEndGameResetPools != null) OnEndGameResetPools();
        if (OnGameOverComplete != null) OnGameOverComplete();
    }

    private IEnumerator VictoryAnimation()
    {
        victoryHolder.SetActive(true);
        yield return new WaitForSeconds(5f);

        if (OnEndGameResetPools != null) OnEndGameResetPools();
        if (OnVictoryComplete != null) OnVictoryComplete();
    }

}
