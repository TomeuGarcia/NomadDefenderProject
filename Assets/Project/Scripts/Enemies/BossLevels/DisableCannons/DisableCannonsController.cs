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
    [SerializeField, Min(0)] private float _delayBetweenCannons = 0.2f;
    [SerializeField] private DisableCannon[] _disableCannons;
    private List<DisableCannon> _availableDisableCannons;
    private int _lastUsedCannonIndex;

    [Header("TILES")] 
    [SerializeField] private Transform _shootTilesParent;
    
    
    private void Awake()
    {
        _disableMineFactory.Init();
    
        foreach (DisableCannon disableCannon in _disableCannons)
        {
            disableCannon.Init();
        }


        List<Tile> shootTiles = new List<Tile>(_shootTilesParent.childCount);
        for (int i = 0; i < _shootTilesParent.childCount; ++i)
        {
            if (_shootTilesParent.GetChild(i).TryGetComponent(out Tile tile))
            {
                shootTiles.Add(tile);
            }
        }
        _shootLogic.Init(shootTiles.ToArray(), this);
        

        _availableDisableCannons = new List<DisableCannon>(_disableCannons);
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
            
            yield return new WaitForSeconds(_delayBetweenCannons);
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
}
