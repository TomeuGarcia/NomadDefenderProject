using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [Header("PLAY BUTTON")]
    [SerializeField] private GameObject playButtonGO;

    [Header("TEXT DECODERS")]
    [SerializeField] private TextManager textDecoderManager;
    [SerializeField] private TextDecoder titleTextDecoder;
    [SerializeField] private TextDecoder newGameButtonTextDecoder;
    [SerializeField] private TextDecoder playButtonTextDecoder;
    [SerializeField] private TextDecoder quitTextDecoder;

    [Header("MATERIALS SETUP")]
    [SerializeField] private Material obstacleTilesMaterial;
    [SerializeField] private Material tilesMaterial;
    [SerializeField] private Material outerPlanesMaterial;


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
        PauseMenu.GetInstance().gameCanBePaused = false;
    }

    private void SetupTextDecoderManager()
    {
        List<TextDecoder> textDecoders = new List<TextDecoder>();
        textDecoders.Add(titleTextDecoder);
        if (playButtonGO.activeInHierarchy) textDecoders.Add(playButtonTextDecoder);
        textDecoders.Add(newGameButtonTextDecoder);
        textDecoders.Add(quitTextDecoder);

        textDecoderManager.SetTextDecoders(textDecoders);
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
        GameAudioManager.GetInstance().ChangeMusic(GameAudioManager.MusicType.OWMAP, 0.01f);

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
        GameAudioManager.GetInstance().ChangeMusic(GameAudioManager.MusicType.OWMAP,0.01f);
        //Load First Scene
        SceneLoader.GetInstance().StartLoadNormalGame(false);
    }

    public void Quit()
    {
        if (!canInteract) return;

        Application.Quit();
    }

}
