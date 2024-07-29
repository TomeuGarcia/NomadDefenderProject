using System.Threading.Tasks;
using UnityEngine;

public class SpawnCardCopyInHandBehaviour : MonoBehaviour
{
    private TurretBuilding _owner;
    private bool _isSubscribed;

    int _costIncrement = 50;
    private TurretPassiveBase _baseNullPassive;


    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    public void Init(TurretBuilding owner, int costIncrement, TurretPassiveBase baseNullPassive)
    {
        _owner = owner;
        _costIncrement = costIncrement;
        _baseNullPassive = baseNullPassive;
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        if (_isSubscribed) return;
        _isSubscribed = true;

        EnemyWaveManager.OnStartNewWaves += SpawnCardCopy;
        _owner.OnPlaced += OnOwnerPlaced;
    }

    private void UnsubscribeEvents()
    {
        if (!_isSubscribed) return;
        _isSubscribed = false;

        EnemyWaveManager.OnStartNewWaves -= SpawnCardCopy;
        _owner.OnPlaced -= OnOwnerPlaced;
    }


    private void OnOwnerPlaced(Building owner)
    {
        UnsubscribeEvents();
    }

    private async void SpawnCardCopy()
    {
        await Task.Delay(System.TimeSpan.FromSeconds(0.1f));

        TurretCardParts partsCopy = ScriptableObject.CreateInstance<TurretCardParts>();
        partsCopy.InitCopyingReferences(_owner.TurretCardParts);
        partsCopy.turretPassiveBase = _baseNullPassive;

        CardDrawer cardDrawer = ServiceLocator.GetInstance().CardDrawer;
        int numberOfCards = cardDrawer.GetCardsInHand().Length + 1;

        partsCopy.cardCost += _costIncrement * numberOfCards;
        cardDrawer.SpawnTurretCardInHand(partsCopy);
    }
}
