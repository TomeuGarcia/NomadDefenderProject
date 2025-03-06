using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


[CreateAssetMenu(fileName = "RunState", 
    menuName = SOAssetPaths.GAME_RUN + "RunState")]
public class RunState : ScriptableObject, 
    IRunStateInitialization, IRunStateData, IRunStateUpdate
{
    private float _startTime;
    private Dictionary<EnemyTypeConfig, int> _enemyTypeToDamageDealt;

    public bool Victory { get; private set; }
    public CardDeckAsset StarterDeck { get; private set; }
    public CardDeckContent DeckContent { get; private set; }
    public float RunDuration { get; private set; }
    public string RunDurationAsString()
    {
        int remainingSeconds = Mathf.FloorToInt(RunDuration);
        const int secondsPerMinute = 60;
        const int minutesPerHour = 60;
        const int secondsPerHour = secondsPerMinute * minutesPerHour;
        
        int hours = remainingSeconds / secondsPerHour;
        remainingSeconds -= hours * secondsPerHour;

        int minutes = remainingSeconds / minutesPerHour;
        remainingSeconds -= minutes * minutesPerHour;

        int seconds = remainingSeconds;
        

        string duration = hours.ToString("00") + ':' +
                          minutes.ToString("00") + ':' +
                          seconds.ToString("00");

        return duration;
    }

    public int BattleVictories { get; private set; }
    public int PerfectDefenseBattleVictories { get; private set; }
    public int NodesReached { get; private set; }
    public int TotalBuildingsPlaced { get; private set; }
    public int TotalBuildingsUpgraded { get; private set; }
    public int TotalDamageDealt { get; private set; }
    public int HighestDamageDealt { get; private set; }
    public int TotalDamageTaken { get; private set; }
    public bool MostDamagingEnemy(out EnemyTypeConfig enemyType, out int damage)
    {
        enemyType = null;
        damage = 0;
        foreach (KeyValuePair<EnemyTypeConfig,int> enemyTypeToDamageDealt in _enemyTypeToDamageDealt)
        {
            if (enemyTypeToDamageDealt.Value > damage)
            {
                damage = enemyTypeToDamageDealt.Value;
                enemyType = enemyTypeToDamageDealt.Key;
            }
        }

        return enemyType != null;
    }


    public bool UnlockDifficulty { get; private set; }
    public bool UnlockStarterDeck { get; private set; }
    public bool HasPendingUnlocks => UnlockDifficulty || UnlockStarterDeck;
    public DeckHighscore MakeDeckHighscore()
    {
        return new DeckHighscore(TotalDamageDealt, HighestDamageDealt, PerfectDefenseBattleVictories, TotalDamageTaken);
    }

    public int DestroyedNodes { get; private set; }
    



    public void Init(CardDeckAsset starterDeck, CardDeckContent currentDeckContent)
    {
        _startTime = Time.time;
        _enemyTypeToDamageDealt = new Dictionary<EnemyTypeConfig, int>();
        
        StarterDeck = starterDeck;
        DeckContent = currentDeckContent;

        BattleVictories = 0;
        PerfectDefenseBattleVictories = 0;
        
        RunDuration = 0;
        NodesReached = 0;
        DestroyedNodes = 0;
        
        TotalBuildingsPlaced = 0;
        TotalBuildingsUpgraded = 0;

        TotalDamageDealt = 0;
        HighestDamageDealt = 0;
        TotalDamageTaken = 0;
    }
    

    public void Finish(bool victory, bool unlockDifficulty, bool unlockStarterDeck)
    {
        Victory = victory;
        RunDuration = Time.time - _startTime;
        
        UnlockDifficulty = unlockDifficulty;
        UnlockStarterDeck = unlockStarterDeck;
    }



    public void IncrementBattleVictories(bool perfectDefense)
    {
        ++BattleVictories;
        if (TotalDamageTaken <= 0)
        {
            ++PerfectDefenseBattleVictories;
        }
    }

    public void IncrementNodesReached()
    {
        ++NodesReached;
    }

    public void IncrementDestroyedNodes()
    {
        ++DestroyedNodes;
    }

    public void IncrementPlacedBuildings()
    {
        ++TotalBuildingsPlaced;
    }

    public void IncrementUpgradedBuildings()
    {
        ++TotalBuildingsUpgraded;
    }
    
    public void AddDamageDealt(TurretDamageAttack damageAttackDealt)
    {
        TotalDamageDealt += damageAttackDealt.Damage;
        HighestDamageDealt = Mathf.Max(HighestDamageDealt, damageAttackDealt.Damage);
    }

    public void AddDamageTaken(int damageTaken, EnemyTypeConfig attacker)
    {
        TotalDamageTaken += damageTaken;

        if (_enemyTypeToDamageDealt.ContainsKey(attacker))
        {
            _enemyTypeToDamageDealt[attacker] += damageTaken;
        }
        else
        {
            _enemyTypeToDamageDealt.Add(attacker, damageTaken);   
        }
        
    }


    public void DebugOverwriteWithRandomData(bool victory,
        CardDeckAsset starterDeck, CardDeckContent currentDeckContent, EnemyTypeConfig mostDamagingEnemy)
    {
        Victory = victory;
        StarterDeck = starterDeck;
        DeckContent = currentDeckContent;

        if (mostDamagingEnemy != null)
        {
            _enemyTypeToDamageDealt.Add(mostDamagingEnemy, 1);
        }
        
        RunDuration = Random.Range(1000, 3600);
        NodesReached = Random.Range(1, 15);
    
        TotalBuildingsPlaced = Random.Range(30, 60);
        TotalBuildingsUpgraded = Random.Range(10, 30);

        TotalDamageDealt = Random.Range(10000, 1000000);
        HighestDamageDealt = Random.Range(100, 1000);
    
        DestroyedNodes = Random.Range(1, 10);
        TotalDamageTaken = (DestroyedNodes / 2) * Random.Range(1, 5);
    }

}
