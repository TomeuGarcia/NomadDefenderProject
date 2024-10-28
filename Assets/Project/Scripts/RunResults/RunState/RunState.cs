using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    public int NodesReached { get; private set; }
    public int TotalBuildingsPlaced { get; private set; }
    public int TotalBuildingsUpgraded { get; private set; }
    public int TotalDamageDealt { get; private set; }
    public int HighestDamageDealt { get; private set; }
    public int TotalDamageTaken { get; private set; }
    public int DestroyedNodes { get; private set; }
    



    public void Init(CardDeckAsset starterDeck, CardDeckContent currentDeckContent)
    {
        _startTime = Time.time;
        _enemyTypeToDamageDealt = new Dictionary<EnemyTypeConfig, int>();
        
        StarterDeck = starterDeck;
        DeckContent = currentDeckContent;
        RunDuration = 0;
        NodesReached = 0;
        DestroyedNodes = 0;
        
        TotalBuildingsPlaced = 0;
        TotalBuildingsUpgraded = 0;

        TotalDamageDealt = 0;
        HighestDamageDealt = 0;
        TotalDamageTaken = 0;
    }
    

    public void Finish(bool victory)
    {
        Victory = victory;
        RunDuration = Time.time - _startTime;
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
    
    public void AddDamageDealt(int damageDealt)
    {
        TotalDamageDealt += damageDealt;
        HighestDamageDealt = Mathf.Max(HighestDamageDealt, damageDealt);
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

    
    
    public void DebugOverwriteWithRandomData()
    {
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
