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
    delegate void LoadSceneByNameFunction(string sceneName);

    private int mainMenuSceneIndex = 1;
    private bool alreadyLoadingNextScene;


    public delegate void SceneLoaderAction();
    public static event SceneLoaderAction OnSceneForceQuit;


    private static SceneLoader instance;
    public static SceneLoader GetInstance()
    {
        return instance;
    }

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

        topOpenPosition = topBlackScreen.localPosition;
        bottomOpenPosition = bottomBlackScreen.localPosition;
        backgroundImage.color = openColor;

        alreadyLoadingNextScene = false;
    }

    private void OnEnable()
    {
        //TDGameManager.OnVictoryComplete += StartLoadNextScene; // not called, remove after check
        //TDGameManager.OnGameOverComplete += StartReloadCurrentScene; // not called, remove after check

        //CardPartReplaceManager.OnReplacementDone += StartLoadNextScene; // not called, remove after check

        //GatherNewCardManager.OnCardGatherDone += StartLoadFirstScene; // not called, remove after check

        InitScene.OnStart += StartLoadMainMenu;
    }

    private void OnDisable()
    {
        //TDGameManager.OnVictoryComplete -= StartLoadNextScene; // not called, remove after check
        //TDGameManager.OnGameOverComplete -= StartReloadCurrentScene; // not called, remove after check

        //CardPartReplaceManager.OnReplacementDone -= StartLoadNextScene; // not called, remove after check

        //GatherNewCardManager.OnCardGatherDone -= StartLoadFirstScene; // not called, remove after check

        InitScene.OnStart -= StartLoadMainMenu;

    }

    private void Update()
    {
        if (alreadyLoadingNextScene) return;

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

    public void StartLoadNormalGame()
    {
        StartCoroutine(DoLoadScene(LoadNextScene));
    }
    public void StartLoadTutorialGame()
    {
        StartCoroutine(DoLoadScene(LoadTutorialScene));
    }

    //public void StartLoadTutorialScene()
    //{
    //    StartCoroutine(DoLoadSceneWithoutAnimation(LoadNextScene));
    //}
    //private IEnumerator DoLoadSceneWithoutAnimation(LoadSceneFunction loadSceneFunction)
    //{
    //    alreadyLoadingNextScene = true;

        
    //    loadSceneFunction();
    //    yield return new WaitForSeconds(loadSceneDuration);

    //    alreadyLoadingNextScene = false;
    //}



    private void LoadNextScene()
    {
        //SceneManager.LoadScene("MapGenerationTest");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
    }
    private void LoadTutorialScene()
    {
        SceneManager.LoadScene("TutorialMap");
    }

    public void StartLoadMainMenu()
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



    public void LoadMapScene(string sceneName, bool playAnimations)
    {
        StartCoroutine(DoLoadSceneByName(LoadSceneAsyncByName, sceneName, playAnimations));
    }

    public void UnloadMapScene(string sceneName, bool playAnimations)
    {
        StartCoroutine(DoLoadSceneByName(UnloadSceneAsyncByName, sceneName, playAnimations));
    }

    private IEnumerator DoLoadSceneByName(LoadSceneByNameFunction loadSceneByNameFunction, string sceneName, bool playAnimations)
    {
        alreadyLoadingNextScene = true;

        if(playAnimations)
        {
            ShutAnimation(shutAnimDuration);
            GameAudioManager.GetInstance().PlayScreenShut();
            yield return new WaitForSeconds(shutAnimDuration);
        }

        loadSceneByNameFunction(sceneName);
        yield return new WaitForSeconds(loadSceneDuration);

        if(playAnimations)
        {
            OpenAnimation(openAnimDuration);
            GameAudioManager.GetInstance().PlayScreenOpen();
            yield return new WaitForSeconds(openAnimDuration);
        }


        alreadyLoadingNextScene = false;
    }

    private void LoadSceneAsyncByName(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }
    private void UnloadSceneAsyncByName(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
    }

    public void LoadNextScene(string sceneName)
    {
        StartCoroutine(DoLoadSceneByName(LoadSceneAsyncByName, sceneName, true));
    }

}
