using System;
using UnityEngine;

public class BuildingSellingController : MonoBehaviour
{
    [SerializeField] private BuildingPlacer _buildingPlacer;

    public static BuildingSellingController Instance { get; private set; }


    private void OnEnable()
    {
        Instance = this;
    }
    private void OnDisable()
    {
        Instance = null;
    }

    public void SellBuilding(Building building, int sellValue)
    {
        _buildingPlacer.UnplaceBuilding(building);
        ServiceLocator.GetInstance().CurrencySpawnService.SpawnCurrency(sellValue, Vector3.down * -1000);
        ServiceLocator.GetInstance().CardDrawer.PutCardBackIntoDeck(building.BuildingCard);
    }
}