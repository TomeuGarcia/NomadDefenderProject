using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class InitSceneInstaller : MonoBehaviour
{
    [Header("PARTICLES")]
    [SerializeField] private Transform _particlesParent;
    [SerializeField] private GeneralParticleFactoryConfig _generalParticlesFactoryConfig;
    [SerializeField] private ParticleFactoryConfig _particlesFactoryConfig;
    
    [Header("ACHIEVEMENTS")]
    [SerializeField] private AchievementsManagerConfig _achievementsManagerConfig;

    [Header("GAME DIFFICULTY")] 
    [SerializeField] private GameDifficultyConfig _gameDifficultyConfig;

    [Header("DEMO")] 
    [SerializeField] private DemoManagerConfig _demoManagerConfig;


    [Header("STORAGE")] 
    [SerializeField] private CardCollectionDataStorage _cardCollectionDataStorage;
    [SerializeField] private GameProgressionStatus _gameProgressionStatus;
    
    

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Install(ServiceLocator serviceLocator)
    {
        DOTween.SetTweensCapacity(1250, 50);
        
        serviceLocator.GeneralParticleFactory = new GeneralParticleFactory(_particlesParent, _generalParticlesFactoryConfig);
        serviceLocator.ParticleFactory = new ParticleFactory(_particlesFactoryConfig, _particlesParent);
        serviceLocator.CameraHelp = new CameraHelpService();
        serviceLocator.OptionalTutorialsStateManager = new OptionalTutorialsStateManager_PlayerPrefs();
        
        serviceLocator.AchievementsManager = new SteamAchievementsManager(_achievementsManagerConfig, _demoManagerConfig);
        
        serviceLocator.DynamicProjectileShootingService = new DynamicProjectileShootingService();

        serviceLocator.GameDifficultySettingsSource = _gameDifficultyConfig;
    }

    private void OnApplicationQuit()
    {
        _cardCollectionDataStorage.SaveData(true);
        _gameProgressionStatus.SaveData(true);
    }
}
