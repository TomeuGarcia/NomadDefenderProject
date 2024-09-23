using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TutorialCardOverviewAddOnExtra_Passives : ITutorialCardOverviewAddOnExtra
{
    private readonly TurretIconCanvasDisplay[] _passivesIconDisplays;
    
    public TutorialCardOverviewAddOnExtra_Passives(TurretBuildingCard turretCard)
    {
        _passivesIconDisplays = turretCard.PassivesIconDisplays;
    }
    
    public IEnumerator Play(TutorialCardOverviewAddOn owner)
    {
        foreach (TurretIconCanvasDisplay passiveIconDisplay in _passivesIconDisplays)
        {
            passiveIconDisplay.SetBorderColor(Color.cyan);
            passiveIconDisplay.HideBorder();
            passiveIconDisplay.Show();
        }

        const int maxTimes = 10;
        int times = 0;
        while (!owner.Finished && times < maxTimes)
        {
            foreach (var passiveIconDisplay in _passivesIconDisplays)
            {
                passiveIconDisplay.ShowBorder();
                yield return new WaitForSeconds(0.12f);
                passiveIconDisplay.HideBorder();
            }

            ++times;
        }

        foreach (TurretIconCanvasDisplay passiveIconDisplay in _passivesIconDisplays)
        {
            passiveIconDisplay.SetBorderColor(Color.white);
            passiveIconDisplay.ShowBorder();
        }
        _passivesIconDisplays[1].Hide();
        _passivesIconDisplays[2].Hide();
    }
    
    
}