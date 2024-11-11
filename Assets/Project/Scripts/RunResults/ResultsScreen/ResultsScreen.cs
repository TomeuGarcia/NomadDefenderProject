using System;
using System.Threading.Tasks;
using AYellowpaper;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ResultsScreen : MonoBehaviour
{
    [Header("RUN STATE")]
    [SerializeField] private InterfaceReference<IRunStateData, ScriptableObject> _runStateData;
    private IRunStateData RunStateData => _runStateData.Value;


    [Header("VIEW")] 
    [SerializeField] private Camera _inputCamera;
    [SerializeField] private Camera _camera;
    [SerializeField] private CardMotionConfig _cardMotionConfig;
    [SerializeField] private FadingTextsFactoryConfig _fadingTextsFactoryConfig;
    [SerializeField] private ResultsScreenView _view;
    [SerializeField] private ResultScreenEnemyInteractions _enemyInteractions;
    [SerializeField] private FullScreenPassRendererFeature _fullScreenEffect;
    
    [Header("CONTINUE BUTTON")] 
    [SerializeField] private Button _continueButton;
    
    
    private void Start()
    {
        Init();
    }

    private void OnEnable()
    {
        _continueButton.onClick.AddListener(OnContinueButtonClicked);
        //_fullScreenEffect.SetActive(true);
    }
    private void OnDisable()
    {
        _continueButton.onClick.RemoveAllListeners();
        //_fullScreenEffect.SetActive(false);
    }

    private void Init()
    {
        CheckAchievements();

        CardTooltipDisplayManager.GetInstance().SetDisplayCamera(_inputCamera);
        ServiceLocator.GetInstance().CameraHelp.SetCardsCamera(_inputCamera);
        _cardMotionConfig.SetResultsScreenDisplayMode();
        _fadingTextsFactoryConfig.SetResultsScreenMode();
        
        _view.Init(RunStateData, MakeViewInitData(), _continueButton);
        _view.StartPlayingShowAnimation(RunStateData);
    }
    
    private void CheckAchievements()
    {
        AchievementDefinitions.VictoryWithoutTakingDamage.Check(RunStateData.TotalDamageTaken);
        AchievementDefinitions.VictoryWithoutUpgradingBuildings.Check(RunStateData.TotalBuildingsUpgraded);
    }

    private ResultsScreenView.InitData MakeViewInitData()
    {
        TurretCardData[] turretCards = RunStateData.DeckContent.TurretCardsData;
        TurretCardData mostKillsTurretCard = turretCards[0];
        TurretCardData mostDamageTurretCard = turretCards[0];

        for (int i = 1; i < turretCards.Length; ++i)
        {
            TurretCardData currentTurretCard = turretCards[0];

            if (currentTurretCard.Statistics.TotalKills > mostKillsTurretCard.Statistics.TotalKills)
            {
                mostKillsTurretCard = currentTurretCard;
            }

            if (currentTurretCard.Statistics.TotalDamageDealt > mostDamageTurretCard.Statistics.TotalDamageDealt)
            {
                mostDamageTurretCard = currentTurretCard;
            }
        }

        bool mostKillsAndDamageCardsAreTheSame = mostKillsTurretCard == mostDamageTurretCard;
        TurretBuildingCard mostKillsTurretCardObject =
            ServiceLocator.GetInstance().CardSpawnService.MakeNewTurretCard_FromData(mostKillsTurretCard, transform);
        
        mostKillsTurretCardObject.OnCardUnhovered += SetStandardCard;
        mostKillsTurretCardObject.OnCardHovered += SetHoveredCard;
        mostKillsTurretCardObject.canDisplayInfoIfWhileInteractable = false;

        TurretBuildingCard mostDamageTurretCardObject = null;
        if (mostKillsAndDamageCardsAreTheSame)
        {
            mostDamageTurretCardObject = mostKillsTurretCardObject;
        }
        else
        {
            mostDamageTurretCardObject =
                ServiceLocator.GetInstance().CardSpawnService.MakeNewTurretCard_FromData(mostDamageTurretCard, transform);
            
            mostDamageTurretCardObject.OnCardUnhovered += SetStandardCard;
            mostDamageTurretCardObject.OnCardHovered += SetHoveredCard;
            mostDamageTurretCardObject.canDisplayInfoIfWhileInteractable = false;
        }


        bool mostDamagingEnemyExists =
            RunStateData.MostDamagingEnemy(out EnemyTypeConfig enemyType, out int damage);

        Enemy mostDamagingEnemy = null;
        if (mostDamagingEnemyExists)
        {
            mostDamagingEnemy = EnemyFactory.GetInstance()
                .GetEnemyGameObject(enemyType, transform.position, Quaternion.identity, transform)
                .GetComponent<Enemy>();
            mostDamagingEnemy.InitWithoutFunctionality();

            
            _enemyInteractions.Init(mostDamagingEnemy, RunStateData.HighestDamageDealt);
        }
        
        
        
        ResultsScreenView.InitData viewInitData = new ResultsScreenView.InitData(
            _camera, 
            mostKillsTurretCardObject.gameObject,
            mostDamageTurretCardObject.gameObject,
            mostKillsAndDamageCardsAreTheSame,
            mostDamagingEnemyExists ? mostDamagingEnemy.gameObject : null,
            mostDamagingEnemyExists
            );

        return viewInitData;
    }
    
    
    
    
    
    private void SetHoveredCard(BuildingCard buildingCard)
    {
        GameAudioManager.GetInstance().PlayCardHovered();
        buildingCard.HoveredState(rotate: false);
    }

    private void SetStandardCard(BuildingCard buildingCard)
    {
        buildingCard.StandardState();
    }

    private async void OnContinueButtonClicked()
    {
        _continueButton.interactable = false;

        _continueButton.transform.DOPunchScale(Vector3.one * 0.15f, 0.5f, 7);
        GameAudioManager.GetInstance().PlayCardSelected();
        
        await Task.Delay(TimeSpan.FromSeconds(0.5f));
        
        if (RunStateData.Victory)
        {
            SceneLoader.GetInstance().StartLoadGameEndCredits();
        }
        else
        {
            SceneLoader.GetInstance().StartLoadMainMenu();
        }
    }
}