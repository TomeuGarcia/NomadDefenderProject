using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitSceneInstaller : MonoBehaviour
{
    [Header("PARTICLES")]
    [SerializeField] private Transform _particlesParent;
    [SerializeField] private GeneralParticleFactoryConfig _generalParticlesFactoryConfig;
    [SerializeField] private ParticleFactoryConfig _particlesFactoryConfig;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Install(ServiceLocator serviceLocator)
    {
        serviceLocator.GeneralParticleFactory = new GeneralParticleFactory(_particlesParent, _generalParticlesFactoryConfig);
        serviceLocator.ParticleFactory = new ParticleFactory(_particlesFactoryConfig, _particlesParent);
        serviceLocator.CameraHelp = new CameraHelpService();
        serviceLocator.OptionalTutorialsStateManager = new OptionalTutorialsStateManager_PlayerPrefs();
    }
}
