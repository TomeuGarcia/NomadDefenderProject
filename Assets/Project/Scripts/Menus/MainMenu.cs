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


    private bool canInteract = true;


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
    }

    private void SetupTextDecoderManager()
    {
        List<TextDecoder> textDecoders = new List<TextDecoder>();
        textDecoders.Add(titleTextDecoder);
        textDecoders.Add(newGameButtonTextDecoder);
        if (playButtonGO.activeInHierarchy) textDecoders.Add(playButtonTextDecoder);
        textDecoders.Add(quitTextDecoder);

        textDecoderManager.SetTextDecoders(textDecoders);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            //Reset All Tutorials
            TutorialsSaverLoader.GetInstance().ResetTutorials();
            Debug.Log("All Tutorials Have Been Reset");
            playButtonGO.SetActive(false);
        }
    }

    public void PlayNewGame()
    {
        if (!canInteract) return;

        canInteract = false;
        TutorialsSaverLoader.GetInstance().ResetTutorials();

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

        StartCoroutine(DoPlay());
    }
    private IEnumerator DoPlay()
    {
        yield return new WaitForSeconds(0.3f);

        //Load First Scene
        SceneLoader.GetInstance().StartLoadNormalGame();
    }

    public void Quit()
    {
        if (!canInteract) return;

        Application.Quit();
    }

}
