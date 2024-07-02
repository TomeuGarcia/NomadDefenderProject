using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TDGameManager : MonoBehaviour, TDLocationsUtils
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
    private bool alreadyPlayedVictoryOrGameOver = false;

    [Header("TILES MATERIAL")]
    [SerializeField] private Material obstaclesTilesMaterial;
    [SerializeField] private Material tilesMaterial;
    [SerializeField] private Material outerPlanesMaterial;


    private void Awake()
    {
        ServiceLocator.GetInstance().TDLocationsUtils = this;

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

        tilesMaterial.SetFloat("_ErrorWiresStep", 0f);
        tilesMaterial.SetFloat("_AdditionalErrorWiresStep2", 0f);
        tilesMaterial.SetVector("_ErrorOriginOffset", Vector3.one * -1000);
        tilesMaterial.SetVector("_ErrorOriginOffset2", Vector3.one * -1000);

        obstaclesTilesMaterial.SetFloat("_ErrorWiresStep", 0f);
        obstaclesTilesMaterial.SetFloat("_AdditionalErrorWiresStep2", 0f);
        obstaclesTilesMaterial.SetVector("_ErrorOriginOffset", Vector3.one * -1000);
        obstaclesTilesMaterial.SetVector("_ErrorOriginOffset2", Vector3.one * -1000);

        outerPlanesMaterial.SetFloat("_ErrorWiresStep", 0f);
        outerPlanesMaterial.SetFloat("_AdditionalErrorWiresStep2", 0f);
        outerPlanesMaterial.SetVector("_ErrorOriginOffset", Vector3.one * -1000);
        outerPlanesMaterial.SetVector("_ErrorOriginOffset2", Vector3.one * -1000);
    }


    private bool HasAliveLocationsLeft()
    {
        return numAliveLocations > 0;
    }

    private void DecreaseAliveLocationsAndCheckGameOver(PathLocation destroyedPathLocation)
    {
        if (!HasAliveLocationsLeft()) return;

        --numAliveLocations;
        if (!HasAliveLocationsLeft())
        {
            obstaclesTilesMaterial.SetVector("_ErrorOriginOffset", destroyedPathLocation.transform.position);
            tilesMaterial.SetVector("_ErrorOriginOffset", destroyedPathLocation.transform.position);
            outerPlanesMaterial.SetVector("_ErrorOriginOffset", destroyedPathLocation.transform.position);

            LastEnemyKIllAnimation.instance.DeathAnimation(destroyedPathLocation.transform.position, true);
            GameOver();
        }
        else
        {
            obstaclesTilesMaterial.SetVector("_ErrorOriginOffset2", destroyedPathLocation.transform.position);
            //tilesMaterial.SetVector("_ErrorOriginOffset2", destroyedPathLocation.transform.position);
            outerPlanesMaterial.SetVector("_ErrorOriginOffset2", destroyedPathLocation.transform.position);
            StartCoroutine(FirstLocationDestroyedAnimation());

            //destroyAnim
            LastEnemyKIllAnimation.instance.DeathAnimation(destroyedPathLocation.transform.position, false);
        }
    }

    private void GameOver()
    {
        if (alreadyPlayedVictoryOrGameOver) return;

        alreadyPlayedVictoryOrGameOver = true;
        Debug.Log("GameOver");

        SetBattleStateResult();

        StartCoroutine(GameOverAnimation());
        Sequence audioSequence = DOTween.Sequence();
        audioSequence.AppendInterval(0.35f);
        audioSequence.AppendCallback(() => GameAudioManager.GetInstance().PlayWiresCursedWave());


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
        if (alreadyPlayedVictoryOrGameOver) return;

        alreadyPlayedVictoryOrGameOver = true;
        Debug.Log("Victory");

        StartCoroutine(VictoryAnimation());
        if (OnGameFinishStart != null) OnGameFinishStart();
    }

    private IEnumerator FirstLocationDestroyedAnimation()
    {
        for (float t = 0f; t < 2.3f; t += Time.deltaTime)
        {
            float errorWiresStep = (t * t * 0.3f) + 1.0f;
            obstaclesTilesMaterial.SetFloat("_AdditionalErrorWireStep2", errorWiresStep);
            tilesMaterial.SetFloat("_AdditionalErrorWireStep2", errorWiresStep);
            outerPlanesMaterial.SetFloat("_AdditionalErrorWireStep2", errorWiresStep);
            yield return null;
        }
    }

    private IEnumerator GameOverAnimation()
    {
        //defeatHolder.SetActive(true);
        PauseMenu.GetInstance().gameCanBePaused = false;

        //yield return new WaitForSeconds(5f);
        for (float t = 1f; t < 5f; t += Time.deltaTime)
        {
            float errorWiresStep = t * t * 1.0f;
            obstaclesTilesMaterial.SetFloat("_ErrorWiresStep", errorWiresStep);
            tilesMaterial.SetFloat("_ErrorWiresStep", errorWiresStep);
            outerPlanesMaterial.SetFloat("_ErrorWiresStep", errorWiresStep);
            yield return null;
        }


        if (OnEndGameResetPools != null) OnEndGameResetPools();

        mapSceneNotifier.InvokeOnSceneFinished();
        
        GameAudioManager.GetInstance().ChangeMusic(GameAudioManager.MusicType.OWMAP, 1f);
    }

    private IEnumerator VictoryAnimation()
    {
        //victoryHolder.SetActive(true);

        GameAudioManager.GetInstance().PlayBattleStageVictory();
        //yield return new WaitForSeconds(1f);

        yield return new WaitForSeconds(5f);
        
        SetBattleStateResult();
        if (OnEndGameResetPools != null) OnEndGameResetPools();

        mapSceneNotifier.InvokeOnSceneFinished();
        
        GameAudioManager.GetInstance().ChangeMusic(GameAudioManager.MusicType.OWMAP, 1f);
    }

    public void ForceFinishScene()
    {
        if (OnEndGameResetPools != null) OnEndGameResetPools();

        mapSceneNotifier.InvokeOnSceneFinished();
        SetBattleStateResult();
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



    public int GetHighestLocationHealth()
    {
        int highestHealth = -1;

        for (int i = 0; i < pathLocations.Length; ++i)
        {
            int loactionHealth = pathLocations[i].healthSystem.health;
            if (loactionHealth > highestHealth)
            {
                highestHealth = loactionHealth;
            }
        }

        return highestHealth;
    }

    public PathLocation GetHealthiestLocation(Vector3 quearierPosition)
    {
        int highestHealth = -1;
        float closestDistance = 1000;
        float distanceThreshold = 0.5f;
        PathLocation healthiestLocation = null;

        for (int i = 0; i < pathLocations.Length; ++i)
        {
            if (pathLocations[i].healthSystem.IsDead()) continue;


            float distanceToQuearier = Vector3.Distance(quearierPosition, pathLocations[i].Position) + distanceThreshold;
            bool isCloser = distanceToQuearier < closestDistance;
            if (isCloser)
            {
                closestDistance = distanceToQuearier;
            }


            int loactionHealth = pathLocations[i].healthSystem.health;
            if (loactionHealth > highestHealth || (loactionHealth == highestHealth && isCloser))
            {
                highestHealth = loactionHealth;
                healthiestLocation = pathLocations[i];
            }
        }

        return healthiestLocation;
    }

    public PathLocation GetMostDamagedLocation(Vector3 quearierPosition)
    {
        int lowestHealth = 1000;
        float closestDistance = 1000;
        float distanceThreshold = 0.5f;
        PathLocation mostDamagedLocation = null;

        for (int i = 0; i < pathLocations.Length; ++i)
        {
            if (pathLocations[i].healthSystem.IsDead()) continue;


            float distanceToQuearier = Vector3.Distance(quearierPosition, pathLocations[i].Position) + distanceThreshold;
            bool isCloser = distanceToQuearier < closestDistance;
            if (isCloser)
            {
                closestDistance = distanceToQuearier;
            }


            int loactionHealth = pathLocations[i].healthSystem.health;
            if (loactionHealth < lowestHealth || (loactionHealth == lowestHealth && isCloser))
            {
                lowestHealth = loactionHealth;
                mostDamagedLocation = pathLocations[i];
            }
        }

        return mostDamagedLocation;
    }
}
