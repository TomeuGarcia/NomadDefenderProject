using System;
using System.Collections;
using UnityEngine;

public class TDBattleTutorialsManager : MonoBehaviour
{
    [Header("TD REFERENCES")]
    [SerializeField] private CurrencyCounter _currencyCounter;
    [SerializeField] private HandBuildingCards _hand;
    [SerializeField] private SpeedUpButton _speedUpButton;
    [SerializeField] private BuildingPlacer _buildingPlacer;

    [Space(10)]
    [Header("TUTORIALS")]
    [Header("Building Upgrade")]
    [SerializeField] private OptionalTutorial_CanvasOverview _buildingUpgradeTutorial;
    private const OptionalTutorialTypes BuildingUpgrade_TutorialType = OptionalTutorialTypes.TDGame_BuildingUpgrade;
    private bool _buildingUpgraded = false;
    
    [Header("Game Speed")]
    [SerializeField] private OptionalTutorial_CanvasOverview _gameSpeedTutorial;
    private const OptionalTutorialTypes GameSpeed_TutorialType = OptionalTutorialTypes.TDGame_GameSpeed;
    private bool _gameSpeedInteracted = false;
    
    private IOptionalTutorialsStateManager OptionalTutorialsStateManager => 
        ServiceLocator.GetInstance().OptionalTutorialsStateManager;

    private void Awake()
    {
        OptionalTutorialsStateManager.SetAllTutorialsNotDone(); // Uncomment to Test

        _buildingUpgraded = OptionalTutorialsStateManager.IsTutorialDone(BuildingUpgrade_TutorialType);
        _gameSpeedInteracted = OptionalTutorialsStateManager.IsTutorialDone(GameSpeed_TutorialType);
    }

    private void OnEnable()
    {
        InBattleBuildingUpgrader.OnBuildingUpgraded += OnBuildingUpgraded;
        SpeedUpButton.OnGameSpeedInteracted += OnGameSpeedInteracted;
    }
    private void OnDisable()
    {
        InBattleBuildingUpgrader.OnBuildingUpgraded -= OnBuildingUpgraded;
        SpeedUpButton.OnGameSpeedInteracted -= OnGameSpeedInteracted;
    }

    private void OnDestroy()
    {
        GameTime.SetTimeScale(1f);
        BuildingCard.LockAllCardsFromHover = false;
    }


    public IEnumerator PlayWaveStartTutorials(int waveNumber)
    {
        if (ShouldPlay_BuildingUpgradeTutorial(waveNumber))
        {
            yield return StartCoroutine(Play_BuildingUpgradeTutorial());
        }
        if (ShouldPlay_GameSpeedTutorial(waveNumber))
        {
            yield return StartCoroutine(Play_GameSpeedTutorial());
        }
    }
    
    
    private bool ShouldPlay_BuildingUpgradeTutorial(int waveNumber)
    {
        return waveNumber >= 2 && !OptionalTutorialsStateManager.IsTutorialDone(BuildingUpgrade_TutorialType);
    }

    private IEnumerator Play_BuildingUpgradeTutorial()
    {
        yield return new WaitUntil(() => _currencyCounter.HasEnoughCurrency(150) && 
                                         !_hand.IsInteractingWithCards &&
                                         _buildingPlacer.PlacedBuildingsCount > 0);
        if (!ShouldPlay_BuildingUpgradeTutorial(2))
        {
            yield break;
        }
        
        _speedUpButton.CompletelyDisableTimeSpeed();
        GameTime.SetTimeScale(0f);
        BuildingCard.LockAllCardsFromHover = true;
        Coroutine buildingUpgradeTutorial = StartCoroutine(_buildingUpgradeTutorial.Play());
        
        yield return new WaitUntil(() => _buildingUpgraded);
        
        _buildingUpgradeTutorial.Finish();
        if (buildingUpgradeTutorial != null)
        {
            StopCoroutine(buildingUpgradeTutorial);
        } 
        
        GameTime.SetTimeScale(1f);
        _speedUpButton.CompletelyEnableTimeSpeed();
        BuildingCard.LockAllCardsFromHover = false;
        OptionalTutorialsStateManager.SetTutorialAsDone(BuildingUpgrade_TutorialType);
    }
    private void OnBuildingUpgraded()
    {
        if (!_buildingUpgraded)
        {
            OptionalTutorialsStateManager.SetTutorialAsDone(OptionalTutorialTypes.TDGame_BuildingUpgrade);
        }
        
        _buildingUpgraded = true;
    }
    
    
    
    private bool ShouldPlay_GameSpeedTutorial(int waveNumber)
    {
        return waveNumber == 1 && !OptionalTutorialsStateManager.IsTutorialDone(GameSpeed_TutorialType) &&
               !_gameSpeedInteracted;
    }
    private IEnumerator Play_GameSpeedTutorial()
    {
        const float delayBeforePlaying = 5f;
        yield return new WaitForSeconds(delayBeforePlaying);

        if (!ShouldPlay_GameSpeedTutorial(1))
        {
            yield break;
        }

        
        GameTime.SetTimeScale(0f);
        yield return StartCoroutine(_gameSpeedTutorial.Play());
        GameTime.SetTimeScale(1f);
        OptionalTutorialsStateManager.SetTutorialAsDone(GameSpeed_TutorialType);
    }
    private void OnGameSpeedInteracted()
    {
        if (!_gameSpeedInteracted)
        {
            OptionalTutorialsStateManager.SetTutorialAsDone(OptionalTutorialTypes.TDGame_GameSpeed);
        }
        
        _gameSpeedInteracted = true;
    }
    
}