using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper;
using UnityEngine;

public class ResultsScreen : MonoBehaviour
{
    [Header("RUN STATE")]
    [SerializeField] private InterfaceReference<IRunStateData, ScriptableObject> _runStateData;
    private IRunStateData RunStateData => _runStateData.Value;

    
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

    [Header("STATS")] 
    [SerializeField] private ResultScreenStat _statPrefab;
    [SerializeField] private RectTransform _statSeparatorPrefab;
    [SerializeField] private TextDecoder _statsHeader;
    [SerializeField] private Stats _stats;

    [Header("DECK")]
    [SerializeField] private TextDecoder _deckHeader;
    [SerializeField] private TextDecoder _deckNameSubheader;
    [SerializeField] private TextDecoder _mostKillsCardText;
    [SerializeField] private TextDecoder _mostDamageCardText;
    [SerializeField] private GameObject _cardsHolder;
    [SerializeField] private TextDecoder _mostDangerousEnemyText;
    [SerializeField] private GameObject _enemyHolder;
    
    
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        CheckAchievements();
        _stats.Init(RunStateData, _statPrefab, _statSeparatorPrefab);

        StartCoroutine(PlayShowAnimation());
    }


    private void CheckAchievements()
    {
        AchievementDefinitions.VictoryWithoutTakingDamage.Check(RunStateData.TotalDamageTaken);
        AchievementDefinitions.VictoryWithoutUpgradingBuildings.Check(RunStateData.TotalBuildingsUpgraded);
    }
    
    private IEnumerator PlayShowAnimation()
    {
        SetupShowAnimation();

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(PlayShowTitleAnimation());
        yield return StartCoroutine(PlayShowStatsAnimation());
        yield return StartCoroutine(PlayShowDeckAnimation());
    }
    
    private void SetupShowAnimation()
    {
        _cardsHolder.SetActive(false);
        _enemyHolder.SetActive(false);
        
        _deckNameSubheader.SetTextStrings(RunStateData.StarterDeck.DeckName + " starter deck");
    }

    private IEnumerator PlayShowTitleAnimation()
    {
        bool playingVictory = RunStateData.Victory;
        _victoryTitle.gameObject.SetActive(playingVictory);
        _defeatTitle.gameObject.SetActive(!playingVictory);
            
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

        _cardsHolder.SetActive(true);
        yield return new WaitForSeconds(0.15f);
        yield return StartCoroutine(PlayTextDecoder(_mostKillsCardText));
        yield return StartCoroutine(PlayTextDecoder(_mostDamageCardText));
        yield return new WaitForSeconds(0.15f);
        
        
        _enemyHolder.SetActive(true);
        yield return new WaitForSeconds(0.15f);
        yield return StartCoroutine(PlayTextDecoder(_mostDangerousEnemyText));
        yield return new WaitForSeconds(0.15f);
    }


    private IEnumerator PlayTextDecoder(TextDecoder textDecoder)
    {
        textDecoder.Activate();
        yield return new WaitUntil(() => textDecoder.FinishedLine);
    }
}
