using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialGameManager : GameManager
{
    protected override void StartVictory()
    {
        victoryHolder.SetActive(true);

        StartCoroutine(DelayedStartVictorySceneLoad(3f));
    }

    private IEnumerator DelayedStartVictorySceneLoad(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneLoader.GetInstance().StartLoadNormalGame();
    }

}
