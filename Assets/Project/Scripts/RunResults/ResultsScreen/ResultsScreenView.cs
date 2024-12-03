using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class ResultsScreenView : MonoBehaviour
{
    public class InitData
    {
        public Camera Camera { get; }
        public GameObject MostKillsCard { get; }
        public GameObject MostDamageCard { get; }
        public bool MostKillsAndDamageAreTheSame { get; }
        public GameObject MostDamagingEnemy { get; }
        public bool ExistsMostDamagingEnemy { get; }

        public InitData(Camera camera,
            GameObject mostKillsCard, GameObject mostDamageCard, bool mostKillsAndDamageAreTheSame,
            GameObject mostDamagingEnemy, bool existsMostDamagingEnemy)
        {
            Camera = camera;
            MostKillsCard = mostKillsCard;
            MostDamageCard = mostDamageCard;
            MostKillsAndDamageAreTheSame = mostKillsAndDamageAreTheSame;
            MostDamagingEnemy = mostDamagingEnemy;
            ExistsMostDamagingEnemy = existsMostDamagingEnemy;
        }
    }
    
    
    [System.Serializable]
    private class Stats
    {
        [SerializeField] private RectTransform _statsParent;
        
        private ResultScreenStat _simulationTime;
        private ResultScreenStat _nodesReached;
        private ResultScreenStat _buildingsPlaced;
        private ResultScreenStat _buildingsUpgraded;
        private ResultScreenStat _totalDamageDealt;
        private ResultScreenStat _highestDamageDealt;
        private ResultScreenStat _totalDamageTaken;
        private ResultScreenStat _totalDestroyedNodes;

        public void Init(IRunStateData runStateData, ResultScreenStat statPrefab, RectTransform statSeparatorPrefab)
        {
            InstantiateStat("simulationTime", runStateData.RunDurationAsString(), statPrefab, out _simulationTime);
            InstantiateStat("nodesReached", runStateData.NodesReached.ToString(), statPrefab, out _nodesReached);
            Instantiate(statSeparatorPrefab, _statsParent);
            InstantiateStat("buildingsPlaced", runStateData.TotalBuildingsPlaced.ToString(), statPrefab, out _buildingsPlaced);
            InstantiateStat("buildingsUpgraded", runStateData.TotalBuildingsUpgraded.ToString(), statPrefab, out _buildingsUpgraded);
            Instantiate(statSeparatorPrefab, _statsParent);
            InstantiateStat("totalDamageDealt", runStateData.TotalDamageDealt.ToString("N0"), statPrefab, out _totalDamageDealt);
            InstantiateStat("highestDamageDealt", runStateData.HighestDamageDealt.ToString("N0"), statPrefab, out _highestDamageDealt);
            Instantiate(statSeparatorPrefab, _statsParent);
            InstantiateStat("totalDamageTaken", runStateData.TotalDamageTaken.ToString(), statPrefab, out _totalDamageTaken);
            InstantiateStat("totalDestroyedNodes", runStateData.DestroyedNodes.ToString(), statPrefab, out _totalDestroyedNodes);
        }

        private void InstantiateStat(string statName, string statValue, ResultScreenStat statPrefab, out ResultScreenStat stat)
        {
            const string namePrefix = "> ";
            stat = Instantiate(statPrefab, _statsParent);
            stat.Init(namePrefix + statName, statValue.Replace(',', '.'));
            stat.gameObject.name = statPrefab.gameObject.name + "_" + statName;
        }

        public IEnumerator PlayAnimations(MonoBehaviour coroutinesParent)
        {
            yield return coroutinesParent.StartCoroutine(_simulationTime.PlayAnimation());
            yield return coroutinesParent.StartCoroutine(_nodesReached.PlayAnimation());
            yield return coroutinesParent.StartCoroutine(_buildingsPlaced.PlayAnimation());
            yield return coroutinesParent.StartCoroutine(_buildingsUpgraded.PlayAnimation());
            yield return coroutinesParent.StartCoroutine(_totalDamageDealt.PlayAnimation());
            yield return coroutinesParent.StartCoroutine(_highestDamageDealt.PlayAnimation());
            yield return coroutinesParent.StartCoroutine(_totalDamageTaken.PlayAnimation());
            yield return coroutinesParent.StartCoroutine(_totalDestroyedNodes.PlayAnimation());
        }
    }

    
    [Header("TITLE")]
    [SerializeField] private TextDecoder _victoryTitle;
    [SerializeField] private TextDecoder _defeatTitle;
    [SerializeField] private TextDecoder _resultsSubtitle;

    [Header("TITLE FADES")]
    [SerializeField] private RectTransform _fadeVictory;
    [SerializeField] private RectTransform _scrollVictory;
    [SerializeField] private RectTransform _fadeDefeat;
    [SerializeField] private RectTransform _scrollDefeat;
    [SerializeField] private float[] _fadePopping;
    [SerializeField] private ResultScreenMaskIntro _maskIntro; 

    [Header("STATS")] 
    [SerializeField] private ResultScreenStat _statPrefab;
    [SerializeField] private RectTransform _statSeparatorPrefab;
    [SerializeField] private TextDecoder _statsHeader;
    [SerializeField] private Stats _stats;

    [Header("DECK")]
    [SerializeField] private TextDecoder _deckHeader;
    [SerializeField] private TextDecoder _deckNameSubheader;
    [SerializeField] private ResultsScreenObjectPreviewer _mostDamageCardScreenPreviewer;
    [SerializeField] private ResultsScreenObjectPreviewer _mostKillsCardScreenPreviewer;
    [SerializeField] private ResultsScreenObjectPreviewer _mostKillsAndDamageCardScreenPreviewer;

    [Header("ENEMY")]
    [SerializeField] private TextDecoder _enemiesHeader;
    [SerializeField] private ResultsScreenObjectPreviewer _mostDamagingEnemyScreenPreviewer;
    [SerializeField] private TextDecoder _noDamageEnemyText;

    [Header("CONTINUE BUTTON")] 
    [SerializeField] private TextDecoder _continueButtonText;
    [SerializeField] private GameObject _continueTextArrows;
    private Button _continueButton;

    [Header("RESULT SPECIFIC")]
    [SerializeField] private GameObject _mapVictoryObjects;
    [SerializeField] private GameObject _mapDefeatObjects;



    private InitData _initData;
    

    public void Init(IRunStateData runStateData, InitData initData, Button continueButton)
    {
        _initData = initData;
        _continueButton = continueButton;
        _stats.Init(runStateData, _statPrefab, _statSeparatorPrefab);

        ResultChanges(runStateData);

        SetupShowAnimation(runStateData, initData);
    }

    private void ResultChanges(IRunStateData runStateData)
    {
        _mapVictoryObjects.SetActive(runStateData.Victory);
        _mapDefeatObjects.SetActive(!runStateData.Victory);
    }


    private void SetupShowAnimation(IRunStateData runStateData, InitData initData)
    {
        bool playingVictory = runStateData.Victory;
        _victoryTitle.gameObject.SetActive(playingVictory);
        _defeatTitle.gameObject.SetActive(!playingVictory);

        if (initData.MostKillsAndDamageAreTheSame)
        {
            _mostKillsCardScreenPreviewer.InitToNotShow();
            _mostDamageCardScreenPreviewer.InitToNotShow();
            _mostKillsAndDamageCardScreenPreviewer.InitToShow(initData.Camera, initData.MostKillsCard);
        }
        else
        {
            _mostKillsCardScreenPreviewer.InitToShow(initData.Camera, initData.MostKillsCard);
            _mostDamageCardScreenPreviewer.InitToShow(initData.Camera, initData.MostDamageCard);
            InitCard(initData.MostKillsCard);
            InitCard(initData.MostDamageCard);
        }
        
        _mostDamagingEnemyScreenPreviewer.InitToShow(initData.Camera, initData.MostDamagingEnemy);

        _deckNameSubheader.SetTextStrings(runStateData.StarterDeck.DeckName + " starter deck");
        
        _continueButton.interactable = false;

        //Fades
        _fadeVictory.gameObject.SetActive(false);
        _fadeDefeat.gameObject.SetActive(false);

        _continueTextArrows.gameObject.SetActive(false);
    }

    private void InitCard(GameObject card)
    {
        Vector3 cardPosition = card.transform.position;
        card.GetComponent<BuildingCard>().InitPositions(cardPosition, Vector3.zero, cardPosition);
    }
    
    
    public void StartPlayingShowAnimation(IRunStateData runStateData)
    {
        StartCoroutine(PlayShowAnimation(runStateData));
    }
    
    private IEnumerator PlayShowAnimation(IRunStateData runStateData)
    {
        yield return new WaitForSeconds(1f);
        if(runStateData.Victory)
        {
            yield return StartCoroutine(ShowTitleFades(_fadeVictory, _scrollVictory));
        }
        else
        {
            yield return StartCoroutine(ShowTitleFades(_fadeDefeat, _scrollDefeat));
        }
        yield return StartCoroutine(PlayShowTitleAnimation(runStateData));
        yield return StartCoroutine(PlayShowStatsAnimation());
        yield return StartCoroutine(PlayShowDeckAnimation());
        yield return StartCoroutine(PlayShowEnemiesAnimation());
        yield return StartCoroutine(PlayShowContinueButton());
    }

    private IEnumerator PlayShowTitleAnimation(IRunStateData runStateData)
    {
        bool playingVictory = runStateData.Victory;
        TextDecoder titleDecoder = playingVictory ? _victoryTitle : _defeatTitle;
        yield return StartCoroutine(PlayTextDecoder(titleDecoder));
        yield return StartCoroutine(PlayTextDecoder(_resultsSubtitle));
    }

    private IEnumerator PlayShowStatsAnimation()
    {
        yield return StartCoroutine(PlayTextDecoder(_statsHeader));
        yield return StartCoroutine(_stats.PlayAnimations(this));
    }
    
    private IEnumerator PlayShowDeckAnimation()
    {
        yield return StartCoroutine(PlayTextDecoder(_deckHeader));
        yield return StartCoroutine(PlayTextDecoder(_deckNameSubheader));

        if (_initData.MostKillsAndDamageAreTheSame)
        {
            yield return StartCoroutine(_mostKillsAndDamageCardScreenPreviewer.PlayShowAnimation()); 
        }
        else
        {
            yield return StartCoroutine(_mostKillsCardScreenPreviewer.PlayShowAnimation());
            yield return StartCoroutine(_mostDamageCardScreenPreviewer.PlayShowAnimation());
        }
    }

    private IEnumerator PlayShowEnemiesAnimation()
    {
        yield return StartCoroutine(PlayTextDecoder(_enemiesHeader));
        if (!_initData.ExistsMostDamagingEnemy)
        {
            yield return StartCoroutine(PlayTextDecoder(_noDamageEnemyText));
        }
        yield return StartCoroutine(_mostDamagingEnemyScreenPreviewer.PlayShowAnimation()); 
        _initData.MostDamagingEnemy.GetComponent<Enemy>().InitWithoutFunctionality();
    }

    private IEnumerator PlayShowContinueButton()
    {
        yield return StartCoroutine(PlayTextDecoder(_continueButtonText));
        _continueTextArrows.SetActive(true);
        _continueButton.interactable = true;
    }
    

    private IEnumerator PlayTextDecoder(TextDecoder textDecoder)
    {
        textDecoder.Activate();
        yield return new WaitUntil(() => textDecoder.FinishedLine);
    }

    private IEnumerator ShowTitleFades(RectTransform fade, RectTransform scrollFadeParent)
    {
        foreach (float f in _fadePopping)
        {
            fade.gameObject.SetActive(!fade.gameObject.activeInHierarchy);
            yield return new WaitForSeconds(f);
        }

        _maskIntro.StartScroll(scrollFadeParent);
    }
}
