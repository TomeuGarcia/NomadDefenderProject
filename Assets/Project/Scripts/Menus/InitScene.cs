using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitScene : MonoBehaviour
{
    [SerializeField] private InitSceneInstaller _initSceneInstaller;
    [SerializeField] private EnemiesPhotograph _enemiesPhotograph;

    private IEnumerator Start()
    {
        _initSceneInstaller.Install(ServiceLocator.GetInstance());

        yield return null;
        yield return new WaitUntil(() => _enemiesPhotograph.Finished);
        SceneLoader.GetInstance().LoadFacilityInstantly();
        GameAudioManager.GetInstance().ChangeMusic(GameAudioManager.MusicType.MENU, 0.5f);
    }
}
