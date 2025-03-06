using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeckSelectorHighscoreDisplayer : MonoBehaviour
{
    [System.Serializable]
    private class StatView
    {
        [SerializeField] private TextDecoder _statNameTextDecoder;
        [SerializeField] private TextDecoder _statCountTextDecoder;

        public void Init(int statCount)
        {
            _statCountTextDecoder.SetTextStrings(statCount.ToString("N0").Replace(',', '.'));
        }

        public IEnumerator Show()
        {
            _statNameTextDecoder.Activate();
            yield return new WaitUntil(() => _statNameTextDecoder.FinishedLine);
            
            _statCountTextDecoder.Activate();
            yield return new WaitUntil(() => _statCountTextDecoder.FinishedLine);
        }

        public void Hide()
        {
            _statNameTextDecoder.ClearAndStop();
            _statCountTextDecoder.ClearAndStop();
        }
    }


    [Header("GENERAL")] 
    [SerializeField] private GameProgressionStatus _gameProgressionStatus;
    [SerializeField] private TextDecoder _difficultyTextDecoder;
    [SerializeField] private GameObject _textsHolder;
    private DeckSelector _deckSelector;
    private DifficultyDisplay _difficultyDisplay;
    
    [Header("STATS")]
    [SerializeField] private StatView _totalDamageDealtStat;
    [SerializeField] private StatView _singleHitDamageStat;
    [SerializeField] private StatView _perfectDefenses;
    [SerializeField] private StatView _lostHealthStat;
    private StatView[] _allStats;

    [Header("SPECIAL CASES")] 
    [SerializeField] private CardDeckAsset _berserkerStarterDeck;


    public void Init(DeckSelector deckSelector, DifficultyDisplay difficultyDisplay)
    {
        _deckSelector = deckSelector;
        _difficultyDisplay = difficultyDisplay;
        
        _textsHolder.SetActive(false);
        _allStats = new[]
        {
            _totalDamageDealtStat, _singleHitDamageStat, _perfectDefenses, _lostHealthStat
        };
    }

    
    public void Show()
    {
        StopAllCoroutines();
        
        InitForShowing(
            _difficultyDisplay.CurrentDifficultyDisplay.DisplayText, 
            _deckSelector.CurrentlySelectedStarterDeck, 
            ServiceLocator.GetInstance().GameDifficultySettingsSource.CurrentGameDifficulty, 
            out List<StatView> statsToShow);
        
        StartCoroutine(DoShow(statsToShow));
    }
    
    
    private void InitForShowing(string difficultyText, CardDeckAsset starterDeck, GameDifficultyType gameDifficulty, 
        out List<StatView> statsToShow)
    {
        _difficultyTextDecoder.ClearAndStop();
        _difficultyTextDecoder.SetTextStrings(difficultyText);

        statsToShow = new List<StatView>(3)
        {
            _totalDamageDealtStat,
            _singleHitDamageStat,
        };


        DeckHighscore deckHighscore = _gameProgressionStatus.Game.StarterDecksSaveStatus.
            GetDeckSaveDataByName(starterDeck).GetHighscoreByDifficulty(gameDifficulty);
        
        _totalDamageDealtStat.Init(deckHighscore.totalDamageDealt);
        _singleHitDamageStat.Init(deckHighscore.highestDamageDealt);

        if (starterDeck.DeckName == _berserkerStarterDeck.DeckName)
        {
            statsToShow.Add(_lostHealthStat);
            _lostHealthStat.Init(deckHighscore.lostHealth);
        }
        else
        {
            statsToShow.Add(_perfectDefenses);
            _perfectDefenses.Init(deckHighscore.perfectDefenseCount);
        }
    }

    private IEnumerator DoShow(List<StatView> statsToShow)
    {        
        _textsHolder.SetActive(true);
        foreach (StatView stat in _allStats)
        {
            stat.Hide();
        }
        
        _difficultyTextDecoder.Activate();
        yield return new WaitUntil(() => _difficultyTextDecoder.FinishedLine);
        yield return new WaitForSeconds(0.1f);

        foreach (StatView statToShow in statsToShow)
        {
            yield return StartCoroutine(statToShow.Show());
            yield return new WaitForSeconds(0.1f);
        }
    }
    

}
