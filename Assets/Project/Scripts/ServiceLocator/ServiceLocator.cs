using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    static private ServiceLocator instance;

    public CardDrawer CardDrawer { get; set; }
    public TDLocationsUtils TDLocationsUtils { get; set; }
    public TDTurretBinderHelper TDTurretBinderHelper { get; set; }
    public CurrencyCounter CurrencyCounter { get; set; }


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


    public static ServiceLocator GetInstance()
    {
        return instance;
    }


}
