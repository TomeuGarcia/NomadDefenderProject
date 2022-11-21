using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SceneLoader : MonoBehaviour
{
    [SerializeField] private Transform topBlackScreen;
    private Vector3 topOpenPosition;

    [SerializeField] private Transform bottomBlackScreen;
    private Vector3 bottomOpenPosition;

    [SerializeField] private Image backgroundImage;
    [SerializeField] private Color shutColor;
    [SerializeField] private Color openColor;

    [SerializeField, Min(0f)] private float openAnimDuration = 0.2f;
    [SerializeField, Min(0f)] private float shutAnimDuration = 0.1f;
    [SerializeField, Min(0f)] private float loadSceneDuration = 0.7f;

    delegate void LoadSceneFunction();

    private int mainMenuSceneIndex = 1;
    private bool alreadyLoadingNextScene;


    public delegate void SceneLoaderAction();
    public static event SceneLoaderAction OnSceneForceQuit;




    private void Awake()
    {
        DontDestroyOnLoad(this);

        topOpenPosition = topBlackScreen.localPosition;
        bottomOpenPosition = bottomBlackScreen.localPosition;
        backgroundImage.color = openColor;

        alreadyLoadingNextScene = false;
    }

    private void OnEnable()
    {
        TDGameManager.OnVictoryComplete += StartLoadNextScene;
        TDGameManager.OnGameOverComplete += StartReloadCurrentScene;

        CardPartReplaceManager.OnReplacementDone += StartLoadNextScene;

        GatherNewCardManager.OnCardGatherDone += StartLoadNextScene;

        InitScene.OnStart += StartLoadMainMenu;
        MainMenu.OnPlayStart += StartLoadNextScene;
    }

    private void OnDisable()
    {
        TDGameManager.OnVictoryComplete -= StartLoadNextScene;
        TDGameManager.OnGameOverComplete -= StartReloadCurrentScene;

        CardPartReplaceManager.OnReplacementDone -= StartLoadNextScene;

        GatherNewCardManager.OnCardGatherDone -= StartLoadNextScene;

        InitScene.OnStart -= StartLoadMainMenu;
        MainMenu.OnPlayStart -= StartLoadNextScene;
    }

    private void Update()
    {
        if (alreadyLoadingNextScene) return;

        //if (Input.GetKeyDown(KeyCode.O)) OpenAnimation(openAnimDuration);
        //else if (Input.GetKeyDown(KeyCode.P)) ShutAnimation(shutAnimDuration);

        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().buildIndex != mainMenuSceneIndex)
        {
            if (OnSceneForceQuit != null) OnSceneForceQuit();
            StartLoadMainMenu();
        }
    }


    private void ShutAnimation(float duration)
    {
        topBlackScreen.DOLocalMove(Vector3.zero, duration);
        bottomBlackScreen.DOLocalMove(Vector3.zero, duration);
        backgroundImage.DOColor(shutColor, duration * 0.8f);
    }

    private void OpenAnimation(float duration)
    {
        topBlackScreen.DOLocalMove(topOpenPosition, duration);
        bottomBlackScreen.DOLocalMove(bottomOpenPosition, duration);
        backgroundImage.DOColor(openColor, duration * 1.2f);
    }

    private void StartLoadNextScene()
    {
        StartCoroutine(DoLoadScene(LoadNextScene));
    }
    private IEnumerator DoLoadScene(LoadSceneFunction loadSceneFunction)
    {
        alreadyLoadingNextScene = true;

        ShutAnimation(shutAnimDuration);
        GameAudioManager.GetInstance().PlayScreenShut();
        yield return new WaitForSeconds(shutAnimDuration);

        loadSceneFunction();
        yield return new WaitForSeconds(loadSceneDuration);

        OpenAnimation(openAnimDuration);
        GameAudioManager.GetInstance().PlayScreenOpen();
        yield return new WaitForSeconds(openAnimDuration);

        alreadyLoadingNextScene = false;
    }
    private void LoadNextScene()
    {
        int nextSceneI = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneI < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneI);
        }
        else
        {
            LoadMainMenu();
        }
        
    }

    private void StartLoadMainMenu()
    {
        StartCoroutine(DoLoadScene(LoadMainMenu));
    }
    private void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneIndex); // Main menu
    }


    private void StartReloadCurrentScene()
    {
        StartCoroutine(DoLoadScene(ReloadCurrentScene));
    }
    private void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }



}
