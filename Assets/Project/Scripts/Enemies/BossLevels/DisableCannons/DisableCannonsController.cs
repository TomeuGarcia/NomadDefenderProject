using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class DisableCannonsController : MonoBehaviour, IDisableCannonsShootController, IDisableMineDisappearListener
{
    [Header("FACTORY")]
    [SerializeField] private DisableMineFactory _disableMineFactory;

    [Header("LOGIC")] 
    [SerializeField] private DisableCannonsShootLogic _shootLogic;

    [Header("CANNONS")] 
    [SerializeField] private DisableCannon[] _startingDisableCannons;
    private List<DisableCannon> _availableDisableCannons;
    private int _lastUsedCannonIndex;

    [Header("TILES")] 
    [SerializeField] private Transform _shootTilesParent;
    
    [Header("TIMINGS")]
    [SerializeField, Min(0)] private float _delayBetweenCannonShots = 0.2f;
    [SerializeField, Min(0)] private float _delayBetweenCannonActivations = 0.5f;
    [SerializeField, Min(0)] private float _delayBetweenCannonDeactivations = 0.2f;
    
    
    
    private void Awake()
    {
        _disableMineFactory.Init();
        
        
        List<Tile> shootTiles = new List<Tile>(_shootTilesParent.childCount);
        for (int i = 0; i < _shootTilesParent.childCount; ++i)
        {
            if (_shootTilesParent.GetChild(i).TryGetComponent(out Tile tile))
            {
                shootTiles.Add(tile);
            }
        }
        _shootLogic.Init(shootTiles.ToArray(), this);
        

        foreach (DisableCannon disableCannon in _startingDisableCannons)
        {
            disableCannon.Init();
        }
        _availableDisableCannons = new List<DisableCannon>(_startingDisableCannons.Length);
        StartCoroutine(AddAvailableCannons(_startingDisableCannons));
        
        
        _lastUsedCannonIndex = 0;
    }
    
    
    private IEnumerator DoMakeCannonsShoot(DisableMine[] minesToShoot)
    {
        int lastUsedCannonIndexCopy = _lastUsedCannonIndex;
        _lastUsedCannonIndex = (_lastUsedCannonIndex + minesToShoot.Length) % _availableDisableCannons.Count;
        
        for (int i = 0; i < minesToShoot.Length; ++i)
        {
            DisableMine mine = minesToShoot[i];
            Vector3 missileEndPosition = mine.OccupiedTile.buildingPlacePosition;

            int cannonIndex = (lastUsedCannonIndexCopy + i) % _availableDisableCannons.Count;
            DisableCannon cannon = _availableDisableCannons[cannonIndex];
            
            cannon.LaunchMissile(missileEndPosition, mine);

            yield return StartCoroutine(GameTime.WaitForSeconds(_delayBetweenCannonShots));
        }
    }

    public void ShootAtTiles(Tile[] tiles)
    {
        DisableMine[] mines = new DisableMine[tiles.Length];

        for (int i = 0; i < mines.Length; ++i)
        {
            Tile tile = tiles[i];
            DisableMine mine = _disableMineFactory.Create(tile.buildingPlacePosition);
            mine.Prepare(tile, this);
            mines[i] = mine;
        }

        StartCoroutine(DoMakeCannonsShoot(mines));
    }

    public void OnDisableMineDisappeared(DisableMine disableMine)
    {
        _shootLogic.MakeTileAvailable(disableMine.OccupiedTile);
    }



    public IEnumerator AddAvailableCannons(DisableCannon[] cannonsToAdd)
    {
        List<DisableCannon> newAddedCannons = new List<DisableCannon>(cannonsToAdd.Length);

        foreach (DisableCannon cannonToAdd in cannonsToAdd)
        {
            if (!_availableDisableCannons.Contains(cannonToAdd))
            {
                _availableDisableCannons.Add(cannonToAdd);
                newAddedCannons.Add(cannonToAdd);
            }
        }
        
        for (int i = 0; i < newAddedCannons.Count; ++i)
        {
            newAddedCannons[i].PlayEnterActive();
            yield return StartCoroutine(GameTime.WaitForSeconds(_delayBetweenCannonActivations));
        }
    }
    
    public IEnumerator RemoveAllAvailableCannons()
    {
        if (_availableDisableCannons.Count <= 0)
        {
            yield break;
        }
        
        DisableCannon[] randomlySortedCannons = new DisableCannon[_availableDisableCannons.Count];
        int randomSortI = 0;
        while (_availableDisableCannons.Count > 0)
        {
            int randomIndex = Random.Range(0, _availableDisableCannons.Count);
            randomlySortedCannons[randomSortI] = _availableDisableCannons[randomIndex];
            
            _availableDisableCannons.RemoveAt(randomIndex);
            
            ++randomSortI;
        }
        
        for (int i = 0; i < randomlySortedCannons.Length; ++i)
        {
            randomlySortedCannons[i].PlayEnterNotActive();
            yield return StartCoroutine(GameTime.WaitForSeconds(_delayBetweenCannonDeactivations));
        }
    }
    
}
