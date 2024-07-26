using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    [Header("DECK DATA")]
    [SerializeField] protected DecksLibrary decksLibrary;

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



    private void OnEnable()
    {
        overworldMapGameManager.OnVictory += StartVictory;
        overworldMapGameManager.OnGameOver += StartGameOver;
    }
    private void OnDisable()
    {
        overworldMapGameManager.OnVictory -= StartVictory;
        overworldMapGameManager.OnGameOver -= StartGameOver;

        PauseMenu.GetInstance().gameCanBePaused = true;
    }


    private void Awake()
    {
        decksLibrary.InitGameDeck();

        victoryHolder.SetActive(false);
        gameOverHolder.SetActive(false);

        PauseMenu.GetInstance().gameCanBePaused = true;
    }


    protected virtual void StartVictory()
    {
        victoryHolder.SetActive(true);
        //mapSceneLoader.LoadMainMenuScene(3f);
        StartCoroutine(DoStartVictory());

        StarterDecksUnlocker.GetInstance().UnlockNextDeck();
    }
    private IEnumerator DoStartVictory()
    {
        PauseMenu.GetInstance().gameCanBePaused = false;

        yield return new WaitForSeconds(2.0f);        
        victoryTitleTextDecoder.Activate();

        yield return new WaitForSeconds(1.0f);
        victorySubtitleTextDecoder.Activate();
        yield return new WaitForSeconds(4f);

        GameAudioManager.GetInstance().MusicFadeOut(0.5f);
        

        cgVictoryHolder.DOFade(0f, 0.25f);
        GameAudioManager.GetInstance().PlayRandomGlitchSound();
        globalVolume.profile = glitchVol;
        yield return new WaitForSeconds(0.2f);
        globalVolume.profile = initVol;
        yield return new WaitForSeconds(1.0f);


        yield return StartCoroutine(VictoryWatcherScripedSequence());


        for (int i = 0; i < 3; ++i)
        {
            GameAudioManager.GetInstance().PlayRandomGlitchSound();
            globalVolume.profile = glitchVol;
            yield return new WaitForSeconds(Random.Range(0.2f, 0.3f));
            globalVolume.profile = initVol;
        }
        yield return new WaitForSeconds(0.2f);

        SceneLoader.GetInstance().StartLoadGameEndCredits();
        GameAudioManager.GetInstance().ChangeMusic(GameAudioManager.MusicType.MENU, 2.0f);
    }


    private void StartGameOver()
    {
        gameOverHolder.SetActive(true);

        StartCoroutine(DoStartGameOver());
    }
    private IEnumerator DoStartGameOver()
    {
        PauseMenu.GetInstance().gameCanBePaused = false;

        yield return new WaitForSeconds(2.0f);
        gameOverTitleTextDecoder.Activate();

        yield return new WaitForSeconds(1.0f);
        gameOverSubtitleTextDecoder.Activate();

        yield return new WaitForSeconds(4.0f);
        mapSceneLoader.LoadMainMenuScene(1f);

        for (int i = 0; i < 3; ++i)
        {
            GameAudioManager.GetInstance().PlayRandomGlitchSound();
            globalVolume.profile = glitchVol;
            yield return new WaitForSeconds(Random.Range(0.2f, 0.3f));
            globalVolume.profile = initVol;
        }
        
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


}
