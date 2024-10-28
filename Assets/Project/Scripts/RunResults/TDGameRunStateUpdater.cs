using System;
using AYellowpaper;
using UnityEngine;

namespace Project.Scripts.RunResults
{
    public class TDGameRunStateUpdater : MonoBehaviour
    {
        [Header("RUN STATE")] 
        [SerializeField] private InterfaceReference<IRunStateUpdate, ScriptableObject> _runStateUpdate;
        private IRunStateUpdate RunStateUpdate => _runStateUpdate.Value;

        

        private void OnEnable()
        {
            PathLocation.OnDeathGlobal += OnPathLocationDestroyed;
            InBattleBuildingUpgrader.OnBuildingUpgraded += OnBuildingUpgraded;
            BuildingPlacer.OnBuildingPlacedGlobal += OnBuildingPlacedGlobal;
            Enemy.OnTakeDamage += OnEnemyTakesDamage;
            Enemy.OnDealDamage += OnEnemyDealsDamage;
        }
        private void OnDisable()
        {
            PathLocation.OnDeathGlobal -= OnPathLocationDestroyed;
            InBattleBuildingUpgrader.OnBuildingUpgraded -= OnBuildingUpgraded;
            BuildingPlacer.OnBuildingPlacedGlobal -= OnBuildingPlacedGlobal;
            Enemy.OnTakeDamage -= OnEnemyTakesDamage;
            Enemy.OnDealDamage -= OnEnemyDealsDamage;
        }


        private void OnPathLocationDestroyed(PathLocation pathLocation)
        {
            RunStateUpdate.IncrementDestroyedNodes();
        }

        private void OnBuildingUpgraded()
        {
            RunStateUpdate.IncrementUpgradedBuildings();
        }
        private void OnBuildingPlacedGlobal()
        {
            RunStateUpdate.IncrementPlacedBuildings();
        }

        private void OnEnemyTakesDamage(EnemyTypeConfig enemyType, int damageTaken)
        {
            RunStateUpdate.AddDamageDealt(damageTaken);
        }
        private void OnEnemyDealsDamage(EnemyTypeConfig enemyType, int damageDealt)
        {
            RunStateUpdate.AddDamageTaken(damageDealt, enemyType);
        }
    }
}