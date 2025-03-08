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
    [SerializeField] private InterfaceReference<IGameProgressionStatus, ScriptableObject> _gameProgressionStatus;

    [Header("DEMO")] 
    [SerializeField] private DemoManagerConfig _demoManagerConfig;

    private IRunStateData RunStateData => _runStateData.Value;


    [Header("VIEW")] 
    [SerializeField] private Camera _inputCamera;
    [SerializeField] private Camera _camera;
    [SerializeField] private CardMotionConfig _cardMotionConfig;
    [SerializeField] private FadingTextsFactoryConfig _fadingTextsFactoryConfig;
    [SerializeField] private ResultsScreenView _view;
    [SerializeField] private ResultScreenEnemyInteractions _enemyInteractions;
    [SerializeField] private FullScreenPassRendererFeature _fullScreenEffect;
    [SerializeField] private Transform _spawnsHolder;
    
    [Header("CONTINUE BUTTON")] 
    [SerializeField] private Button _continueButton;
    [SerializeField] private Graphic[] _continueButtonArrows;
    [SerializeField] private MouseOverNotifier _continueButtonMouseNotifier;

    private void Awake()
    {
        PauseMenu.GetInstance().GameCanBePaused = false;
    }

    private void OnDestroy()
    {
        PauseMenu.GetInstance().GameCanBePaused = true;
    }

    private void Start()
    {
        Init();
    }

    private void OnEnable()
    {
        _continueButtonMouseNotifier.OnMouseEntered += OnContinueButtonHover;
        _continueButtonMouseNotifier.OnMouseExited += OnContinueButtonUnhover;
        _continueButtonMouseNotifier.OnMousePressed += OnContinueButtonClicked;
    }
    private void OnDisable()
    {
        _continueButtonMouseNotifier.OnMouseEntered -= OnContinueButtonHover;
        _continueButtonMouseNotifier.OnMouseExited -= OnContinueButtonUnhover;
        _continueButtonMouseNotifier.OnMousePressed -= OnContinueButtonClicked;
    }

    private void Init()
    {
        CheckAchievements();
        UpdateDeckHighscore();
        
        CardTooltipDisplayManager.GetInstance().SetDisplayCamera(_inputCamera);
        ServiceLocator.GetInstance().CameraHelp.SetCardsCamera(_inputCamera);
        _cardMotionConfig.SetResultsScreenDisplayMode();
        _fadingTextsFactoryConfig.SetResultsScreenMode();
        
        _view.Init(RunStateData, MakeViewInitData(), _continueButton);
        _view.StartPlayingShowAnimation(RunStateData);
    }
    
    private void CheckAchievements()
    {
        AchievementDefinitions.VictoryWithLessThanDamage.Check(RunStateData.TotalDamageTaken,
            ServiceLocator.GetInstance().GameDifficultySettingsSource.CurrentGameDifficulty);
        
        AchievementDefinitions.VictoryHardRunNoDamage.Check(RunStateData.TotalDamageTaken,
            ServiceLocator.GetInstance().GameDifficultySettingsSource.CurrentGameDifficulty);
    }

    private void UpdateDeckHighscore()
    {
        StarterDeckSaveData starterDeckSaveData = _gameProgressionStatus.Value.Game.StarterDecksSaveStatus.
            GetDeckSaveDataByName(_runStateData.Value.StarterDeck);

        GameDifficultyType gameDifficulty =
            ServiceLocator.GetInstance().GameDifficultySettingsSource.CurrentGameDifficulty;
        
        starterDeckSaveData.UpdateHighscore(gameDifficulty, _runStateData.Value.MakeDeckHighscore());


        if (RunStateData.TotalDamageTaken < 1 && gameDifficulty >= GameDifficultyType.Hard)
        {
            _gameProgressionStatus.Value.Game.SetBeatARunWithFullPerfectDefense();
        }
    }

    private ResultsScreenView.InitData MakeViewInitData()
    {
        TurretCardData[] turretCards = RunStateData.DeckContent.TurretCardsData;
        TurretCardData mostKillsTurretCard = turretCards[0];
        TurretCardData mostDamageTurretCard = turretCards[0];

        for (int i = 1; i < turretCards.Length; ++i)
        {
            TurretCardData currentTurretCard = turretCards[i];

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
            ServiceLocator.GetInstance().CardSpawnService.MakeNewTurretCard_FromData(mostKillsTurretCard, _spawnsHolder);
        
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
                ServiceLocator.GetInstance().CardSpawnService.MakeNewTurretCard_FromData(mostDamageTurretCard, _spawnsHolder);
            
            mostDamageTurretCardObject.OnCardUnhovered += SetStandardCard;
            mostDamageTurretCardObject.OnCardHovered += SetHoveredCard;
            mostDamageTurretCardObject.canDisplayInfoIfWhileInteractable = false;
        }


        bool mostDamagingEnemyExists =
            RunStateData.MostDamagingEnemy(out EnemyTypeConfig enemyType, out int damage);

        Enemy mostDamagingEnemy = null;
        if (mostDamagingEnemyExists)
        {
            mostDamagingEnemy = EnemyFactory.GetInstance().CreateEnemy(enemyType, transform.position, Quaternion.identity, _spawnsHolder);
            mostDamagingEnemy.gameObject.SetActive(false);
            mostDamagingEnemy.InitWithoutFunctionality();

            const float hitsToKill = 3f;
            int damageToDeal = Mathf.RoundToInt(mostDamagingEnemy.HealthSystem.GetMaxHealth() / hitsToKill);
            _enemyInteractions.Init(mostDamagingEnemy, damageToDeal);
        }
        
        
        
        ResultsScreenView.InitData viewInitData = new ResultsScreenView.InitData(
            _camera, 
            mostKillsTurretCardObject.gameObject, mostKillsTurretCard.Statistics.TotalKills,
            mostDamageTurretCardObject.gameObject, mostDamageTurretCard.Statistics.TotalDamageDealt,
            mostKillsAndDamageCardsAreTheSame,
            mostDamagingEnemyExists ? mostDamagingEnemy.gameObject : null,
            mostDamagingEnemyExists ? damage : 0,
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

    private void OnContinueButtonHover()
    {
        if (!_continueButton.interactable) return;
        SetContinueButtonColor(_continueButton.colors.highlightedColor);
    }

    private void OnContinueButtonUnhover()
    {
        if (!_continueButton.interactable) return;
        SetContinueButtonColor(_continueButton.colors.normalColor);
    }

    private void SetContinueButtonColor(Color color)
    {
        _continueButton.targetGraphic.color = color;
        foreach (Graphic continueButtonArrow in _continueButtonArrows)
        {
            continueButtonArrow.color = color;
        }
    }
    
    private async void OnContinueButtonClicked()
    {
        _continueButton.interactable = false;
        
        _continueButton.transform.DOPunchScale(Vector3.one * 0.30f, 0.5f, 7);
        _continueButton.transform.DOPunchPosition(Vector3.back * 0.10f, 0.3f, 3);
        GameAudioManager.GetInstance().PlayCardSelected();
        
        await Task.Delay(TimeSpan.FromSeconds(0.5f));

        if (_comingFromVictoryUnlocks)
        {
            OnContinueButtonClickedAfterUnlocks_Victory_Normal();
            return;
        }
        
        if (RunStateData.Victory)
        {
            if (_demoManagerConfig.DemoEnabled)
            {
                OnContinueButtonClicked_Victory_Demo();
            }
            else
            {
                OnContinueButtonClicked_Victory_Normal();
            }
        }
        else
        {
            OnContinueButtonClicked_Defeat();
        }
    }

    private void OnContinueButtonClicked_Defeat()
    {
        SceneLoader.GetInstance().StartLoadMainMenu();
    }
    
    private void OnContinueButtonClicked_Victory_Demo()
    {
        if (_gameProgressionStatus.Value.Game.VictoriesCount > 1)
        {
            SceneLoader.GetInstance().StartLoadMainMenu();
        }
        else
        {
            SceneLoader.GetInstance().StartLoadDemoThanksForPlaying();
        }
    }


    private bool _comingFromVictoryUnlocks = false;
    private void OnContinueButtonClicked_Victory_Normal()
    {
        bool unlockDifficulty = _runStateData.Value.UnlockDifficulty;
        bool unlocksStarterDeck = _runStateData.Value.UnlockStarterDeck;
        bool hasPendingUnlocks = _runStateData.Value.HasPendingUnlocks;

        if (hasPendingUnlocks)
        {
            SetContinueButtonColor(_continueButton.colors.normalColor);
            _spawnsHolder.gameObject.SetActive(false);
            _view.StartPlayingShowUnlocksAnimation(unlockDifficulty, unlocksStarterDeck);
            _comingFromVictoryUnlocks = true;
        }
        else
        {
            OnContinueButtonClickedAfterUnlocks_Victory_Normal();
        }
    }
    
    private void OnContinueButtonClickedAfterUnlocks_Victory_Normal()
    {
        if (_gameProgressionStatus.Value.Game.VictoriesCount > 1)
        {
            SceneLoader.GetInstance().StartLoadMainMenu();
        }
        else
        {
            SceneLoader.GetInstance().StartLoadGameEndCredits();
        }
    }
}