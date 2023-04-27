using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    float lastTimeScale;
    [SerializeField] GameObject pauseMenuUI;
    public bool gameCanBePaused;
    [SerializeField] GameObject textManager;
    // Update is called once per frame

    private Button button;
    private TextMeshProUGUI buttonText;
    [SerializeField] private TextMeshProUGUI titleText;

    private static Color fadedInColor = Color.white;
    private static Color fadedOutColor = new Color(0.6f, 0.6f, 0.6f);
    private static Color disabledColor = new Color(0.15f, 0.15f, 0.15f);

    static PauseMenu instance;
    private void Start()
    {
        //textManager.SetActive(false);
        pauseMenuUI.SetActive(false);
        GameAudioManager.GetInstance().PlayMusic1();
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
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (GameAudioManager.GetInstance().isMusicPaused())
            {
                GameAudioManager.GetInstance().PlayMusic1();
            }
            else
            {
                GameAudioManager.GetInstance().PauseMusic1();
            }
        }

    }


    public void Resume()
    {

        // textManager.SetActive(false);
        //Camera.main.GetComponent<OWCameraMovement>().CanDrag(true);
        pauseMenuUI.SetActive(false);
        GameAudioManager.GetInstance().NormalMusicPitch();
        Time.timeScale = 1;   //segurament s ha de fer d un altre manera
        GameIsPaused = false;
        textManager.GetComponent<TextManager>().ResetTexts();
    }

    void Pause()
    {
        if (gameCanBePaused)
        {
            if (button != null)
            {
                buttonText.rectTransform.DOScale(Vector3.one, 0f).SetUpdate(true);
                button.image.color = fadedInColor;
                buttonText.color = fadedInColor;
            }
            //Camera.main.GetComponent<OWCameraMovement>().CanDrag(false);//accedir a cameraMovement per quan estic en batalla
            StartCoroutine(textManager.GetComponent<TextManager>().DecodeTexts());
            EventSystem.current.SetSelectedGameObject(null);
            GameAudioManager.GetInstance().PausedMusicPitch();
            //textManager.SetActive(true);
            pauseMenuUI.SetActive(true);
            lastTimeScale = Time.timeScale;
            Time.timeScale = 0;   //segurament s ha de fer d un altre manera
            GameIsPaused = true;
            TextFadeIn(titleText);
        }
    }

    public void LoadMenu()
    {
        GameAudioManager.GetInstance().ChangeMusic(GameAudioManager.MusicType.MENU, 0.01f);
        Time.timeScale = 1;
        SceneLoader.GetInstance().StartLoadMainMenu();
        pauseMenuUI.SetActive(false);
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
}
