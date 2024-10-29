using AYellowpaper;
using UnityEngine;

public class ResultsScreen : MonoBehaviour
{
    [Header("RUN STATE")]
    [SerializeField] private InterfaceReference<IRunStateData, ScriptableObject> _runStateData;
    private IRunStateData RunStateData => _runStateData.Value;


    [Header("VIEW")] 
    [SerializeField] private Camera _camera;
    [SerializeField] private ResultsScreenView _view;
    
    
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        CheckAchievements();

        _view.Init(RunStateData, MakeViewInitData());
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
        }


        bool mostDamagingEnemyExists =
            RunStateData.MostDamagingEnemy(out EnemyTypeConfig enemyType, out int damage);

        Enemy mostDamagingEnemy = null;
        if (mostDamagingEnemyExists)
        {
            mostDamagingEnemy = EnemyFactory.GetInstance()
                .GetEnemyGameObject(enemyType, transform.position, Quaternion.identity, transform)
                .GetComponent<Enemy>();
            mostDamagingEnemy.enabled = false;

            mostDamagingEnemy.GetComponent<PathFollower>().enabled = false;

            if (mostDamagingEnemy.TryGetComponent(out AreaSpawnerArmor areaSpawnerArmor))
            {
                areaSpawnerArmor.enabled = false;
            }
            if (mostDamagingEnemy.TryGetComponent(out AreaSpawnerHealth areaSpawnerHealth))
            {
                areaSpawnerHealth.enabled = false;
            }
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
    
    
    
    
    
    void SetHoveredCard(BuildingCard buildingCard)
    {
        GameAudioManager.GetInstance().PlayCardHovered();
        buildingCard.HoveredState(rotate: false);
    }

    void SetStandardCard(BuildingCard buildingCard)
    {
        buildingCard.StandardState();
    }
}