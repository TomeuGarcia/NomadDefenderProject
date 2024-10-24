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

    public CardDeckInUseData DeckInUse { get; private set; }
    public float RunDuration { get; private set; }
    public int NodesReached { get; private set; }
    public int TotalBuildingsPlaced { get; private set; }
    public int TotalBuildingsUpgraded { get; private set; }
    public int TotalDamageDealt { get; private set; }
    public int HighestDamageDealt { get; private set; }
    public int TotalDamageTaken { get; private set; }



    public void Init(CardDeckInUseData starterDeck)
    {
        _startTime = Time.time;
        _enemyTypeToDamageDealt = new Dictionary<EnemyTypeConfig, int>();
        
        DeckInUse = starterDeck;
        RunDuration = 0;
        NodesReached = 0;
        
        TotalBuildingsPlaced = 0;
        TotalBuildingsUpgraded = 0;

        TotalDamageDealt = 0;
        HighestDamageDealt = 0;
        TotalDamageTaken = 0;
    }
    
    public void Finish()
    {
        RunDuration = Time.time - _startTime;
    }

    
    
    public void IncrementNodesReached()
    {
        ++NodesReached;
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


}
