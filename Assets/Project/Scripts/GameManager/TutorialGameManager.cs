using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialGameManager : GameManager
{
    [SerializeField] private OWMapTutorialManager2 tutoManager2;

    [Header("\nREPLACE DECK SELECTOR")]
    [Header("Libraries")]
    [SerializeField] private CardsLibrary cardLibrary;
    [SerializeField] private PartsLibrary partLibrary;

    [Header("Run Content")]
    [SerializeField] private CardsLibraryContent cardsContent;
    [SerializeField] private AttackPartsLibraryContent attacksContent;
    [SerializeField] private BodyPartsLibraryContent bodiesContent;
    [SerializeField] private PassivesLibraryContent basesContent;
    [SerializeField] private BonusStatsPartsLibraryContent bonusStatsContent;


    private void Start()
    {
        cardLibrary.SetContent(cardsContent);
        partLibrary.SetContent(attacksContent, bodiesContent, basesContent, bonusStatsContent);
    }

/*
    protected override void StartVictory()
    {
        StartCoroutine(tutoManager2.TutorialAnimation(this));
    }
*/

    public IEnumerator DelayedStartVictorySceneLoad(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneLoader.GetInstance().StartLoadNormalGame(true);
    }

    public void LoadRegularGame()
    {
        SceneLoader.GetInstance().LoadDeckSelector();
    }

}
