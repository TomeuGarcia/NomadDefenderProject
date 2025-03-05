using System;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeedUpButton : MonoBehaviour
{
    [System.Serializable]
    public struct SpeedScale
    {
        [SerializeField] public int timeMultiplier;
        [SerializeField] public string text;
        [SerializeField] public bool isBuggy;
    }
    
    [SerializeField] private SpeedScale[] timeScales;

    [SerializeField] private TextMeshProUGUI timeSpeedCountText;
    [SerializeField] private TextMeshProUGUI timeSpeedCountLeftText;
    [SerializeField] private TextMeshProUGUI timeSpeedCountRightText;
    [SerializeField] private Button incrementButton;
    [SerializeField] private Button decrementButton;
    [SerializeField] private GameObject _gamePausedDisplay;
    [SerializeField] private bool _startHidden = false;
    private bool isIncrementButtonHovered = false;
    private bool isDecrementButtonHovered = false;

    private int current = 0;
    private int numSpeeds = 0;
    private bool gameFinished = false;
    private bool _inputsAreDisabled = false;

    public bool IsTimePaused { get; private set; }
    public static bool UsingBuggyTimeScale { get; private set; }

    public static Action OnGameSpeedInteracted;
    
    public static SpeedUpButton Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        InitNumSpeed();
        IsTimePaused = false;
        UpdateTimeSpeed();
        PauseMenu.GameIsPaused = false;

        if (_startHidden)
        {
            _gamePausedDisplay.SetActive(false);
        }
    }

    private IEnumerator Start()
    {
        yield return null;
        UpdateTimeSpeed();
        
        if (_startHidden)
        {
            gameObject.SetActive(false);
            _gamePausedDisplay.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        Instance = null;
        Time.timeScale = 1.0f;
    }

    private void OnEnable()
    {
        TDGameManager.OnGameFinishStart += ResetTimeOnGameEnd;

        LastEnemyKIllAnimation.OnQueryResumeTimescale += UpdateTimeSpeed;
    }
    private void OnDisable()
    {
        TDGameManager.OnGameFinishStart -= ResetTimeOnGameEnd;

        LastEnemyKIllAnimation.OnQueryResumeTimescale -= UpdateTimeSpeed;
    }

    private void Update()
    {
        if (PauseMenu.GameIsPaused || gameFinished || _inputsAreDisabled) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetCurrentTimeSpeed(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetCurrentTimeSpeed(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetCurrentTimeSpeed(2);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsTimePaused)
            {
                SetCurrentTimeSpeed(0);
            }
            else
            {
                SetCurrentTimeSpeed(3);
            }
            OnGameSpeedInteracted?.Invoke();
        }
        
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            IncrementTime();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            DecrementTime();
        }
    }

    public void InitNumSpeed()
    {
        numSpeeds = timeScales.Length;
    }
    
    public void ChangeTimeSpeed()
    {
        current = (current + 1) % numSpeeds;

        UpdateTimeSpeed();
    }

    public void CompletelyDisableTimeSpeed()
    {
        current = 0;
        UpdateTimeSpeed();
        _gamePausedDisplay.SetActive(false); // Don't see that the game is paused
        
        incrementButton.enabled = false;
        decrementButton.enabled = false;
        
        gameObject.SetActive(false);
        _inputsAreDisabled = true;
    }

    public void CompletelyEnableTimeSpeed()
    {
        current = 0;
        UpdateTimeSpeed();
        _gamePausedDisplay.SetActive(true); // Make visible that the game is paused

        incrementButton.enabled = true;
        decrementButton.enabled = true;
        
        gameObject.SetActive(true);
        _inputsAreDisabled = false;
    }

    private void ResetTimeOnGameEnd()
    {
        gameFinished = true;
        CompletelyDisableTimeSpeed();
    }

    private void UpdateTimeSpeed()
    {
        SpeedScale speedScale = timeScales[current];
        SpeedScale previousSpeedScale = timeScales[((current - 1) + numSpeeds) % numSpeeds];
        SpeedScale nextSpeedScale = timeScales[(current + 1) % numSpeeds];

        bool wantsToPauseGame = speedScale.timeMultiplier == 0;
        if (wantsToPauseGame)
        {
            PauseTimeScale();
            Time.timeScale = 1;
        }
        else
        {
            ResumeTimeScale();
            Time.timeScale = speedScale.timeMultiplier;
        }
        
        _gamePausedDisplay.SetActive(wantsToPauseGame);

        timeSpeedCountText.text = speedScale.text;
        timeSpeedCountLeftText.text = previousSpeedScale.text;
        timeSpeedCountRightText.text = nextSpeedScale.text;

        UsingBuggyTimeScale = speedScale.isBuggy;
    }

    private void PauseTimeScale()
    {
        IsTimePaused = true;
        GameTime.SetTimeScale(0);
    }
    private void ResumeTimeScale()
    {
        IsTimePaused = false;
        GameTime.SetTimeScale(1);
    }
    

    public void IncrementTime()
    {
        SetCurrentTimeSpeed((current + 1) % numSpeeds);
        IncrementButtonPressed();
        OnGameSpeedInteracted?.Invoke();
    }
    public void DecrementTime()
    {
        SetCurrentTimeSpeed((current + numSpeeds - 1) % numSpeeds);
        DecrementButtonPressed();
        OnGameSpeedInteracted?.Invoke();
    }

    public void SetDefaultTimeSpeed()
    {
        SetCurrentTimeSpeed(0);
    }
    private void SetCurrentTimeSpeed(int newTimeSpeed)
    {
        current = newTimeSpeed;

        UpdateTimeSpeed();
        
        GameAudioManager.GetInstance().PlayCardInfoMoveHidden();
    }

    private void ButtonPressed(Button button)
    {
        button.transform.DOComplete();
        button.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 5).SetUpdate(UpdateType.Late, true);        
    }

    private void IncrementButtonPressed()
    {
        ButtonPressed(incrementButton);

        float t = 0.2f;

        incrementButton.image.DOComplete(true);
        incrementButton.image.DOColor(Color.white, t).OnComplete(() => { 
            if (isIncrementButtonHovered) {
                incrementButton.image.DOColor(Color.cyan, t); 
            } 
        }).SetUpdate(true);
    }
    private void DecrementButtonPressed()
    {
        ButtonPressed(decrementButton);

        float t = 0.2f;

        decrementButton.image.DOComplete(true);
        decrementButton.image.DOColor(Color.white, t).OnComplete(() => {
            if (isDecrementButtonHovered)
            {
                decrementButton.image.DOColor(Color.cyan, t);
            }
        }).SetUpdate(true);
    }


    public void IncrementButtonHovered()
    {
        ButtonHovered(incrementButton);
        isIncrementButtonHovered = true;
    }
    public void DecrementButtonHovered()
    {
        ButtonHovered(decrementButton);
        isDecrementButtonHovered = true;
    }
    private void ButtonHovered(Button button)
    {
        button.image.color = Color.cyan;
    }


    public void IncrementButtonUnhovered()
    {
        ButtonUnhovered(incrementButton);
        isIncrementButtonHovered = false;
    }
    public void DecrementButtonUnhovered()
    {
        ButtonUnhovered(decrementButton);
        isDecrementButtonHovered = false;
    }
    private void ButtonUnhovered(Button button)
    {
        button.image.color = Color.white;
    }




    public void SetSpeedTo0AndDisableInteractions()
    {
        // disable buttons interactions & update() inputs
        incrementButton.enabled = false;
        decrementButton.enabled = false;
        _inputsAreDisabled = true;

        // if paused, exit from paused
        _gamePausedDisplay.SetActive(false);


        // in the end, set GameSpeed to 0
        current = 0;
        PauseTimeScale();
        Time.timeScale = 1;
    }
    public void ResumeSpeedAndEnableInteractions()
    {
        // enable buttons interactions & update() inputs
        incrementButton.enabled = true;
        decrementButton.enabled = true;
        _inputsAreDisabled = false;

        // in the end, set GameSpeed to 1 / return to default speed
        SetCurrentTimeSpeed(0);
    }
    
}
