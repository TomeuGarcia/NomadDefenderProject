using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TDGameManager : MonoBehaviour
{
    [Header("SCENE MANAGEMENT")]
    [SerializeField] private MapSceneNotifier mapSceneNotifier;

    [Header("Base Path Location")]
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

    public delegate void TD_GM_BattleStateAction(out BattleStateResult battleStateResult);
    public static event TD_GM_BattleStateAction OnQueryReferenceToBattleStateResult;


    // BattleStateResult
    private BattleStateResult battleStateResult;
    [Header("PATH LOCATIONS")]
    [SerializeField] private PathLocation[] pathLocations;




    private void Awake()
    {
        victoryHolder.SetActive(false);
        defeatHolder.SetActive(false);

        if (OnQueryReferenceToBattleStateResult != null)
            OnQueryReferenceToBattleStateResult(out battleStateResult);
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
        SetBattleStateResult();

        StartCoroutine(GameOverAnimation());

        basePathLocation.OnDeath -= GameOver;

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
        SetBattleStateResult();

        StartCoroutine(VictoryAnimation());
    }


    private IEnumerator GameOverAnimation()
    {
        defeatHolder.SetActive(true);
        yield return new WaitForSeconds(5f);

        if (OnEndGameResetPools != null) OnEndGameResetPools();

        mapSceneNotifier.InvokeOnSceneFinished();
        //if (OnGameOverComplete != null) OnGameOverComplete();
    }

    private IEnumerator VictoryAnimation()
    {
        victoryHolder.SetActive(true);
        yield return new WaitForSeconds(5f);

        if (OnEndGameResetPools != null) OnEndGameResetPools();

        mapSceneNotifier.InvokeOnSceneFinished();
        //if (OnVictoryComplete != null) OnVictoryComplete(); // 
    }


    private void SetBattleStateResult()
    {
        for (int i = 0; i < battleStateResult.nodeResults.Length; ++i)
        {
            battleStateResult.nodeResults[i].healthState = ComputeHealthState(pathLocations[i].healthSystem);
        }
    }

    public NodeEnums.HealthState ComputeHealthState(HealthSystem healthSystem)
    {
        if (healthSystem.IsDead())
            return NodeEnums.HealthState.DESTROYED;

        if (healthSystem.IsFullHealth())
            return NodeEnums.HealthState.UNDAMAGED;


        float healthRatio = healthSystem.HealthRatio;
        if (healthRatio > 0.5f)
            return NodeEnums.HealthState.SLIGHTLY_DAMAGED;
        else
            return NodeEnums.HealthState.GREATLY_DAMAGED;
    }

}
