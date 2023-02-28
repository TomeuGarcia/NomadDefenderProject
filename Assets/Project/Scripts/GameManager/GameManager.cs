using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("DECK DATA")]
    [SerializeField] protected DeckData starterDeckData;
    [SerializeField] protected DeckData gameDeckData;

    [Header("CANVAS")]
    [SerializeField] protected GameObject victoryHolder;
    [SerializeField] protected GameObject gameOverHolder;

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
        gameDeckData.ReplaceFor(starterDeckData);

        victoryHolder.SetActive(false);
        gameOverHolder.SetActive(false);
    }


    protected virtual void StartVictory()
    {
        victoryHolder.SetActive(true);
        mapSceneLoader.LoadMainMenuScene(3f);
    }
    private void StartGameOver()
    {
        gameOverHolder.SetActive(true);
        mapSceneLoader.LoadMainMenuScene(3f);
    }



}
