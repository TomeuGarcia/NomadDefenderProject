using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;


[CreateAssetMenu(fileName = "SelfHurtUpgradeStat", menuName = "TurretPassives/SelfHurtUpgradeStat")]
public class SelfHurtUpgradeStat : BasePassive
{
    private TurretBinder binder;
    private TurretBuilding owner;


    public override void ApplyEffects(TurretBuilding owner)
    {        
        this.owner = owner;
        owner.OnPlaced += OnOwnerTurretPlaced;

        PathLocation.OnHealthChanged += OnPathLocationChanged;
    }


    private async void OnOwnerTurretPlaced(Building invokerBuilding)
    {
        owner.OnPlaced -= OnOwnerTurretPlaced;
        PathLocation.OnHealthChanged -= OnPathLocationChanged;

        await Task.Delay(300);

        TurretUpgradeType lowestStat = owner.Upgrader.GetLowestStatUpgradeType(false);
        if (lowestStat == TurretUpgradeType.NONE) return;

        owner.Upgrader.FreeTurretUpgrade(lowestStat);

        ServiceLocator.GetInstance().TDLocationsUtils.GetHealthiestLocation().TakeDamage(1);
        
        HideBinder();
    }


    public override void GotEnabledPlacing()
    {
        ShowNewBinder();
        ConnectBinderWithPathLocation();
    }
    public override void GotDisabledPlacing()
    {
        HideBinder();
    }

    public override void GotMovedWhenPlacing()
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
        PathLocation pathLocation = ServiceLocator.GetInstance().TDLocationsUtils.GetHealthiestLocation();
        TurretBinderUtils.UpdateTurretBinder(binder.Transform, pathLocation.transform, owner.BinderPointTransform);
    }

    private void OnPathLocationChanged()
    {
        if (binder != null)
        {
            ConnectBinderWithPathLocation();
        }        
    }
}
