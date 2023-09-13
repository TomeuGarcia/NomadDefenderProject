using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;



[CreateAssetMenu(fileName = "DeployHealBase", menuName = "TurretPassives/DeployHealBase")]
public class HealBase : BasePassive
{
    [SerializeField] private HealBaseLogic logicPrefab;


    public override void ApplyEffects(TurretBuilding owner)
    {
        HealBaseLogic spawnedLogic = Instantiate(logicPrefab, owner.transform);
        spawnedLogic.Init(owner, owner.CardLevel);
    }

}
