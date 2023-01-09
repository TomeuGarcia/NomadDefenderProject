using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWindowManager : MonoBehaviour
{
    //Singleton
    private static UIWindowManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    public static UIWindowManager GetInstance()
    {
        return instance;
    }



    //In-Battle Building Upgrades
    private InBattleBuildingUpgrader currentInBattleBuildingUpgrader;

    public void OpenedWindow(InBattleBuildingUpgrader newInBattleBuildingUpgrader)
    {
        if(currentInBattleBuildingUpgrader != null)
        {
            currentInBattleBuildingUpgrader.CloseWindow();
        }

        currentInBattleBuildingUpgrader = newInBattleBuildingUpgrader;
    }

    public void ClosedWindow(InBattleBuildingUpgrader newInBattleBuildingUpgrader)
    {
        currentInBattleBuildingUpgrader = null;
    }

    public bool IsWindowOpen(InBattleBuildingUpgrader newInBattleBuildingUpgrader)
    {
        return (currentInBattleBuildingUpgrader != null);
    }

    public bool IsHoveringOtherWindow(InBattleBuildingUpgrader newInBattleBuildingUpgrader)
    {
        return (currentInBattleBuildingUpgrader != null && currentInBattleBuildingUpgrader.IsHoveringWindow());
    }
}
