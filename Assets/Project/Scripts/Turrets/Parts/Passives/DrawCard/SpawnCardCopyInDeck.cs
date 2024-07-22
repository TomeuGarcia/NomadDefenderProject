using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "DeploySpawnCopyInDeck", 
    menuName = SOAssetPaths.TURRET_PARTS_BASEPASSIVES + "DeploySpawnCopyInDeck")]
public class SpawnCardCopyInDeck : BasePassive
{
    private TurretBuilding owner;

    [Space]
    [SerializeField, Min(0)] private int costIncrement = 50;


    public override void ApplyEffects(TurretBuilding owner)
    {
        this.owner = owner;
        owner.OnGotPlaced += SpawnCardCopy;
        owner.OnDestroyed += UnsubscriveEvents;        
    }

    private void UnsubscriveEvents()
    {
        owner.OnGotPlaced -= SpawnCardCopy;
        owner.OnDestroyed -= UnsubscriveEvents;
    }


    private async void SpawnCardCopy()
    {
        await Task.Delay(500);

        TurretCardParts partsCopy = ScriptableObject.CreateInstance("TurretCardParts") as TurretCardParts;
        partsCopy.InitCopyingReferences(owner.TurretCardParts);
        partsCopy.cardCost += costIncrement;

        ServiceLocator.GetInstance().CardDrawer.SpawnTurretCardInDeck(partsCopy);
    }

}