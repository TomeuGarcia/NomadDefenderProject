using System.Collections;using UnityEngine;

public class TutorialCardOverviewAddOnExtra_Projectile : ITutorialCardOverviewAddOnExtra
{
    private readonly TurretIconCanvasDisplay _projectileIconDisplay;
    
    public TutorialCardOverviewAddOnExtra_Projectile(TurretBuildingCard turretCard)
    {
        _projectileIconDisplay = turretCard.ProjectileIconDisplay;
    }
    
    public IEnumerator Play(TutorialCardOverviewAddOn owner)
    {
        _projectileIconDisplay.SetBorderColor(Color.cyan);

        const int maxTimes = 10;
        int times = 0;
        while (!owner.Finished && times < maxTimes)
        {
            _projectileIconDisplay.ShowBorder();
            yield return new WaitForSeconds(0.15f);
            _projectileIconDisplay.HideBorder();
            yield return new WaitForSeconds(0.15f);
            ++times;
        }

        _projectileIconDisplay.SetBorderColor(Color.white);
        _projectileIconDisplay.ShowBorder();
    }
    
}