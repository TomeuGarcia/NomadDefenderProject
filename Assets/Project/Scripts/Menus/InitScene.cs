using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitScene : MonoBehaviour
{
    public delegate void InitSceneAction();
    public static event InitSceneAction OnStart;

    [SerializeField] private InitSceneInstaller _initSceneInstaller;


    private IEnumerator Start()
    {
        _initSceneInstaller.Install(ServiceLocator.GetInstance());

        yield return null;
        if (OnStart != null) OnStart();
    }
}
