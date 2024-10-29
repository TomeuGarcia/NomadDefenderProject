using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Serialization;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    float lastTimeScale;
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] private CanvasGroup interactionCanvasGroup;
    public bool GameCanBePaused { get; set; } = true;
    [SerializeField] TextManager textManager;

    private Button button;
    private TextMeshProUGUI buttonText;
    [SerializeField] private TextMeshProUGUI titleText;

    [SerializeField] private TextMeshProUGUI mainMenuButtonText;
    [SerializeField] private TextMeshProUGUI surrenderText;


    [SerializeField] private OptionsMenu optionsMenuUI;
    


    private static Color fadedInColor = Color.white;
    private static Color fadedOutColor = new Color(0.6f, 0.6f, 0.6f);
    private static Color disabledColor = new Color(0.15f, 0.15f, 0.15f);

    static PauseMenu instance;

    public static Action OnGameSurrender;


    private void Start()
    {
        //textManager.SetActive(false);
        optionsMenuUI.Init();

        optionsMenuUI.gameObject.SetActive(false);
        pauseMenuUI.SetActive(false);
        surrenderText.gameObject.SetActive(false);
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    public static PauseMenu GetInstance()
    {
        return instance;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }


    public void Resume()
    {

        // textManager.SetActive(false);
        //Camera.main.GetComponent<OWCameraMovement>().CanDrag(true);
        pauseMenuUI.SetActive(false);
        
        if (optionsMenuUI.gameObject.activeInHierarchy)
        {
            optionsMenuUI.Hide();
            optionsMenuUI.gameObject.SetActive(false);
        }


        //GameAudioManager.GetInstance().NormalMusicPitch();
        Time.timeScale = lastTimeScale;   //segurament s ha de fer d un altre manera
        GameIsPaused = false;
        textManager.ResetTexts();        

        OWMap_Node.IsGlobalInteractable = true;
    }

    void Pause()
    {
        if (!GameCanBePaused) return;

        if (button != null)
        {
            buttonText.rectTransform.DOScale(Vector3.one, 0f).SetUpdate(true);
            button.image.color = fadedInColor;
            buttonText.color = fadedInColor;
        }
        //Camera.main.GetComponent<OWCameraMovement>().CanDrag(false);//accedir a cameraMovement per quan estic en batalla
        StartCoroutine(textManager.DecodeTextsWithDelay());
        EventSystem.current.SetSelectedGameObject(null);
        //GameAudioManager.GetInstance().PausedMusicPitch();
        //textManager.SetActive(true);
        pauseMenuUI.SetActive(true);
        lastTimeScale = Time.timeScale;
        Time.timeScale = 0;   //segurament s ha de fer d un altre manera
        GameIsPaused = true;
        TextFadeIn(titleText);

        interactionCanvasGroup.interactable = true;

        OWMap_Node.IsGlobalInteractable = false;

        CardTooltipDisplayManager.GetInstance().StopDisplayingTooltip();
    }

    public void MainMenuOptions()
    {
        GoToOptionsMenu();
    }

    public void LoadMenu()
    {
        OnGameSurrender?.Invoke();
        
        GameAudioManager.GetInstance().ChangeMusic(GameAudioManager.MusicType.MENU, 1f);
        Time.timeScale = 1;

        //HideUI();
        
        interactionCanvasGroup.interactable = false;

        OWMap_Node.IsGlobalInteractable = true;
        textManager.ResetTexts();

        SceneLoader.GetInstance().StartLoadMainMenu();
    }

    public void HideUI()
    {
        pauseMenuUI.SetActive(false);
        mainMenuButtonText.gameObject.SetActive(true);
        surrenderText.gameObject.SetActive(false);
    }

    private void TextFadeIn(TextMeshProUGUI text, bool onEndFadeOut = true)
    {
        if (!GameIsPaused) { return; }
        text.rectTransform.DOScale(Vector3.one * 1.05f, 2f).SetUpdate(true).OnComplete(() => { if (onEndFadeOut) TextFadeOut(titleText); }); ;
        //text.DOBlendableColor(fadedInColor, 1.0f).SetUpdate(true);
    }

    private void TextFadeOut(TextMeshProUGUI text, bool onEndFadeIn = true)
    {
        text.rectTransform.DOScale(Vector3.one * 1f, 2f).SetUpdate(true).OnComplete(() => { if (onEndFadeIn) TextFadeIn(titleText); }); ;
        //text.DOBlendableColor(fadedOutColor, 1.0f).SetUpdate(true);
    }

    //private void StopButtonFade(Button button, TextMeshProUGUI buttonText, bool goToFadedOut)
    //{
    //    button.transform.DOKill();
    //    button.image.DOKill();
    //    buttonText.DOKill();

    //    if (goToFadedOut && button.interactable)
    //    {
    //        ButtonFadeOut(button, buttonText, false);
    //    }
    //}
    public void ButtonHovered(Button _button)
    {
        //StopButtonFade(finishRedrawsButton, finishRedrawsButtonText, false);
        button = _button;
        buttonText = button.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        button.image.DOBlendableColor(Color.cyan, 0.1f).SetUpdate(true);
        buttonText.DOBlendableColor(Color.cyan, 0.1f).SetUpdate(true);
        buttonText.rectTransform.DOScale(Vector3.one * 1.1f, 0.1f).SetUpdate(true);
        GameAudioManager.GetInstance().PlayCardInfoShown();
    }
    public void ButtonUnhovered()
    {
        button.transform.DOKill();
        button.image.DOKill();
        buttonText.DOKill();
        button.image.color = fadedInColor;
        buttonText.color = fadedInColor;
        buttonText.rectTransform.DOScale(Vector3.one, 0.1f).SetUpdate(true);

        //ButtonFadeOut(finishRedrawsButton, finishRedrawsButtonText, true);
        GameAudioManager.GetInstance().PlayCardInfoHidden();
    }


    public void MainMenuHoverShowSurrender()
    {
        mainMenuButtonText.gameObject.SetActive(false);
        surrenderText.gameObject.SetActive(true);
    }
    public void MainMenuUnhoverHideSurrender()
    {
        mainMenuButtonText.gameObject.SetActive(true);
        surrenderText.gameObject.SetActive(false);
    }

    public void GoToOptionsMenu()
    {
        optionsMenuUI.gameObject.SetActive(true);
        optionsMenuUI.Show();
    }

    public void LeaveOptionsMenu()
    {
        ButtonUnhovered();
        optionsMenuUI.Hide();
        optionsMenuUI.gameObject.SetActive(false);
    }

}
