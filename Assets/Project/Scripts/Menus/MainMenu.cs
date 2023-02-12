using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] DeckData originalStartDeck;
    [SerializeField] DeckData gameStartDeck;

    private bool canInteract = true;


    private void Awake()
    {
        canInteract = true;
    }

    private void Update()
    {
        if(Input.GetKeyDown("f"))
        {
            //Reset All Tutorials
            TutorialsSaverLoader.GetInstance().ResetTutorials();
            Debug.Log("All Tutorials Have Been Reset");
        }
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

            //Load First Scene
            SceneLoader.GetInstance().StartLoadFirstScene();

    }

    public void Quit()
    {
        if (!canInteract) return;

        Application.Quit();
    }

}
