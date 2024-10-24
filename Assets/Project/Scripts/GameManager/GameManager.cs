using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    [Header("DECK DATA")]
    [SerializeField] protected DecksLibrary decksLibrary;

    [Header("TROPHIES")] 
    [SerializeField] private CardDeckInUseData _cardDeckInUseData;
    [SerializeField] private UnlockableTrophiesManager _unlockableTrophiesManager;

    [Header("CANVAS")]
    [SerializeField] protected GameObject victoryHolder;
    [SerializeField] protected CanvasGroup cgVictoryHolder;
    [SerializeField] protected GameObject gameOverHolder;
    [SerializeField] private ScriptedSequence victoryScriptedSequence;

    [Header("TEXTS")]
    [SerializeField] protected TextDecoder victoryTitleTextDecoder;
    [SerializeField] protected TextDecoder victorySubtitleTextDecoder;
    [SerializeField] protected TextDecoder gameOverTitleTextDecoder;
    [SerializeField] protected TextDecoder gameOverSubtitleTextDecoder;

    [Header("DEPENDENCIES")]
    [SerializeField] private OverworldMapGameManager overworldMapGameManager;
    [SerializeField] private MapSceneLoader mapSceneLoader;

    [Header("GLITCH")]
    [SerializeField] private Volume globalVolume;
    [SerializeField] private VolumeProfile initVol;
    [SerializeField] private VolumeProfile glitchVol;
    [SerializeField] private GameObject _watcherFace;
    [SerializeField] private Camera _mapCamera;

    private void OnEnable()
    {
        overworldMapGameManager.OnVictory += StartVictory;
        overworldMapGameManager.OnGameOver += StartGameOver;
    }
    private void OnDisable()
    {
        overworldMapGameManager.OnVictory -= StartVictory;
        overworldMapGameManager.OnGameOver -= StartGameOver;

        PauseMenu.GetInstance().GameCanBePaused = true;
    }


    private void Awake()
    {
        decksLibrary.InitGameDeck();

        victoryHolder.SetActive(false);
        gameOverHolder.SetActive(false);

        PauseMenu.GetInstance().GameCanBePaused = true;
        
        _watcherFace.SetActive(false);
    }

    [Button()]
    protected virtual void StartVictory()
    {
        victoryHolder.SetActive(true);
        //mapSceneLoader.LoadMainMenuScene(3f);
        StartCoroutine(DoStartVictory());

        UnlockVictoryContent();
    }

    private void UnlockVictoryContent()
    {
        StarterDecksUnlocker.GetInstance().UnlockNextDeck();
        _unlockableTrophiesManager.Unlock(_cardDeckInUseData.WinTrophyModel);
    }
    
    
    private IEnumerator DoStartVictory()
    {
        PauseMenu.GetInstance().GameCanBePaused = false;

        yield return new WaitForSeconds(2.0f);        
        victoryTitleTextDecoder.Activate();

        yield return new WaitForSeconds(1.0f);
        victorySubtitleTextDecoder.Activate();
        yield return new WaitForSeconds(4f);

        GameAudioManager.GetInstance().MusicFadeOut(0.5f);
        

        cgVictoryHolder.DOFade(0f, 0.25f);
        GameAudioManager.GetInstance().PlayRandomGlitchSound();
        globalVolume.profile = glitchVol;
        StartCoroutine(MakeWatcherFaceVisibleForDuration(0.15f));
        yield return new WaitForSeconds(0.2f);
        globalVolume.profile = initVol;
        yield return new WaitForSeconds(1.0f);


        yield return StartCoroutine(VictoryWatcherScripedSequence());


        for (int i = 0; i < 3; ++i)
        {
            GameAudioManager.GetInstance().PlayRandomGlitchSound();
            globalVolume.profile = glitchVol;
            float glitchDuration = Random.Range(0.15f, 0.25f);
            yield return new WaitForSeconds(glitchDuration);
            globalVolume.profile = initVol;
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.2f);

        SceneLoader.GetInstance().StartLoadGameEndCredits();
        GameAudioManager.GetInstance().ChangeMusic(GameAudioManager.MusicType.MENU, 2.0f);
    }

    [Button()]
    private void StartGameOver()
    {
        gameOverHolder.SetActive(true);

        StartCoroutine(DoStartGameOver());
    }
    private IEnumerator DoStartGameOver()
    {
        PauseMenu.GetInstance().GameCanBePaused = false;

        yield return new WaitForSeconds(2.0f);
        gameOverTitleTextDecoder.Activate();

        yield return new WaitForSeconds(1.0f);
        gameOverSubtitleTextDecoder.Activate();

        yield return new WaitForSeconds(4.0f);
        //mapSceneLoader.LoadMainMenuScene(1f);
        GameOverFinishLoadScene();

        gameOverHolder.SetActive(false);
        for (int i = 0; i < 3; ++i)
        {
            GameAudioManager.GetInstance().PlayRandomGlitchSound();
            globalVolume.profile = glitchVol;
            float glitchDuration = Random.Range(0.15f, 0.25f);
            StartCoroutine(MakeWatcherFaceVisibleForDuration(glitchDuration-0.05f));
            yield return new WaitForSeconds(glitchDuration);
            globalVolume.profile = initVol;
            yield return new WaitForSeconds(0.1f);
        }
        
    }

    private void GameOverFinishLoadScene()
    {
        mapSceneLoader.LoadMainMenuScene(0.85f);
    }


    private IEnumerator VictoryWatcherScripedSequence()
    {
        // 0
        victoryScriptedSequence.NextLine();
        yield return new WaitUntil(() => victoryScriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1f);
        // 1
        victoryScriptedSequence.NextLine();
        yield return new WaitUntil(() => victoryScriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(3f);

        // 2
        victoryScriptedSequence.Clear();
        victoryScriptedSequence.NextLine();
        yield return new WaitUntil(() => victoryScriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1f);
        // 3
        victoryScriptedSequence.NextLine();
        yield return new WaitUntil(() => victoryScriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(3f);

        // 4
        victoryScriptedSequence.Clear();
        victoryScriptedSequence.NextLine();
        yield return new WaitUntil(() => victoryScriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1f);
        // 5
        victoryScriptedSequence.NextLine();
        yield return new WaitUntil(() => victoryScriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(3f);

        // 6
        victoryScriptedSequence.Clear();
        victoryScriptedSequence.NextLine();
        yield return new WaitUntil(() => victoryScriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1f);
        // 7
        victoryScriptedSequence.NextLine();
        yield return new WaitUntil(() => victoryScriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(3f);

    }


    [Button()]
    public void TESTMakeWatcherFaceVisibleForDuration()
    {
       StartCoroutine(MakeWatcherFaceVisibleForDuration(0.2f));
    }
    private IEnumerator MakeWatcherFaceVisibleForDuration(float duration)
    {
        _watcherFace.SetActive(true);

        Vector3 cameraForward = _mapCamera.transform.forward;
        Vector3 cameraProjectedForward = Vector3.ProjectOnPlane(cameraForward, Vector3.up);
        Vector3 position = _mapCamera.transform.position + 
                           (3.5f * cameraProjectedForward) +
                           (-2.25f * Vector3.up);

        _watcherFace.transform.position = position;
        _watcherFace.transform.forward = cameraForward;
        _watcherFace.transform.localScale = Vector3.one * 0.09f;
        
        yield return new WaitForSeconds(duration);
        _watcherFace.SetActive(false);
    }

}
