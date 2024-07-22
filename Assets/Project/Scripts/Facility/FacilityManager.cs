using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FacilityManager : MonoBehaviour
{
    [Header("SCENE REFERENCES")]
    [SerializeField] private FacilityPointAndClickManager _facilityPointAndClick;
    [SerializeField] private List<AFacilityInteractable> _startOnInteractables = new();

    private bool _isNewGame;

    private void Awake()
    {
        //TODO - Play Facility Music (if any)

        //TODO - CursorChanger (if FACITY cursor exists)
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
        }
    }

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
}
