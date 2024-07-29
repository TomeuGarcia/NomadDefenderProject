using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


[CreateAssetMenu(fileName = "EnemyWaveFinishSpawnCopyInHand", 
    menuName = SOAssetPaths.TURRET_PARTS_BASEPASSIVES + "EnemyWaveFinishSpawnCopyInHand")]
public class SpawnCardCopyInHand : BasePassive
{
    [Space]
    [SerializeField, Min(0)] private int costIncrement = 50;
    [SerializeField] private TurretPassiveBase baseNullPassive;
    [SerializeField] private SpawnCardCopyInHandBehaviour _behaviourPrefab;

    public override void ApplyEffects(TurretBuilding owner)
    {
        SpawnCardCopyInHandBehaviour behaviour = Instantiate(_behaviourPrefab, owner.transform);
        behaviour.Init(owner, costIncrement, baseNullPassive);
    }
}