using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    static private ServiceLocator instance;

    public CardDrawer CardDrawer { get; set; }
    public ITDGameState TDGameState { get; set; }
    public TDLocationsUtils TDLocationsUtils { get; set; }
    public TDTurretBinderHelper TDTurretBinderHelper { get; set; }
    public CurrencyCounter CurrencyCounter { get; set; }
    public IFadingTextsFactory FadingTextFactory { get; set; }
    public ITDCurrencySpawnService CurrencySpawnService { get; set; }
    public RunInfo RunInfo { get; set; }
    public CursorChanger CursorChanger { get; set; }
    public IGeneralParticleFactory GeneralParticleFactory { get; set; }
    public IParticleFactory ParticleFactory { get; set; }
    public ICameraHelpService CameraHelp { get; set; }
    public ICardSpawnService CardSpawnService { get; set; }
    public IOptionalTutorialsStateManager OptionalTutorialsStateManager { get; set; }
    public ITutorialViewUtilities TutorialViewUtilities { get; set; }


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
