using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class SpawnCardCopyInDeckBehaviour : MonoBehaviour
{
    private TurretBuilding _owner;
    private int _costIncrement;

    private void OnDestroy()
    {
        UnsubscriveEvents();
    }

    public void Init(TurretBuilding owner, int costIncrement)
    {
        _owner = owner;
        _costIncrement = costIncrement;

        owner.OnGotPlaced += SpawnCardCopy;
    }

    private void UnsubscriveEvents()
    {
        _owner.OnGotPlaced -= SpawnCardCopy;
    }


    private async void SpawnCardCopy()
    {
        await Task.Delay(System.TimeSpan.FromSeconds(0.5f));

        TurretCardParts partsCopy = ScriptableObject.CreateInstance<TurretCardParts>();
        partsCopy.InitCopyingReferences(_owner.TurretCardParts);
        partsCopy.cardCost += _costIncrement;

        ServiceLocator.GetInstance().CardDrawer.SpawnTurretCardInDeck(partsCopy);
    }
}
