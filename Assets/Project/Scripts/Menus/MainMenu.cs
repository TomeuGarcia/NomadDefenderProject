using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] DeckData originalStartDeck;
    [SerializeField] DeckData gameStartDeck;

    private bool canInteract = true;


    public delegate void MainMenuAction();
    public static event MainMenuAction OnPlayStart;

    private void Awake()
    {
        canInteract = true;
    }

    public void Play()
    {
        if (!canInteract) return;

        canInteract = false;
        gameStartDeck.ReplaceFor(originalStartDeck);

        StartCoroutine(DoPlay());
    }

    private IEnumerator DoPlay()
    {
        yield return new WaitForSeconds(0.3f);

        if (OnPlayStart != null) OnPlayStart();
    }

    public void Quit()
    {
        if (!canInteract) return;

        Application.Quit();
    }

}
