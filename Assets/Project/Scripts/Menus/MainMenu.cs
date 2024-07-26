using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [Header("BUTTONS")]
    [SerializeField] private GameObject playButtonGO;
    [SerializeField] private GameObject newGameButton;
    [SerializeField] private GameObject optionsButton;
    [SerializeField] private GameObject creditsButton;
    [SerializeField] private GameObject quitButton;

    [Header("BUTTONS")]
    [SerializeField] private TextMeshProUGUI playButtonText;
    [SerializeField] private TextMeshProUGUI newGameButtonText;
    [SerializeField] private TextMeshProUGUI creditsButtonText;
    [SerializeField] private TextMeshProUGUI optionsButtonText;
    [SerializeField] private TextMeshProUGUI quitButtonText;


    [Header("TEXT DECODERS")]
    [SerializeField] private TextManager textDecoderManager;
    [SerializeField] private TextDecoder titleTextDecoder;
    [SerializeField] private TextDecoder newGameButtonTextDecoder;
    [SerializeField] private TextDecoder creditsButtonDecoder;
    [SerializeField] private TextDecoder optionsButtonTextDecoder;
    [SerializeField] private TextDecoder playButtonTextDecoder;
    [SerializeField] private TextDecoder quitTextDecoder;

    [Header("STEAM")]
    [SerializeField, Min(0)] private float _steamStartDelay = 2.5f;
    [SerializeField] private CanvasGroup _steamCanvasGroup;
    [SerializeField] private TextDecoder _steamDecoder;

    [Header("UPCOMING CHANGES")]
    [SerializeField, Min(0)] private float _upcomingChangesStartDelay = 2.5f;
    [SerializeField] private CanvasGroup _upcomingChangesCanvasGroup;
    [SerializeField] private TextDecoder _upcomingChangesDecoder;


    [Header("MATERIALS SETUP")]
    [SerializeField] private Material obstacleTilesMaterial;
    [SerializeField] private Material tilesMaterial;
    [SerializeField] private Material outerPlanesMaterial;

    [Header("COMING CHANGES")]
    [SerializeField] private ComingChangesMenu _comingChangesMenu;


    private int titleClickCount = 0;
    private string originalTitleString;

    private bool canInteract = true;

    private bool skipFirstBattle = false;

    public const string STEAM_WISHLIST_LINK = "https://store.steampowered.com/app/2712740/Nomad_Defender/?l=english";


    private void Awake()
    {
        canInteract = true;

        bool finishedTutorails = TutorialsSaverLoader.GetInstance().IsTutorialDone(Tutorials.BATTLE) &&
                                 TutorialsSaverLoader.GetInstance().IsTutorialDone(Tutorials.OW_MAP);
        if (!finishedTutorails)
        {
            playButtonGO.SetActive(false);
            float offset = 0.3f;

            newGameButton.GetComponent<RectTransform>().position -= Vector3.right * (offset + 0.1f);
            quitTextDecoder.GetComponent<RectTransform>().position -= Vector3.right * offset;
            optionsButton.GetComponent<RectTransform>().position += Vector3.right * offset;
            creditsButtonDecoder.GetComponent<RectTransform>().position += Vector3.right * offset;
        }

        SetupTextDecoderManager();

        obstacleTilesMaterial.SetFloat("_ErrorWiresStep", 0f);
        obstacleTilesMaterial.SetFloat("_AdditionalErrorWireStep2", 0f);
        tilesMaterial.SetFloat("_ErrorWiresStep", 0f);
        tilesMaterial.SetFloat("_AdditionalErrorWireStep2", 0f);
        outerPlanesMaterial.SetFloat("_ErrorWiresStep", 0f);
        outerPlanesMaterial.SetFloat("_AdditionalErrorWireStep2", 0f);

        PauseMenu.GameIsPaused = false;
        PauseMenu.GetInstance().HideUI();

        StartCoroutine(PlayShowSteamAnimation());
    }
    private void Start()
    {
        originalTitleString = titleTextDecoder.textStrings[0];
        PauseMenu.GetInstance().gameCanBePaused = false;
        GameAudioManager.GetInstance().ChangeMusic(GameAudioManager.MusicType.MENU, 0.2f);
    }

    

    private void SetupTextDecoderManager()
    {
        List<TextDecoder> textDecoders = new List<TextDecoder>();
        textDecoders.Add(titleTextDecoder);
        if (playButtonGO.activeInHierarchy) { textDecoders.Add(playButtonTextDecoder); }
        textDecoders.Add(newGameButtonTextDecoder);
        textDecoders.Add(optionsButtonTextDecoder);
        textDecoders.Add(quitTextDecoder);
        textDecoders.Add(creditsButtonDecoder);

        textDecoderManager.SetTextDecoders(textDecoders);
        //textDecoderManager.ResetTexts();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            skipFirstBattle = true;
            ////Reset All Tutorials
            //TutorialsSaverLoader.GetInstance().ResetTutorials();
            //Debug.Log("All Tutorials Have Been Reset");
            //playButtonGO.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.P) && canInteract)
        {
            TutorialsSaverLoader.GetInstance().SetAllTutorialsDone();            
            Play();
        }
    }

    public void PlayNewGame()
    {
        if (!canInteract) return;

        canInteract = false;
        //PauseMenu.GetInstance().gameCanBePaused = true;

        ButtonClickedPunch(newGameButtonText);

        GameAudioManager.GetInstance().ChangeMusic(GameAudioManager.MusicType.OWMAP, 1f);

        if (skipFirstBattle)
        {
            TutorialsSaverLoader.GetInstance().ResetTutorialsExceptFirstBattle();
        }
        else
        {
            TutorialsSaverLoader.GetInstance().ResetTutorials();
        }

        ServiceLocator.GetInstance().RunInfo.SetNewGame(true);
        StartCoroutine(DoPlay());
    }

    public void Play()
    {
        if (!canInteract) return;

        canInteract = false;

        ButtonClickedPunch(playButtonText);

        //PauseMenu.GetInstance().gameCanBePaused = true;

        ServiceLocator.GetInstance().RunInfo.SetNewGame(false);
        StartCoroutine(DoPlay());
    }
    private IEnumerator DoPlay()
    {
        yield return new WaitForSeconds(0.3f);

        ServiceLocator.GetInstance().RunInfo.SetComeFromRun(false);
        SceneLoader.GetInstance().LoadFacility();
    }

    public void Credits()
    {
        if (!canInteract) return;

        canInteract = false;


        ButtonClickedPunch(creditsButtonText);

        GameAudioManager.GetInstance().ChangeMusic(GameAudioManager.MusicType.OWMAP, 1f);
        SceneLoader.GetInstance().StartLoadMainMenuCredits();
    }

    public void Options()
    {
        if (!canInteract) return;

        //canInteract = false;


        ButtonClickedPunch(optionsButtonText);

        PauseMenu.GetInstance().MainMenuOptions();
    }

    public void Title()
    {
        if (!canInteract) return;

        titleClickCount++;
        titleClickCount %= 10;

        titleTextDecoder.ClearDecoder();
        if(titleClickCount == 3)
        {
            titleTextDecoder.SetTextStrings("NOMAD ATTACKER");
            StartCoroutine(BackToDefender());
        }else if(titleClickCount == 4)
        {
            titleTextDecoder.SetTextStrings(originalTitleString);
        }
        titleTextDecoder.Activate();
    }

    private IEnumerator BackToDefender()
    {
        yield return new WaitForSeconds(3.0f);
        if(titleClickCount == 3)
        {
            titleTextDecoder.ClearDecoder();
            titleTextDecoder.SetTextStrings(new List<string> { originalTitleString });
            titleTextDecoder.Activate();
        }
    }

    public void Quit()
    {
        if (!canInteract) return;

        ButtonClickedPunch(quitButtonText);

        Application.Quit();
    }

    public void ButtonHovered()
    {
        GameAudioManager.GetInstance().PlayCardInfoShown();
    }

    public void ButtonUnhovered()
    {
        GameAudioManager.GetInstance().PlayCardInfoHidden();
    }

    private void ButtonClickedPunch(TextMeshProUGUI buttonText)
    {
        float duration = 0.5f;

        Sequence punchSequence = DOTween.Sequence();
        punchSequence.Append(buttonText.rectTransform.DOPunchScale(Vector3.one * 0.4f, duration, 6));

        //int numBlinks = 3;
        //float blinkDuration = duration / (numBlinks*10f);
        //for (int i = 0; i < numBlinks; ++i)
        //{
        //    punchSequence.Join(buttonText.DOFade(0.0f, blinkDuration));
        //    punchSequence.AppendInterval(blinkDuration);
        //    punchSequence.Join(buttonText.DOFade(1.0f, blinkDuration));
        //    punchSequence.AppendInterval(blinkDuration);
        //}
        


        GameAudioManager.GetInstance().PlayCardSelected();
    }


    private void OnDisable()
    {
        //textDecoderManager.ResetTexts();
    }


    private IEnumerator PlayShowSteamAnimation()
    {
        _steamCanvasGroup.alpha = 0;
        _upcomingChangesCanvasGroup.alpha = 0;
        yield return StartCoroutine(PlayShowItemAnimation(_steamCanvasGroup, _steamStartDelay, _steamDecoder));
        yield return StartCoroutine(PlayShowItemAnimation(_upcomingChangesCanvasGroup, _upcomingChangesStartDelay, _upcomingChangesDecoder));
    }
    
    private IEnumerator PlayShowItemAnimation(CanvasGroup canvasGroup, float startDelay, TextDecoder textDecoder)
    {        
        yield return new WaitForSeconds(startDelay);

        float blinkDuration = 0.1f;

        for (int i = 0; i < 2; ++i)
        {
            GameAudioManager.GetInstance().PlayCardInfoShown();
            canvasGroup.alpha = 1;
            yield return new WaitForSeconds(blinkDuration);
            canvasGroup.alpha = 0;
            yield return new WaitForSeconds(blinkDuration);
        }
        canvasGroup.alpha = 1;

        textDecoder.Activate();
    }

    public void OpenSteamWishlist()
    {
        Application.OpenURL(STEAM_WISHLIST_LINK);
    }
    
    public void OpenComingChanges()
    {
        _comingChangesMenu.Open();
    }
}
