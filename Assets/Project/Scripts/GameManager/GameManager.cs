using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("DECK DATA")]
    [SerializeField] protected DecksLibrary decksLibrary;

    [Header("CANVAS")]
    [SerializeField] protected GameObject victoryHolder;
    [SerializeField] protected GameObject gameOverHolder;

    [Header("TEXTS")]
    [SerializeField] protected TextDecoder victoryTitleTextDecoder;
    [SerializeField] protected TextDecoder victorySubtitleTextDecoder;
    [SerializeField] protected TextDecoder gameOverTitleTextDecoder;
    [SerializeField] protected TextDecoder gameOverSubtitleTextDecoder;

    [Header("DEPENDENCIES")]
    [SerializeField] private OverworldMapGameManager overworldMapGameManager;
    [SerializeField] private MapSceneLoader mapSceneLoader;



    private void OnEnable()
    {
        overworldMapGameManager.OnVictory += StartVictory;
        overworldMapGameManager.OnGameOver += StartGameOver;
    }
    private void OnDisable()
    {
        overworldMapGameManager.OnVictory -= StartVictory;
        overworldMapGameManager.OnGameOver -= StartGameOver;
    }


    private void Awake()
    {
        decksLibrary.InitGameDeck();

        victoryHolder.SetActive(false);
        gameOverHolder.SetActive(false);
    }


    protected virtual void StartVictory()
    {
        victoryHolder.SetActive(true);
        //mapSceneLoader.LoadMainMenuScene(3f);
        StartCoroutine(DoStartVictory());
    }
    private IEnumerator DoStartVictory()
    {
        yield return new WaitForSeconds(2.0f);
        victoryTitleTextDecoder.Activate();

        yield return new WaitForSeconds(1.0f);
        victorySubtitleTextDecoder.Activate();


        SceneLoader.GetInstance().StartLoadGameEndCredits();
        GameAudioManager.GetInstance().ChangeMusic(GameAudioManager.MusicType.MENU, 3.0f);
    }


    private void StartGameOver()
    {
        gameOverHolder.SetActive(true);

        StartCoroutine(DoStartGameOver());
    }
    private IEnumerator DoStartGameOver()
    {
        yield return new WaitForSeconds(2.0f);
        gameOverTitleTextDecoder.Activate();

        yield return new WaitForSeconds(1.0f);
        gameOverSubtitleTextDecoder.Activate();

        mapSceneLoader.LoadMainMenuScene(5f);
    }


}
