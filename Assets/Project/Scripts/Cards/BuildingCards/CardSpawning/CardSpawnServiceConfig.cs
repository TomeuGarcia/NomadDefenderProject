using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardSpawnServiceConfig", 
    menuName = SOAssetPaths.CARDS + "CardSpawnServiceConfig")]
public class CardSpawnServiceConfig : ScriptableObject
{
    [Header("TURRETS")]
    [SerializeField] private TurretBuildingCard _turretCardPrefab;
    [SerializeField] private TurretBuilding _turretPrefab;
    
    [Header("SUPPORTS")]
    [SerializeField] private SupportBuildingCard _supportCardPrefab;
    [SerializeField] private SupportBuilding _supportPrefab;
    
    
    public TurretBuildingCard TurretCardPrefab => _turretCardPrefab;
    public TurretBuilding TurretPrefab => _turretPrefab;
    
    public SupportBuildingCard SupportCardPrefab => _supportCardPrefab;
    public SupportBuilding SupportPrefab => _supportPrefab;
    
}
