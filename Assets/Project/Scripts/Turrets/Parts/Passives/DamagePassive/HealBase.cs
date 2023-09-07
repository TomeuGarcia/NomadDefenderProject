using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;



[CreateAssetMenu(fileName = "DeployHealBase", menuName = "TurretPassives/DeployHealBase")]
public class HealBase : BasePassive
{
    private TurretBuilding owner;

    TurretBinder binder;


    public override void ApplyEffects(TurretBuilding owner)
    {
        this.owner = owner;
        owner.OnGotPlaced += OnOwnerGotPlaced;
        owner.OnDestroyed += UnsubscriveEvents;
    }

    private void UnsubscriveEvents()
    {
        owner.OnGotPlaced -= OnOwnerGotPlaced;
        owner.OnDestroyed -= UnsubscriveEvents;
    }



    private void OnOwnerGotPlaced()
    {
        HideBinder();
        DoHeal();
    }

    private async void DoHeal()
    {
        await Task.Delay(300);

        int healAmount = owner.CardLevel;

        PathLocation mostDamagedLocation = ServiceLocator.GetInstance().TDLocationsUtils.GetMostDamagedLocation();

        mostDamagedLocation.Heal(healAmount);
    }



    public override void GotEnabledPlacing()
    {
        ShowNewBinder();
        ConnectBinderWithPathLocation();

        PathLocation.OnHealthChanged += OnPathLocationHealthChanged;
    }
    public override void GotDisabledPlacing()
    {
        HideBinder();

        PathLocation.OnHealthChanged -= OnPathLocationHealthChanged;
    }

    public override void GotMovedWhenPlacing()
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
        PathLocation pathLocation = ServiceLocator.GetInstance().TDLocationsUtils.GetMostDamagedLocation();
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
