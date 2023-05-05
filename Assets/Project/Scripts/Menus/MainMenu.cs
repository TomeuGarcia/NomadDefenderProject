using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [Header("PLAY BUTTON")]
    [SerializeField] private GameObject playButtonGO;
    [SerializeField] private GameObject newGameButton;

    [Header("TEXT DECODERS")]
    [SerializeField] private TextManager textDecoderManager;
    [SerializeField] private TextDecoder titleTextDecoder;
    [SerializeField] private TextDecoder newGameButtonTextDecoder;
    [SerializeField] private TextDecoder creditsButtonDecoder;
    [SerializeField] private TextDecoder playButtonTextDecoder;
    [SerializeField] private TextDecoder quitTextDecoder;

    [Header("MATERIALS SETUP")]
    [SerializeField] private Material obstacleTilesMaterial;
    [SerializeField] private Material tilesMaterial;
    [SerializeField] private Material outerPlanesMaterial;


    private int titleClickCount = 0;
    private string originalTitleString;

    private bool canInteract = true;

    private bool skipFirstBattle = false;


    private void Awake()
    {
        canInteract = true;

        bool finishedTutorails = TutorialsSaverLoader.GetInstance().IsTutorialDone(Tutorials.BATTLE) &&
                                 TutorialsSaverLoader.GetInstance().IsTutorialDone(Tutorials.OW_MAP);
        if (!finishedTutorails)
        {
            playButtonGO.SetActive(false);
            Vector3 pos = newGameButton.GetComponent<RectTransform>().position;
            newGameButton.GetComponent<RectTransform>().position = new Vector3(0.0f, pos.y, pos.z);

            pos = creditsButtonDecoder.transform.parent.GetComponent<RectTransform>().position;
            creditsButtonDecoder.transform.parent.GetComponent<RectTransform>().position = new Vector3(pos.x + 0.25f, pos.y, pos.z);

            pos = quitTextDecoder.transform.parent.GetComponent<RectTransform>().position;
            quitTextDecoder.transform.parent.GetComponent<RectTransform>().position = new Vector3(pos.x - 0.25f, pos.y, pos.z);
        }

        SetupTextDecoderManager();

        obstacleTilesMaterial.SetFloat("_ErrorWiresStep", 0f);
        obstacleTilesMaterial.SetFloat("_AdditionalErrorWireStep2", 0f);
        tilesMaterial.SetFloat("_ErrorWiresStep", 0f);
        tilesMaterial.SetFloat("_AdditionalErrorWireStep2", 0f);
        outerPlanesMaterial.SetFloat("_ErrorWiresStep", 0f);
        outerPlanesMaterial.SetFloat("_AdditionalErrorWireStep2", 0f);        
    }
    private void Start()
    {
        originalTitleString = titleTextDecoder.textStrings[0];
        PauseMenu.GetInstance().gameCanBePaused = false;
    }

    private void SetupTextDecoderManager()
    {
        List<TextDecoder> textDecoders = new List<TextDecoder>();
        textDecoders.Add(titleTextDecoder);
        if (playButtonGO.activeInHierarchy) { textDecoders.Add(playButtonTextDecoder); }
        textDecoders.Add(newGameButtonTextDecoder);
        textDecoders.Add(creditsButtonDecoder);
        textDecoders.Add(quitTextDecoder);

        textDecoderManager.SetTextDecoders(textDecoders);
        //textDecoderManager.ResetTexts();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            skipFirstBattle = true;
            ////Reset All Tutorials
            //TutorialsSaverLoader.GetInstance().ResetTutorials();
            //Debug.Log("All Tutorials Have Been Reset");
            //playButtonGO.SetActive(false);
        }
    }

    public void PlayNewGame()
    {
        if (!canInteract) return;

        canInteract = false;
        PauseMenu.GetInstance().gameCanBePaused = true;
        GameAudioManager.GetInstance().ChangeMusic(GameAudioManager.MusicType.OWMAP, 1f);

        if (skipFirstBattle)
        {
            TutorialsSaverLoader.GetInstance().ResetTutorialsExceptFirstBattle();
        }
        else
        {
            TutorialsSaverLoader.GetInstance().ResetTutorials();
        }

        StartCoroutine(DoPlayNewGame());
    }
    private IEnumerator DoPlayNewGame()
    {
        yield return new WaitForSeconds(0.3f);

        //Load First Scene
        SceneLoader.GetInstance().StartLoadTutorialGame();
    }


    public void Play()
    {
        if (!canInteract) return;

        canInteract = false;
        PauseMenu.GetInstance().gameCanBePaused = true;

        StartCoroutine(DoPlay());
    }
    private IEnumerator DoPlay()
    {
        yield return new WaitForSeconds(0.3f);
        GameAudioManager.GetInstance().ChangeMusic(GameAudioManager.MusicType.OWMAP, 1f);
        //Load First Scene        
        SceneLoader.GetInstance().StartLoadMainMenuCredits();
    }

    public void Credits()
    {
        if (!canInteract) return;

        //TODO: LOAD CREDITS SCENE
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

    private void OnDisable()
    {
        //textDecoderManager.ResetTexts();
    }
}
