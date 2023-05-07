using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SpeedUpButton : MonoBehaviour
{
    //[SerializeField] private Image image;
    //[SerializeField] private List<Sprite> sprites = new List<Sprite>();
    [SerializeField] private List<float> timeScales = new List<float>();

    [SerializeField] private TextMeshProUGUI timeSpeedCountText;
    [SerializeField] private Button incrementButton;
    [SerializeField] private Button decrementButton;
    private bool isIncrementButtonHovered = false;
    private bool isDecrementButtonHovered = false;

    private int current = 0;
    private int numSpeeds = 0;


    private void Awake()
    {
        numSpeeds = timeScales.Count;
        UpdateTimeSpeed();
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
            IncrementTime();
        }
    }

    public void ChangeTimeSpeed()
    {
        current = (current + 1) % numSpeeds;

        UpdateTimeSpeed();
    }

    private void ResetTimeOnGameEnd()
    {
        current = 0;
        UpdateTimeSpeed();

        incrementButton.enabled = false;
        decrementButton.enabled = false;
    }

    private void UpdateTimeSpeed()
    {
        Time.timeScale = timeScales[current];
        //image.sprite = sprites[current];
        timeSpeedCountText.text = (current + 1).ToString();
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
