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
    }
    
    [SerializeField] private SpeedScale[] timeScales;

    [SerializeField] private TextMeshProUGUI timeSpeedCountText;
    [SerializeField] private Button incrementButton;
    [SerializeField] private Button decrementButton;
    [SerializeField] private GameObject _gamePausedDisplay;
    [SerializeField] private bool _startHidden = false;
    private bool isIncrementButtonHovered = false;
    private bool isDecrementButtonHovered = false;

    private int current = 0;
    private int numSpeeds = 0;
    private bool gameFinished = false;

    private bool _isTimePaused;

    private void Awake()
    {
        numSpeeds = timeScales.Length;
        _isTimePaused = false;
        UpdateTimeSpeed();
        PauseMenu.GameIsPaused = false;

        if (_startHidden)
        {
            gameObject.SetActive(false);
            _gamePausedDisplay.SetActive(false);
        }
    }

    private void OnDestroy()
    {
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
        if (PauseMenu.GameIsPaused || gameFinished) return;

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
            if (_isTimePaused)
            {
                SetCurrentTimeSpeed(0);
            }
            else
            {
                SetCurrentTimeSpeed(3);
            }
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
    }

    private void ResetTimeOnGameEnd()
    {
        gameFinished = true;
        CompletelyDisableTimeSpeed();
    }

    private void UpdateTimeSpeed()
    {
        SpeedScale speedScale = timeScales[current];

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
    }

    private void PauseTimeScale()
    {
        _isTimePaused = true;
        GameTime.SetTimeScale(0);
    }
    private void ResumeTimeScale()
    {
        _isTimePaused = false;
        GameTime.SetTimeScale(1);
    }
    

    public void IncrementTime()
    {
        SetCurrentTimeSpeed((current + 1) % numSpeeds);
        IncrementButtonPressed();
    }
    public void DecrementTime()
    {
        SetCurrentTimeSpeed((current + numSpeeds - 1) % numSpeeds);
        DecrementButtonPressed();
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
        incrementButton.image.DOBlendableColor(Color.white, t).OnComplete(() => { 
            if (isIncrementButtonHovered) {
                incrementButton.image.DOBlendableColor(Color.cyan, t); 
            } 
        }).SetUpdate(UpdateType.Late, true);
    }
    private void DecrementButtonPressed()
    {
        ButtonPressed(decrementButton);

        float t = 0.2f;

        decrementButton.image.DOComplete(true);
        decrementButton.image.DOBlendableColor(Color.white, t).OnComplete(() => {
            if (isDecrementButtonHovered)
            {
                decrementButton.image.DOBlendableColor(Color.cyan, t);
            }
        }).SetUpdate(UpdateType.Late, true);
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



}
