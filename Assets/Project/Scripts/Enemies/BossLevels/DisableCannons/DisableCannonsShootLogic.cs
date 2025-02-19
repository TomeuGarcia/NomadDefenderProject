using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class DisableCannonsShootLogic : MonoBehaviour
{
    private Tile[] _defaultAvailableTiles;
    private HashSet<Tile> _currentlyAvailableTiles;
    private IDisableCannonsShootController _shootController;

    public void Init(Tile[] defaultAvailableTiles, IDisableCannonsShootController shootController)
    {
        _defaultAvailableTiles = defaultAvailableTiles;
        _currentlyAvailableTiles = new HashSet<Tile>(_defaultAvailableTiles);
        _shootController = shootController;
    }


    [Button()]
    private void ShootFocusingRandomBuilding_3_HardTarget()
    {
        ShootFocusingRandomBuilding(3, 1, 0, 1, 4);
    }

    [Button()]
    private void ShootFocusingRandomBuilding_3_SoftTarget()
    {
        ShootFocusingRandomBuilding(3, 2, 2, 1, 8);
    }
    
    [Button()]
    private void ShootFocusingRandomBuilding_3_SoftTargetBackwards()
    {
        ShootFocusingRandomBuilding(3, 5, -2, 1, 8);
    }
    


    private void ShootFocusingRandomBuilding(int numberOfShots, 
        int startDistanceStep = 1, int distanceStepChangePerShot = 0, 
        int minStepDistance = 1, int maxStepDistance = 4)
    {
        Building[] placedBuildings = BuildingPlacer.GetCurrentPlacedBuildings();
        if (placedBuildings.Length < 1)
        {
            Debug.Log("NO Buildings xdd");
            return;
        }

        
        Building randomlyTargetedBuilding = placedBuildings[Random.Range(0, placedBuildings.Length)];
        Vector3 targetPosition = randomlyTargetedBuilding.PlacedTile.buildingPlacePosition;

        ShootFocusingPosition(numberOfShots, targetPosition,
            startDistanceStep, distanceStepChangePerShot, minStepDistance, maxStepDistance);
    }
    
    private void ShootFocusingPosition(int numberOfShots, Vector3 targetPosition, 
        int startDistanceStep = 1, int distanceStepChangePerShot = 0, 
        int minStepDistance = 1, int maxStepDistance = 4)
    {
        startDistanceStep = Mathf.Clamp(startDistanceStep, minStepDistance, maxStepDistance);
        distanceStepChangePerShot = Mathf.Clamp(distanceStepChangePerShot, -maxStepDistance, maxStepDistance);
        int distanceStep = startDistanceStep;
        
        Dictionary<int, List<Tile>> tilesOverDistance = 
            MakeTilesOverDistanceMap(targetPosition, minStepDistance, maxStepDistance);

        
        List<Tile> targetedTiles = new List<Tile>(numberOfShots);
        for (int tileTargetIt = 0; tileTargetIt < numberOfShots; ++tileTargetIt)
        {
            Tile targetedTile = GetRandomTileAndUpdateState(tilesOverDistance, distanceStep);
            if (targetedTile == null)
            {
                Debug.Log("No tiles found");
                continue;
            }
            
            targetedTiles.Add(targetedTile);

            distanceStep = (distanceStep + distanceStepChangePerShot + maxStepDistance) % maxStepDistance;
        }
        
        
        _shootController.ShootAtTiles(targetedTiles.ToArray());
    }


    private Dictionary<int, List<Tile>> MakeTilesOverDistanceMap(Vector3 targetedPosition, 
        int minDistanceStep, int maxDistanceStep)
    {
        int totalDistanceSteps = maxDistanceStep - minDistanceStep;
        
        Dictionary<int, List<Tile>> tilesOverDistance = 
            new Dictionary<int, List<Tile>>(totalDistanceSteps);

        
        Tile[] availableTiles = _currentlyAvailableTiles.ToArray();
        foreach (Tile availableTile in availableTiles)
        {
            if (availableTile.isOccupied)
            {
                continue;
            }
            
            Vector3 tilePosition = availableTile.buildingPlacePosition;
            int distance = Mathf.RoundToInt((tilePosition - targetedPosition).magnitude);

            if (tilesOverDistance.TryGetValue(distance, out List<Tile> tilesAtDistance))
            {
                tilesAtDistance.Add(availableTile);
            }
            else
            {
                List<Tile> newTilesAtDistanceEntry = new List<Tile>() { availableTile };
                tilesOverDistance.Add(distance, newTilesAtDistanceEntry);
            }
        }

        return tilesOverDistance;
    }

    
    private Tile GetRandomTileAndUpdateState(Dictionary<int, List<Tile>> tilesOverDistance, int distanceStep)
    {
        const int MAX_ITERATIONS = 5;
        int iterationsCount = 0;
        
        int maxDistanceStep = 1;
        foreach (KeyValuePair<int,List<Tile>> pair in tilesOverDistance)
        {
            if (pair.Key > maxDistanceStep && pair.Value.Count > 0)
            {
                maxDistanceStep = pair.Key;
            }
        }
        

        List<Tile> tilesAtDistance = null;
        
        while ((!tilesOverDistance.TryGetValue(distanceStep, out tilesAtDistance) || tilesAtDistance.Count <= 0) &&
               iterationsCount < MAX_ITERATIONS)
        {
            distanceStep = (distanceStep + 1) % maxDistanceStep;
            ++iterationsCount;
        }
        

        if (tilesAtDistance == null)
        {
            return null;
        }
        
        
        int randomTileIndex = Random.Range(0, tilesAtDistance.Count);
        Tile randomTile = tilesAtDistance[randomTileIndex];
        tilesAtDistance.RemoveAt(randomTileIndex);

        MakeTileNotAvailable(randomTile);

        return randomTile;
    }


    private void MakeTileNotAvailable(Tile tile)
    {
        _currentlyAvailableTiles.Remove(tile);
    }
    public void MakeTileAvailable(Tile tile)
    {
        _currentlyAvailableTiles.Add(tile);
    }
    
}