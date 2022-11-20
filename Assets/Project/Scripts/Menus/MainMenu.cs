using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] DeckData originalStartDeck;
    [SerializeField] DeckData gameStartDeck;


    public delegate void MainMenuAction();
    public static event MainMenuAction OnPlayStart;


    public void Play()
    {
        gameStartDeck.ReplaceFor(originalStartDeck);

        StartCoroutine(DoPlay());
    }

    private IEnumerator DoPlay()
    {
        yield return new WaitForSeconds(1f);

        if (OnPlayStart != null) OnPlayStart();
    }

    public void Quit()
    {
        Application.Quit();
    }

}
