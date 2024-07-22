using System;
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

        await Task.Delay(TimeSpan.FromSeconds(0.3f));

        owner.Upgrader.FreeTurretUpgrade();

        if (ServiceLocator.GetInstance().TDLocationsUtils.GetHealthiestLocation(owner.Position, out PathLocation pathLocatioon))
        {
            pathLocatioon.TakeDamage(damageAmount);
        }

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
        if (ServiceLocator.GetInstance().TDLocationsUtils.GetHealthiestLocation(owner.Position, out PathLocation pathLocation))
        {
            TurretBinderUtils.UpdateTurretBinder(binder.Transform, pathLocation.transform, owner.BinderPointTransform);
        }        
    }

    private void OnPathLocationHealthChanged()
    {
        if (binder != null)
        {
            ConnectBinderWithPathLocation();
        }
    }
}
