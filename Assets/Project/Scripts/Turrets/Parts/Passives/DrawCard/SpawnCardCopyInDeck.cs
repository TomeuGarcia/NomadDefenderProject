using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "DeploySpawnCopyInDeck", 
    menuName = SOAssetPaths.TURRET_PARTS_BASEPASSIVES + "DeploySpawnCopyInDeck")]
public class SpawnCardCopyInDeck : BasePassive
{
    [Space]
    [SerializeField, Min(0)] private int costIncrement = 50;
    [SerializeField] private SpawnCardCopyInDeckBehaviour _behaviourPrefab;


    public override void ApplyEffects(TurretBuilding owner)
    {
        SpawnCardCopyInDeckBehaviour behaviour = Instantiate(_behaviourPrefab, owner.transform);
        behaviour.Init(owner, costIncrement);        
    }

}