using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FacilityManager : MonoBehaviour
{
    [Header("SCENE REFERENCES")]
    [SerializeField] private FacilityPointAndClickManager _facilityPointAndClick;
    [SerializeField] private FIScreenButton _cardCollectionButton;
    [SerializeField] private FIScreenButton _optionsButton;
    [SerializeField] private FIScreenButton _creditsButton;
    [SerializeField] private List<AFacilityInteractable> _startOnInteractables = new();

    private CursorChanger _cursorChanger;

    private bool _isNewGame;

    private void Awake()
    {
        //TODO - Play Facility Music (if any)

        _cursorChanger = ServiceLocator.GetInstance().CursorChanger;
        PauseMenu.GetInstance().GameCanBePaused = true;
        PauseMenu.GetInstance().CanPauseNormally = false;
        PauseMenu.GetInstance().CanDisplayNewGame = true;


        bool finishedTutorials = TutorialsSaverLoader.GetInstance().IsTutorialDone(Tutorials.BATTLE) &&
                                 TutorialsSaverLoader.GetInstance().IsTutorialDone(Tutorials.OW_MAP);
        if (!finishedTutorials)
        {
            ServiceLocator.GetInstance().RunInfo.SetNewGame(true);
        }
    }

    private void Start()
    {
        if(ServiceLocator.GetInstance().RunInfo.IsNewGame)
        {
            _isNewGame = true;
        }
        else
        {
            _isNewGame = false;

            if(ServiceLocator.GetInstance().RunInfo.ComeFromRun)
            {
                ComeFromRun();
            }
            else
            {
                StartWithOpenSetup();
            }
        }

        bool showCardCollection = !ServiceLocator.GetInstance().RunInfo.IsNewGame;
        _cardCollectionButton.Init(showCardCollection, this);
        _optionsButton.Init(showCardCollection, this);
        _creditsButton.Init(showCardCollection, this);
    }

    private void OnEnable()
    {
        PauseMenu.GetInstance().OnEnterMainMenuOptions += OnEnterMainMenuOptions;
    }

    private void OnDisable()
    {
        PauseMenu.GetInstance().OnEnterMainMenuOptions -= OnEnterMainMenuOptions;
    }

    private void OnDestroy()
    {
        _cursorChanger.RegularCursor();
        ServiceLocator.GetInstance().RunInfo.SetNewGame(false);
        PauseMenu.GetInstance().GameCanBePaused = false;
        PauseMenu.GetInstance().CanPauseNormally = true;
        PauseMenu.GetInstance().CanDisplayNewGame = false;
    }

    /*
    private void Update()
    {
        // SKIP TUTORIAL
        if (Input.GetKeyDown(KeyCode.P) && ServiceLocator.GetInstance().RunInfo.IsNewGame)
        {
            ServiceLocator.GetInstance().RunInfo.SetNewGame(true);
            SceneLoader.GetInstance().LoadFacility();
            TutorialsSaverLoader.GetInstance().SetAllTutorialsDone();
        }
    }
    */


    private void ComeFromRun()
    {
        StartWithOpenSetup();

        //TODO - Add scrpted events, interesting things
        if (ServiceLocator.GetInstance().RunInfo.WonRun)
        {
            //TODO - Cinematic

        }
        else
        {
            //TODO - You Lost

        }
    }

    public void TransitionToNextScene()
    {
        if(_isNewGame)
        {
            SceneLoader.GetInstance().StartLoadTutorialGame();
        }
        else {
            SceneLoader.GetInstance().LoadDeckSelector();
        }
    }
    
    private void StartWithOpenSetup()
    {
        _facilityPointAndClick.IsMultiSocketOn = true;
        _facilityPointAndClick.IsPCOn = true;

        foreach (AFacilityInteractable interactable in _startOnInteractables)
        {
            interactable.InteractedStart();
        }
    }

    
    
    public void TransitionToCardCollection()
    {
        SceneLoader.GetInstance().StartLoadFacilityCardCollection();
    }
    public void TransitionToCredits()
    {
        GameAudioManager.GetInstance().ChangeMusic(GameAudioManager.MusicType.OWMAP, 1f);
        SceneLoader.GetInstance().StartLoadMainMenuCredits();
    }
    public void TransitionToOptions()
    {
        PauseMenu.GetInstance().MainMenuOptions();
    }


    private void OnEnterMainMenuOptions()
    {
        _facilityPointAndClick.SetInteractingLocked(true);
        StartCoroutine(WaitForOptionsExit());
    }
    
    private IEnumerator WaitForOptionsExit()
    {
        yield return new WaitUntil(() => !PauseMenu.GetInstance().ShowingOptions);
        yield return new WaitForSeconds(0.2f);
        _facilityPointAndClick.SetInteractingLocked(false);
    }

}
