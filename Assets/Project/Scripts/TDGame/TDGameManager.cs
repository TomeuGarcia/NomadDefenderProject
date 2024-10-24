using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TDGameManager : MonoBehaviour
{
    [Header("SCENE MANAGEMENT")]
    [SerializeField] private MapSceneNotifier mapSceneNotifier;


    [Header("Canvas")]
    [SerializeField] private GameObject victoryHolder;
    [SerializeField] private Transform victoryTextTransform;
    [SerializeField] private GameObject defeatHolder;
    [SerializeField] private Transform defeatTextTransform;


    public delegate void TDGameManagerAction();
    public static event TDGameManagerAction OnGameFinishStart;
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
    private int numAliveLocations = 0;


    [SerializeField] private bool hasToSendBattleState = true;




    private void Awake()
    {
        victoryHolder.SetActive(false);
        defeatHolder.SetActive(false);

        if (OnQueryReferenceToBattleStateResult != null)
            OnQueryReferenceToBattleStateResult(out battleStateResult);

        numAliveLocations = pathLocations.Length;        

        InitLocationsVisuals();
    }    


    private void OnEnable()
    {
        EnemyWaveManager.OnAllWavesFinished += CheckVictory;

        for (int i = 0; i < pathLocations.Length; ++i)
        {
            pathLocations[i].OnDeath += DecreaseAliveLocationsAndCheckGameOver;
        }
    }

    private void OnDisable()
    {
        EnemyWaveManager.OnAllWavesFinished -= CheckVictory;

        for (int i = 0; i < pathLocations.Length; ++i)
        {
            pathLocations[i].OnDeath -= DecreaseAliveLocationsAndCheckGameOver;
        }
    }

    private void InitLocationsVisuals()
    {
        for (int i = 0; i < pathLocations.Length; ++i)
        {
            OWMap_Node owMapNode = battleStateResult.nodeResults[i].owMapNode;
            
            pathLocations[i].InitNodeVisuals(owMapNode.NodeIconTexture, owMapNode.BorderColor);
        }        
    }


    private bool HasAliveLocationsLeft()
    {
        return numAliveLocations > 0;
    }

    private void DecreaseAliveLocationsAndCheckGameOver()
    {
        --numAliveLocations;
        if (!HasAliveLocationsLeft())
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log("GameOver");
        SetBattleStateResult();

        StartCoroutine(GameOverAnimation());

        if (OnGameOverStart != null) OnGameOverStart();
        if (OnGameFinishStart != null) OnGameFinishStart();
    }

    private void CheckVictory()
    {
        if (HasAliveLocationsLeft())
        {
            Victory();
        }
    }
    private void Victory()
    {
        Debug.Log("Victory");
        SetBattleStateResult();

        StartCoroutine(VictoryAnimation());
        if (OnGameFinishStart != null) OnGameFinishStart();
    }


    private IEnumerator GameOverAnimation()
    {
        defeatHolder.SetActive(true);
        yield return new WaitForSeconds(5f);

        if (OnEndGameResetPools != null) OnEndGameResetPools();

        mapSceneNotifier.InvokeOnSceneFinished();        
    }

    private IEnumerator VictoryAnimation()
    {
        victoryHolder.SetActive(true);
        yield return new WaitForSeconds(5f);

        if (OnEndGameResetPools != null) OnEndGameResetPools();

        mapSceneNotifier.InvokeOnSceneFinished();        
    }

    public void ForceFinishScene()
    {
        if (OnEndGameResetPools != null) OnEndGameResetPools();

        mapSceneNotifier.InvokeOnSceneFinished();
    }


    private void SetBattleStateResult()
    {
        if (!hasToSendBattleState) { return; }

        for (int i = 0; i < battleStateResult.nodeResults.Length; ++i)
        {
            battleStateResult.nodeResults[i].healthState = ComputeHealthState(pathLocations[i].healthSystem);
        }
    }

    public static NodeEnums.HealthState ComputeHealthState(HealthSystem healthSystem)
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
