using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialGameManager : GameManager
{
    [SerializeField] private OWMapTutorialManager2 tutoManager2;

    protected override void StartVictory()
    {
        StartCoroutine(tutoManager2.TutorialAnimation(this));
    }

    public IEnumerator DelayedStartVictorySceneLoad(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneLoader.GetInstance().StartLoadNormalGame(true);
    }

}
