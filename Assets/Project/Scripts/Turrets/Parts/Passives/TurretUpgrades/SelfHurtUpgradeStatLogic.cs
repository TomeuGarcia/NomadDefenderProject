using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SelfHurtUpgradeStatLogic : MonoBehaviour
{
    private TurretBinder binder;
    private TurretBuilding owner;

    private bool isSubscribed = false;

    private int damageAmount;


    private void SubscribeEvents()
    {
        if (isSubscribed) return;
        isSubscribed = true;

        owner.OnPlaced += OnOwnerTurretPlaced;
        owner.OnDestroyed += UnsubscribeEvents;
        owner.OnGotEnabledPlacing += GotEnabledPlacing;
        owner.OnGotDisabledPlacing += GotDisabledPlacing;
        owner.OnGotMovedWhenPlacing += GotMovedWhenPlacing;

        PathLocation.OnHealthChanged += OnPathLocationHealthChanged;
    }
    private void UnsubscribeEvents()
    {
        if (!isSubscribed) return;
        isSubscribed = false;

        owner.OnPlaced -= OnOwnerTurretPlaced;
        owner.OnDestroyed -= UnsubscribeEvents;
        owner.OnGotEnabledPlacing -= GotEnabledPlacing;
        owner.OnGotDisabledPlacing -= GotDisabledPlacing;
        owner.OnGotMovedWhenPlacing -= GotMovedWhenPlacing;

        PathLocation.OnHealthChanged -= OnPathLocationHealthChanged;
    }


    public void Init(TurretBuilding owner, int damageAmount)
    {
        this.owner = owner;
        this.damageAmount = damageAmount;

        isSubscribed = false;
        SubscribeEvents();        
    }


    private async void OnOwnerTurretPlaced(Building invokerBuilding)
    {
        UnsubscribeEvents();

        await Task.Delay(300);

        TurretUpgradeType lowestStat = owner.Upgrader.GetLowestStatUpgradeType(false);
        if (lowestStat == TurretUpgradeType.NONE) return;

        owner.Upgrader.FreeTurretUpgrade(lowestStat);

        ServiceLocator.GetInstance().TDLocationsUtils.GetHealthiestLocation(owner.Position).TakeDamage(damageAmount);

        HideBinder();
    }


    public void GotEnabledPlacing()
    {
        ShowNewBinder();
        ConnectBinderWithPathLocation();
    }
    public void GotDisabledPlacing()
    {
        HideBinder();
    }

    public void GotMovedWhenPlacing()
    {
        ConnectBinderWithPathLocation();
    }


    public void ShowNewBinder()
    {
        binder = ServiceLocator.GetInstance().TDTurretBinderHelper.TakeBinder(TDTurretBinderHelper.BinderType.HURT_TARGET);
        binder.Show();
    }
    public void HideBinder()
    {
        if (binder == null) return;

        binder.Hide();
        ServiceLocator.GetInstance().TDTurretBinderHelper.GiveBackBinder(binder);
        binder = null;
    }

    private void ConnectBinderWithPathLocation()
    {
        PathLocation pathLocation = ServiceLocator.GetInstance().TDLocationsUtils.GetHealthiestLocation(owner.Position);
        TurretBinderUtils.UpdateTurretBinder(binder.Transform, pathLocation.transform, owner.BinderPointTransform);
    }

    private void OnPathLocationHealthChanged()
    {
        if (binder != null)
        {
            ConnectBinderWithPathLocation();
        }
    }
}
