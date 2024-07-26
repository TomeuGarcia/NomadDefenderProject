using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCreditsManager : MonoBehaviour
{
    [SerializeField] private CreditsDisplayer creditsDisplayer;
    [Min(0)] private float delayBeforeLoadingMainMenu = 1.5f;

    private bool hasQuited = false;


    private void Start()
    {
        creditsDisplayer.StartCredits();
        GameAudioManager.GetInstance().ChangeMusic(GameAudioManager.MusicType.MENU, 0.5f);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !hasQuited)
        {
            creditsDisplayer.ForceFinish(false);
            InstantReturnToMainMenu();            
        }
    }


    private void OnEnable()
    {
        PauseMenu.GetInstance().GameCanBePaused = false;

        creditsDisplayer.OnCreditsFinished += ReturnToMainMenu;
    }
    private void OnDisable()
    {
        PauseMenu.GetInstance().GameCanBePaused = true;

        creditsDisplayer.OnCreditsFinished -= ReturnToMainMenu;
    }

    private void ReturnToMainMenu()
    {
        StartCoroutine(DelayedReturnToMainMenu(delayBeforeLoadingMainMenu));
    }
    private void InstantReturnToMainMenu()
    {
        StartCoroutine(DelayedReturnToMainMenu(0f));
    }

    private IEnumerator DelayedReturnToMainMenu(float delay)
    {
        hasQuited = true;

        yield return new WaitForSeconds(delay);
        SceneLoader.GetInstance().StartLoadMainMenu();
        GameAudioManager.GetInstance().ChangeMusic(GameAudioManager.MusicType.MENU, 1f);
    }
}
