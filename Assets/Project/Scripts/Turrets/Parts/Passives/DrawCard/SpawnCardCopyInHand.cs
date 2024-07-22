using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


[CreateAssetMenu(fileName = "EnemyWaveFinishSpawnCopyInHand", 
    menuName = SOAssetPaths.TURRET_PARTS_BASEPASSIVES + "EnemyWaveFinishSpawnCopyInHand")]
public class SpawnCardCopyInHand : BasePassive
{
    private TurretBuilding owner;

    [Space]
    [SerializeField, Min(0)] private int costIncrement = 50;
    [SerializeField] private TurretPassiveBase baseNullPassive;


    private bool isSubscribed;


    public override void ApplyEffects(TurretBuilding owner)
    {
        this.owner = owner;
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        if (isSubscribed) return;
        isSubscribed = true;

        EnemyWaveManager.OnStartNewWaves += SpawnCardCopy;
        owner.OnDestroyed += UnsubscribeEvents;
        owner.OnPlaced += OnOwnerPlaced;
    }

    private void UnsubscribeEvents()
    {
        if (!isSubscribed) return;
        isSubscribed = false;

        EnemyWaveManager.OnStartNewWaves -= SpawnCardCopy;
        owner.OnDestroyed -= UnsubscribeEvents;
        owner.OnPlaced -= OnOwnerPlaced;
    }


    private void OnOwnerPlaced(Building owner)
    {
        UnsubscribeEvents();
    }

    private async void SpawnCardCopy()
    {
        await Task.Delay(100);

        TurretCardParts partsCopy = ScriptableObject.CreateInstance("TurretCardParts") as TurretCardParts;
        partsCopy.InitCopyingReferences(owner.TurretCardParts);
        partsCopy.turretPassiveBase = baseNullPassive;

        CardDrawer cardDrawer = ServiceLocator.GetInstance().CardDrawer;
        int numberOfCards = cardDrawer.GetCardsInHand().Length + 1;

        partsCopy.cardCost += costIncrement * numberOfCards;
        cardDrawer.SpawnTurretCardInHand(partsCopy);
    }

}