using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HealBaseLogic : MonoBehaviour
{
    private TurretBuilding owner;
    private TurretBinder binder;

    private int healAmount;


    public void Init(TurretBuilding owner, int healAmount)
    {
        this.owner = owner;
        this.healAmount = healAmount;

        SubscriveEvents();
    }

    private void SubscriveEvents()
    {
        owner.OnGotPlaced += OnOwnerGotPlaced;
        owner.OnDestroyed += UnsubscriveEvents;

        owner.OnGotEnabledPlacing += GotEnabledPlacing;
        owner.OnGotDisabledPlacing += GotDisabledPlacing;
        owner.OnGotMovedWhenPlacing += GotMovedWhenPlacing;
    }
    private void UnsubscriveEvents()
    {
        owner.OnGotPlaced -= OnOwnerGotPlaced;
        owner.OnDestroyed -= UnsubscriveEvents;

        owner.OnGotEnabledPlacing -= GotEnabledPlacing;
        owner.OnGotDisabledPlacing -= GotDisabledPlacing;
        owner.OnGotMovedWhenPlacing -= GotMovedWhenPlacing;
    }



    private void OnOwnerGotPlaced()
    {
        HideBinder();
        DoHeal();
    }

    private async void DoHeal()
    {
        await Task.Delay(300);

        PathLocation mostDamagedLocation = ServiceLocator.GetInstance().TDLocationsUtils.GetMostDamagedLocation(owner.Position);

        mostDamagedLocation.Heal(healAmount);
    }



    public void GotEnabledPlacing()
    {
        ShowNewBinder();
        ConnectBinderWithPathLocation();

        PathLocation.OnHealthChanged += OnPathLocationHealthChanged;
    }
    public void GotDisabledPlacing()
    {
        HideBinder();

        PathLocation.OnHealthChanged -= OnPathLocationHealthChanged;
    }

    public void GotMovedWhenPlacing()
    {
        ConnectBinderWithPathLocation();
    }


    public void ShowNewBinder()
    {
        binder = ServiceLocator.GetInstance().TDTurretBinderHelper.TakeBinder(TDTurretBinderHelper.BinderType.HEAL_TARGET);
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
        PathLocation pathLocation = ServiceLocator.GetInstance().TDLocationsUtils.GetMostDamagedLocation(owner.Position);
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
