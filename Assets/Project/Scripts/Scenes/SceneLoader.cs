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


    int init = 0;
    int menu = 1;
    int td1 = 2;
    int td2 = 3;
    int newCard = 4;
    int upgrade1 = 5;
    int upgrade2 = 6;
    int upgrade3 = 7;

    bool lastWasTD2 = false;


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
        TDGameManager.OnVictoryComplete += StartLoadNextScene; // not called, remove after check
        TDGameManager.OnGameOverComplete += StartReloadCurrentScene; // not called, remove after check

        CardPartReplaceManager.OnReplacementDone += StartLoadNextScene; // not called, remove after check

        GatherNewCardManager.OnCardGatherDone += StartLoadNextScene; // not called, remove after check

        InitScene.OnStart += StartLoadMainMenu;
        MainMenu.OnPlayStart += StartLoadNextScene; // TODO change to load map scene
    }

    private void OnDisable()
    {
        TDGameManager.OnVictoryComplete -= StartLoadNextScene; // not called, remove after check
        TDGameManager.OnGameOverComplete -= StartReloadCurrentScene; // not called, remove after check

        CardPartReplaceManager.OnReplacementDone -= StartLoadNextScene; // not called, remove after check

        GatherNewCardManager.OnCardGatherDone -= StartLoadNextScene; // not called, remove after check

        InitScene.OnStart -= StartLoadMainMenu;
        MainMenu.OnPlayStart -= StartLoadNextScene; // TODO change to load map scene
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

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(Random.Range(0, 3) + upgrade1);
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
        int nextSceneI = SceneManager.GetActiveScene().buildIndex;
        int current = SceneManager.GetActiveScene().buildIndex;

        

        if (current <= menu)
        {
            nextSceneI++;
        }
        else if(current <= td2)
        {
            nextSceneI = Random.Range(0, 3) + upgrade1;

            lastWasTD2 = (current == td2);
        }
        else if(current == newCard)
        {
            nextSceneI = (lastWasTD2) ? td1 : td2;
        }
        else
        {
            nextSceneI = newCard;
        }

        SceneManager.LoadScene(nextSceneI);        
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



    public void LoadMapScene(string sceneName)
    {
        StartCoroutine(DoLoadSceneByName(LoadSceneAsyncByName, sceneName));
    }
    public void UnloadMapScene(string sceneName)
    {
        StartCoroutine(DoLoadSceneByName(UnloadSceneAsyncByName, sceneName));
    }

    private IEnumerator DoLoadSceneByName(LoadSceneByNameFunction loadSceneByNameFunction, string sceneName)
    {
        alreadyLoadingNextScene = true;

        ShutAnimation(shutAnimDuration);
        GameAudioManager.GetInstance().PlayScreenShut();
        yield return new WaitForSeconds(shutAnimDuration);

        loadSceneByNameFunction(sceneName);
        yield return new WaitForSeconds(loadSceneDuration);

        OpenAnimation(openAnimDuration);
        GameAudioManager.GetInstance().PlayScreenOpen();
        yield return new WaitForSeconds(openAnimDuration);

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

}
