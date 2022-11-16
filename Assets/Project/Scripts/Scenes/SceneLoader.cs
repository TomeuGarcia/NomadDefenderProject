using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour
{
    [SerializeField] private RectTransform topBlackScreen;
    private Vector3 topOpenPosition;

    [SerializeField] private RectTransform bottomBlackScreen;
    private Vector3 bottomOpenPosition;



    private void Awake()
    {
        DontDestroyOnLoad(this);

        topOpenPosition = topBlackScreen.position;
        bottomOpenPosition = bottomBlackScreen.position;
    }

    private void OnEnable()
    {
        TDGameManager.OnVictoryComplete += StartLoadNextScene;
        TDGameManager.OnGameOverComplete += StartReloadCurrentScene;

        CardPartReplaceManager.OnReplacementDone += StartLoadNextScene;
    }

    private void OnDisable()
    {
        TDGameManager.OnVictoryComplete -= StartLoadNextScene;
        TDGameManager.OnGameOverComplete -= StartReloadCurrentScene;

        CardPartReplaceManager.OnReplacementDone -= StartLoadNextScene;
    }



    private void ShutAnimation(float duration)
    {
        
    }

    private void OpenAnimation(float duration)
    {

    }



    private void StartLoadNextScene()
    {
        StartCoroutine(DoLoadNextScene());
    }

    private IEnumerator DoLoadNextScene()
    {
        float duration = 0.3f;

        ShutAnimation(duration);
        yield return new WaitForSeconds(1f);

        LoadNextScene();
        yield return new WaitForSeconds(1f);
        
        OpenAnimation(duration);
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
            SceneManager.LoadScene(0); // Main menu
        }
        
    }

    private void StartReloadCurrentScene()
    {
        StartCoroutine(DoReloadCurrentScene());
    }

    private IEnumerator DoReloadCurrentScene()
    {
        float duration = 0.3f;

        ShutAnimation(duration);
        yield return new WaitForSeconds(1f);

        ReloadCurrentScene();
        yield return new WaitForSeconds(1f);

        OpenAnimation(duration);
    }

    private void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }



}
