using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndCreditsManager : MonoBehaviour
{
    [SerializeField] private CreditsDisplayer creditsDisplayer;
    [Min(0)] private float delayBeforeLoadingMainMenu = 1.5f;


    private void Start()
    {
        creditsDisplayer.StartCredits();
        PauseMenu.GetInstance().GameCanBePaused = false;
    }


    private void OnEnable()
    {
        creditsDisplayer.OnCreditsFinished += ReturnToMainMenu;
    }
    private void OnDisable()
    {
        creditsDisplayer.OnCreditsFinished -= ReturnToMainMenu;
    }

    private void ReturnToMainMenu()
    {
        StartCoroutine(DelayedReturnToMainMenu());
    }

    private IEnumerator DelayedReturnToMainMenu()
    {
        yield return new WaitForSeconds(delayBeforeLoadingMainMenu);
        SceneLoader.GetInstance().StartLoadMainMenu();
        GameAudioManager.GetInstance().ChangeMusic(GameAudioManager.MusicType.MENU, 1f);
    }

}
